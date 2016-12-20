// ##############################################################################
//
// ICECreatureAstarAdapter.cs
// Version 1.3.5
//
// © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.ice-technologies.com
// mailto:support@ice-technologies.com
// 
// Unity Asset Store End User License Agreement (EULA)
// http://unity3d.com/legal/as_terms
//
// The ICECreatureAstarAdapter Script based on the default movement script AIPath.cs
// which comes with the A* Pathfinding Project and provides ICECreatureControl to 
// use the powerful navigation of the A* Pathfinding System.
//
// ##############################################################################

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if ICE_ASTAR && ICE_CC
using ICE.Creatures;
using ICE.Creatures.Objects;
using ICE.Creatures.EnumTypes;

using Pathfinding;
using Pathfinding.RVO;
#endif

namespace ICE.Integration.Adapter
{
#if ICE_ASTAR && ICE_CC
	[RequireComponent(typeof(ICECreatureControl))]
	[RequireComponent(typeof(Seeker))]
	public class ICECreatureAstarAdapter : MonoBehaviour {

		private ICECreatureControl m_Controller = null;
		protected ICECreatureControl AttachedCreatureController{
			get{ return m_Controller = ( m_Controller == null ? GetComponent<ICECreatureControl>() : m_Controller ); }
		}

		private RVOController m_RVOController = null;
		protected RVOController AttachedRVOController{
			get{ return m_RVOController = ( m_RVOController == null ? GetComponent<RVOController>() : m_RVOController ); }
		}

		private CharacterController m_CharacterController = null;
		protected CharacterController AttachedCharacterController{
			get{ return m_CharacterController = ( m_CharacterController == null ? GetComponent<CharacterController>() : m_CharacterController ); }
		}

		private Rigidbody m_Rigidbody = null;
		protected Rigidbody AttachedRigidbody{
			get{ return m_Rigidbody = ( m_Rigidbody == null ? GetComponent<Rigidbody>() : m_Rigidbody ); }
		}

		private Seeker m_Seeker = null;
		protected Seeker AttachedSeeker{
			get{ return m_Seeker = ( m_Seeker == null ? GetComponent<Seeker>() : m_Seeker ); }
		}


		void OnMoveComplete( GameObject _sender, TargetObject _target  )
		{

		}
		
		void OnMoveUpdatePosition(  GameObject _sender, Vector3 _origin_position, ref Vector3 _new_position )
		{

		}

		void OnTargetMovePositionReached( GameObject _sender, TargetObject _target )
		{
		}

		void OnCustomMove( GameObject _sender, ref Vector3 _new_position, ref Quaternion _new_rotation )
		{
			if( _new_position != m_CurrentMovePosition )
			{
				m_CurrentMovePosition = _new_position;
				SearchPath();
			}
			else
				m_CurrentMovePosition = _new_position;
		}

		void OnPathDelegate( Path _path )
		{
			if( _path == null )
				return;

			Vector3 _end_point = _path.vectorPath[_path.vectorPath.Count-1];
			float _distance = ( _end_point-m_CurrentMovePosition ).sqrMagnitude;

			if( _distance > AttachedCreatureController.Creature.Move.DesiredStoppingDistance )
				Debug.Log( "Distance to EndPoint : " + _distance );
		}

		/// <summary>
		/// Enables or disables searching for paths.
		/// </summary>
		/// <description>Setting this to false does not stop any active path requests from being calculated
		/// or stop it from continuing to follow the current path.
		/// </description>
		public bool AutoRepath = false;

		/// <summary>
		/// Determines how often it will search for new paths.
		/// </summary>
		/// <description>
		/// If you have fast moving targets or AIs, you might want to set it to a lower value.
		/// The value is in seconds between path requests.
		/// </description>
		public float AutoRepathRate = 0.5F;


		/// <summary>
		/// Enables or disables movement.
		/// </summary>
		public bool CanMove = true;
		
		/// <summary>
		/// Distance from the target point where the AI will start to slow down.
		/// </summary>
		/// <description>
		/// Note that this doesn't only affect the end point of the path but also any intermediate points, 
		/// so be sure to set #forwardLook and #pickNextWaypointDist to a higher value than this.
		/// </description>
		public float SlowdownDistance = 0.6F;

		/// <summary>
		/// Determines within what range it will switch to target the next waypoint in the path 
		/// </summary>
		public float PickNextWaypointDist = 2;
		
		/** Target point is Interpolated on the current segment in the path so that it has a distance of #forwardLook from the AI.
	  * See the detailed description of AIPath for an illustrative image */
		public float ForwardLook = 1;
		
		/** Distance to the end point to consider the end of path to be reached.
	 * When this has been reached, the AI will not move anymore until the target changes and OnTargetReached will be called.
	 */
		public float EndReachedDistance = 0.2F;
		
		/// <summary>
		/// Do a closest point on path check when receiving path callback.
		/// </summary>
		/// <description>Usually the AI has moved a bit between requesting the path, and getting it back, 
		/// and there is usually a small gap between the AI and the closest node. If this option is enabled, 
		/// it will simulate, when the path callback is received, movement between the closest node and the 
		/// current AI position. This helps to reduce the moments when the AI just get a new path back, and 
		/// thinks it ought to move backwards to the start of the new path even though it really should just 
		/// proceed forward.</description>
		public bool ClosestOnPathCheck = true;
		
		protected float MinMoveScale = 0.05F;

		/// <summary>
		/// Time when the last path request was sent
		/// </summary>
		protected float m_LastRepath = -9999;
		
		/// <summary>
		/// Current path which is followed
		/// </summary>
		protected Path m_Path;

		protected Transform m_Transform;



		/// <summary>
		/// Current index in the path which is current target		
		/// </summary>
		protected int m_CurrentWaypointIndex = 0;
		
		/// <summary>
		/// Holds if the end-of-path is reached
		/// </summary>
		protected bool m_TargetReached = false;
		
		/// <summary>
		/// Only when the previous path has been returned should be search for a new path
		/// </summary>
		protected bool m_CanSearchAgain = true;
		
		protected Vector3 m_LastFoundWaypointPosition;
		protected float m_LastFoundWaypointTime = -9999;
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="ICE.Creatures.Adapter.ICECreatureAstarAdapter"/> target reached.
		/// </summary>
		/// <value><c>true</c> if target reached; otherwise, <c>false</c>.</value>
		public bool TargetReached {
			get { return m_TargetReached; }
		}
		
		/// <summary>
		/// Holds if the Start function has been run.
		/// </summary>
		/// <description>
		///	Used to test if coroutines should be started in OnEnable to prevent calculating paths
		/// in the awake stage (or rather before start on frame 0).
		/// </description>
		private bool m_StartHasRun = false;

		/// <summary>
		/// Awake this instance and initializes reference variables.
		/// </summary>
		/// <description>
		/// If you override this function you should in most cases call base.Awake () at the start of it.
		/// </description>
		protected virtual void Awake () 
		{			
			if( AttachedRVOController != null ) 
				AttachedRVOController.enableRotation = false;

			//just to optimize the transform component lookup
			m_Transform = transform;		
		}
		
		/// <summary>
		/// Start this instance.
		/// </summary>
		/// <description>
		/// If you override this function you should in most cases call base.Start () at the start of it.
		/// </description>
		protected virtual void Start () 
		{
			m_StartHasRun = true;
			OnEnable ();
		}

		/// <summary>
		/// Raises the enable event and starts RepeatTrySearchPath.
		/// </summary>
		/// <description>
		/// Starts RepeatTrySearchPath.
		/// </description>
		protected virtual void OnEnable () {

			if( AttachedCreatureController != null )
			{
				AttachedCreatureController.Creature.Move.OnTargetMovePositionReached += OnTargetMovePositionReached;
				AttachedCreatureController.Creature.Move.OnMoveComplete += OnMoveComplete;
				AttachedCreatureController.Creature.Move.OnUpdateMovePosition += OnMoveUpdatePosition;
				AttachedCreatureController.Creature.Move.OnCustomMove += OnCustomMove;
			}

			m_LastRepath = -9999;
			m_CanSearchAgain = true;
			
			m_LastFoundWaypointPosition = GetFeetPosition ();
			
			if( m_StartHasRun ) 
			{
				AttachedSeeker.pathCallback += OnPathComplete;
				
				//StartCoroutine( RepeatTrySearchPath() );
			}


		}

		/// <summary>
		/// Raises the disable event.
		/// </summary>
		public void OnDisable () 
		{
			if( AttachedRVOController != null )
				AttachedRVOController.Move( Vector3.zero );

			AttachedCreatureController.Creature.Move.OnTargetMovePositionReached -= OnTargetMovePositionReached;
			AttachedCreatureController.Creature.Move.OnMoveComplete -= OnMoveComplete;
			AttachedCreatureController.Creature.Move.OnUpdateMovePosition -= OnMoveUpdatePosition;
			AttachedCreatureController.Creature.Move.OnCustomMove -= OnCustomMove;

			// Abort calculation of path
			if( AttachedSeeker != null && ! AttachedSeeker.IsDone() ) 
				AttachedSeeker.GetCurrentPath().Error();
			
			// Release current path
			if( m_Path != null ) 
				m_Path.Release( this );

			m_Path = null;
			AttachedSeeker.pathCallback -= OnPathComplete;


		}
		

		/// <summary>
		/// Tries to search for a path every #repathRate seconds.
		/// </summary>
		protected IEnumerator RepeatTrySearchPath () 
		{
			while(true) 
			{
				float _v = TrySearchPath();
				yield return new WaitForSeconds(_v);
			}
		}
		
		/// <summary>
		/// Tries the search path.
		/// </summary>
		/// <returns>returns The time to wait until calling this function again (based on #repathRate)</returns>
		/// <description>
		/// Will search for a new path if there was a sufficient time since the last repath and both
		/// #canSearchAgain and #canSearch are true and there is a target.
		/// </description>
		public float TrySearchPath () 
		{
			if( Time.time - m_LastRepath >= AutoRepathRate && m_CanSearchAgain && AutoRepath ) 
			{
				SearchPath ();
				return AutoRepathRate;
			} 
			else 
			{
				//StartCoroutine (WaitForRepath ());
				float v = AutoRepathRate - (Time.time-m_LastRepath);
				return v < 0 ? 0 : v;
			}
		}

		private Vector3 m_CurrentMovePosition = Vector3.zero;

		/// <summary>
		/// Requests a path to the given MovePosition.
		/// </summary>
		public virtual void SearchPath() 
		{
			m_LastRepath = Time.time;
			//This is where we should search to
			m_CurrentMovePosition = AttachedCreatureController.Creature.Move.MovePosition;
			
			m_CanSearchAgain = false;
			
			//Alternative way of requesting the path
			//ABPath p = ABPath.Construct (GetFeetPosition(),targetPoint,null);
			//seeker.StartPath (p);
			
			//We should search from the current position
			AttachedSeeker.StartPath( GetFeetPosition(), m_CurrentMovePosition, OnPathDelegate );
		}

		/// <summary>
		/// Raises the target reached event.
		/// </summary>
		public virtual void OnTargetReached () 
		{
			//End of path has been reached
			//If you want custom logic for when the AI has reached it's destination
			//add it here
			//You can also create a new script which inherits from this one
			//and override the function in that script
		}

		/// <summary>
		/// Called when a requested path has finished calculation.
		/// </summary>
		/// <param name="_p">_p.</param>
		/// <description>Called when a requested path has finished calculation.
		/// A path is first requested by #SearchPath, it is then calculated, probably in the same or the next frame.
		/// Finally it is returned to the seeker which forwards it to this function.
		/// </description>
		public virtual void OnPathComplete( Path _p ) 
		{
			ABPath _path = _p as ABPath;
			if( _path == null ) throw new System.Exception ("This function only handles ABPaths, do not use special path types");
			
			m_CanSearchAgain = true;
			
			//Claim the new path
			_path.Claim(this);
			
			// Path couldn't be calculated of some reason.
			// More info in p.errorLog (debug string)
			if( _path.error ) 
			{
				_path.Release(this);
				return;
			}
			
			//Release the previous path
			if (m_Path != null) 
				m_Path.Release(this);
			
			//Replace the old path
			m_Path = _path;
			
			//Reset some variables
			m_CurrentWaypointIndex = 0;
			m_TargetReached = false;
			
			//The next row can be used to find out if the path could be found or not
			//If it couldn't (error == true), then a message has probably been logged to the console
			//however it can also be got using p.errorLog
			//if (p.error)
			
			if( ClosestOnPathCheck ) 
			{
				Vector3 _pos_1 = Time.time - m_LastFoundWaypointTime < 0.3f ? m_LastFoundWaypointPosition : _path.originalStartPoint;
				Vector3 _pos_2 = GetFeetPosition();
				Vector3 _direction = _pos_2 - _pos_1;
				float _magnitude = _direction.magnitude;
				_direction /= _magnitude;
				int _steps = (int)(_magnitude/PickNextWaypointDist);				
				
				for( int i = 0; i <= _steps; i++ ) 
				{
					CalculateVelocity( _pos_1 );
					_pos_1 += _direction;
				}
			}
		}

		/// <summary>
		/// Gets the feet position.
		/// </summary>
		/// <returns>The feet position.</returns>
		public virtual Vector3 GetFeetPosition() 
		{
			if( AttachedCharacterController != null && AttachedCharacterController.enabled ) 
				return m_Transform.position - Vector3.up * AttachedCharacterController.height * 0.5f;
			else
				return m_Transform.position;
		}
		
		public virtual void Update () {
			
			if( ! CanMove || AttachedCreatureController == null || ! AttachedCreatureController.enabled )
			{
				if( AttachedRVOController != null )
					AttachedRVOController.Move( Vector3.zero );

				return;
			}

			if( m_CurrentMovePosition != AttachedCreatureController.Creature.Move.MovePosition  )
				TrySearchPath();

			Vector3 _velocity = CalculateVelocity( GetFeetPosition() );
			
			//Rotate towards targetDirection (filled in by CalculateVelocity)
			RotateTowards( m_TargetDirection );

			if( AttachedRVOController != null && AttachedRVOController.enabled )
			{
				AttachedRVOController.maxSpeed = AttachedCreatureController.Creature.Move.MoveSpeed;
				AttachedRVOController.rotationSpeed = AttachedCreatureController.Creature.Move.MoveAngularSpeed;
				AttachedRVOController.Move( _velocity );
			} 
			else if( AttachedCharacterController != null && AttachedCharacterController.enabled ) 
			{
				AttachedCharacterController.SimpleMove( _velocity );
			} 
			else if( AttachedRigidbody != null && ! AttachedRigidbody.isKinematic ) 
			{
				AttachedRigidbody.AddForce( _velocity );
			} 
			else 
			{
				m_Transform.Translate( _velocity * Time.deltaTime, Space.World );
			}
		}
		
		/** Point to where the AI is heading.
	  * Filled in by #CalculateVelocity */
		protected Vector3 m_TargetPoint;
		/** Relative direction to where the AI is heading.
	 * Filled in by #CalculateVelocity */
		protected Vector3 m_TargetDirection;

		/// <summary>
		/// XZs the sqr magnitude.
		/// </summary>
		/// <returns>The sqr magnitude.</returns>
		/// <param name="_a">_a.</param>
		/// <param name="_b">_b.</param>
		protected float XZSqrMagnitude( Vector3 _a, Vector3 _b ) 
		{
			float _dx = _b.x-_a.x;
			float _dz = _b.z-_a.z;
			return _dx*_dx + _dz*_dz;
		}

		/// <summary>
		/// Calculates the desired velocity.
		/// </summary>
		/// <returns>The velocity.</returns>
		/// <param name="_position">_position.</param>
		/// <description>
		/// Finds the target path segment and returns the forward direction, scaled with speed.
		/// A whole bunch of restrictions on the velocity is applied to make sure it doesn't 
		/// overshoot, does not look too far ahead, and slows down when close to the target.
		/// </description>
		protected Vector3 CalculateVelocity( Vector3 _position ) 
		{
			if( m_Path == null || m_Path.vectorPath == null || m_Path.vectorPath.Count == 0 ) 
				return Vector3.zero;
			
			List<Vector3> _vector_path = m_Path.vectorPath;
			
			if( _vector_path.Count == 1 ) 
				_vector_path.Insert( 0,_position );

			if( m_CurrentWaypointIndex >= _vector_path.Count )
				m_CurrentWaypointIndex = _vector_path.Count-1;
			
			if( m_CurrentWaypointIndex <= 1 ) 
				m_CurrentWaypointIndex = 1;
			
			while( true ) 
			{
				if( m_CurrentWaypointIndex < _vector_path.Count - 1 ) 
				{
					//There is a "next path segment"
					float _dist = XZSqrMagnitude( _vector_path[ m_CurrentWaypointIndex ], _position );

					//Mathfx.DistancePointSegmentStrict (vPath[currentWaypointIndex+1],vPath[currentWaypointIndex+2],currentPosition);
					if( _dist < PickNextWaypointDist*PickNextWaypointDist ) 
					{
						m_LastFoundWaypointPosition = _position;
						m_LastFoundWaypointTime = Time.time;
						m_CurrentWaypointIndex++;
					} 
					else
						break;
				} 
				else
					break;
			}
			
			Vector3 _direction = _vector_path[ m_CurrentWaypointIndex ] - _vector_path[ m_CurrentWaypointIndex - 1 ];
			Vector3 _target_position = CalculateTargetPoint( _position, _vector_path[ m_CurrentWaypointIndex - 1 ], _vector_path[m_CurrentWaypointIndex]);
			
			
			_direction = _target_position - _position;
			_direction.y = 0;
			float _target_distance = _direction.magnitude;
			
			float _slowdown = Mathf.Clamp01( _target_distance / SlowdownDistance );
			
			this.m_TargetDirection = _direction;
			this.m_TargetPoint = _target_position;
			
			if( m_CurrentWaypointIndex == _vector_path.Count-1 && _target_distance <= EndReachedDistance ) 
			{
				if( ! m_TargetReached ) 
				{
					m_TargetReached = true; 
					OnTargetReached(); 
				}
				return Vector3.zero;
			}
			
			Vector3 _forward = m_Transform.forward;
			float _dot = Vector3.Dot( _direction.normalized, _forward );

			float _forward_speed = AttachedCreatureController.Creature.Move.MoveSpeed;
			float _speed = _forward_speed * Mathf.Max( _dot, MinMoveScale ) * _slowdown;
			
			
			if( Time.deltaTime	> 0 )
				_speed = Mathf.Clamp( _speed ,0, _target_distance/( Time.deltaTime*2  ));

			return _forward*_speed;
		}
		
		/// <summary>
		/// Rotates in the specified direction.
		/// </summary>
		/// <param name="_direction">_direction.</param>
		protected virtual void RotateTowards( Vector3 _direction ) {
			
			if( _direction == Vector3.zero ) 
				return;
			
			Quaternion _rotation = m_Transform.rotation;
			Quaternion _to_target = Quaternion.LookRotation( _direction );

			float _turning_speed = AttachedCreatureController.Creature.Move.DesiredAngularVelocity.y;

			_rotation = Quaternion.Slerp( _rotation,_to_target,_turning_speed * Time.deltaTime );
			_rotation = Quaternion.Euler( new Vector3( 0, _rotation.eulerAngles.y, 0 ) );
			
			m_Transform.rotation = _rotation;
		}

		/// <summary>
		/// Calculates target point from the current line segment.		
		/// </summary>
		/// <returns>The target point.</returns>
		/// <param name="_p">_p.</param>
		/// <param name="_a">_a.</param>
		/// <param name="_b">_b.</param>
		protected Vector3 CalculateTargetPoint( Vector3 _position, Vector3 _segment_start, Vector3 _segment_end ) 
		{
			_segment_start.y = _position.y;
			_segment_end.y = _position.y;
			
			float _magnitude = (_segment_start-_segment_end).magnitude;
			if( _magnitude == 0 ) 
				return _segment_start;


			float _closest = Mathf.Clamp01 (VectorMath.ClosestPointOnLineFactor(_segment_start, _segment_end, _position));
			Vector3 _point = (_segment_end-_segment_start)*_closest + _segment_start;
			float _distance = (_point-_position).magnitude;
			
			float _look_ahead = Mathf.Clamp( ForwardLook - _distance, 0.0F, ForwardLook );
			
			float _offset = _look_ahead / _magnitude;
			_offset = Mathf.Clamp( _offset + _closest, 0.0F , 1.0F );
			return (_segment_end-_segment_start)*_offset + _segment_start;
		}
	}
#else
	public class ICECreatureAstarAdapter : MonoBehaviour {}
#endif
}
