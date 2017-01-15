// ##############################################################################
//
// ICEWorldNetworkSpawner.cs
// Version 1.3.6
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

using ICE;
using ICE.World;
/*
#if ICE_CC
using ICE.Creatures;
using ICE.Creatures.Objects;
using ICE.Creatures.EnumTypes;
#endif
*/

#if ICE_PUN

#endif

namespace ICE.Integration.Adapter
{
#if ICE_PUN && ICE_CC
	public class ICEWorldNetworkSpawner : Photon.PunBehaviour {

		private ICEWorldRegister m_Register = null;
		protected ICEWorldRegister Register{
			get{ return m_Register = ( m_Register == null ? ICEWorldRegister.Instance : m_Register ); }
		}

		public bool UseDeactivateSceneCreatures = true;
		public bool WaitForUFPSMP = true;

		public bool WaitForExternalSpawnEvent{
			get{
				return WaitForUFPSMP;
			}
		}

		protected virtual void Awake()
		{
			DeactivateSceneCreatures();

			ICEWorldInfo.IsMultiplayer = true;
		}

		/// <summary>
		/// Raises the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			Register.OnRemoveObject += OnRemoveObject;
			Register.OnDestroyObject += OnDestroyObject;
			Register.OnInstantiateObject += OnInstantiateObject; 
		}

		/// <summary>
		/// Raises the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			Register.OnRemoveObject -= OnRemoveObject;
			Register.OnDestroyObject -= OnDestroyObject;
			Register.OnInstantiateObject -= OnInstantiateObject; 
		}

		protected virtual void Update()
		{
			ICEWorldInfo.IsMultiplayer = true;

			if( ! ICEWorldInfo.IsMultiplayer || ! PhotonNetwork.inRoom )
				return;
			
			if( ! WaitForExternalSpawnEvent && ! ICEWorldInfo.NetworkSpawnerReady && PhotonNetwork.inRoom )
				ICEWorldInfo.NetworkSpawnerReady = true;
			else if( vp_MPMaster.Phase == vp_MPMaster.GamePhase.Playing )
				ICEWorldInfo.NetworkSpawnerReady = true;
		}

		public override void OnJoinedRoom()
		{
			if( ! WaitForExternalSpawnEvent )
				ICEWorldInfo.NetworkSpawnerReady = true;
		}

		/// <summary>
		/// Receives the respawn - TransmitRespawnAll()
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="rotation">Rotation.</param>
		/// <param name="info">Info.</param>
		[PunRPC]
		public virtual void ReceivePlayerRespawn( Vector3 position, Quaternion rotation, PhotonMessageInfo info )
		{
			ICEDebug.LogInfo( "Receive RPC PlayerRespawn from vp_MPMaster and set NetworkSpawnerReady to TRUE. Spawning will be available now." );
			ICEWorldInfo.NetworkSpawnerReady = true;
		}

		[PunRPC]
		public void ReceiveLoadLevel(string levelName, PhotonMessageInfo info)
		{
			ICEDebug.LogInfo( "Receive RPC LoadLevel from vp_MPMaster" );
		}

		public void OnInstantiateObject( out GameObject _object, GameObject _reference, Vector3 _position, Quaternion _rotation )
		{

			_object = null;

			ICEWorldEntity _entity = _reference.GetComponent<ICEWorldEntity>();

			if( PhotonNetwork.inRoom )
			{
				if( ! ICEWorldInfo.NetworkSpawnerReady )
					ICEDebug.LogWarning( "NetworkSpawner in Room but not ready!" );

				// a player can spawn its own player only
				if( _entity != null && _entity.EntityType == ICE.World.EnumTypes.EntityClassType.Player )
				{
					ICEDebug.LogInfo( "Spawning Local Player '" + _reference.name + "' via PhotonNetwork." );
					_object = (GameObject)PhotonNetwork.Instantiate( _reference.name, _position, _rotation, 0 );
				}

				// only the master client can spawn scene objects
				else if( PhotonNetwork.isMasterClient )
				{
					if( _entity != null )
						ICEDebug.LogInfo( "Spawning Scene " + _entity.EntityType.ToString() + " Entity '" + _reference.name + "' via PhotonNetwork." );
					else
						ICEDebug.LogInfo( "Spawning Scene Object '" + _reference.name + "' via PhotonNetwork." );

					_object = (GameObject)PhotonNetwork.InstantiateSceneObject( _reference.name, _position, _rotation, 0, null ); 
				}
			}
			else if( ICEWorldInfo.IsMultiplayer == false )
			{
				if( _entity != null )
					ICEDebug.LogInfo( "Instantiate " + _entity.EntityType.ToString() + " Entity '" + _reference.name + "'" );
				else
					ICEDebug.LogInfo( "Instantiate '" + _reference.name + "'." );
				
				_object = (GameObject)GameObject.Instantiate( _reference, _position, _rotation );
			}
		}

		/// <summary>
		/// Raises the destroy object event.
		/// </summary>
		/// <param name="_object">Object.</param>
		/// <param name="_destroyed">Destroyed.</param>
		public void OnRemoveObject( GameObject _object, out bool _destroyed )
		{
			if( PhotonNetwork.isMasterClient == true && _object != null && _object.GetComponent<PhotonView>() != null )
			{
				PhotonNetwork.Destroy( _object );
				_destroyed = true;
			}
			else
				_destroyed = false;
		}

		public void OnDestroyObject( GameObject _object, out bool _destroyed )
		{
			if( PhotonNetwork.isMasterClient == true && _object != null && _object.GetComponent<PhotonView>() != null )
			{
				PhotonNetwork.Destroy( _object );
				_destroyed = true;
			}
			else
				_destroyed = false;
		}
		/*
		static void AddPhotonViewToEntity( ICEWorldEntity _entity, int id)
		{
			PhotonView p = null;

			p = (PhotonView)_entity.gameObject.AddComponent<PhotonView>();

			p.viewID = (id * 1000) + 1;	// TODO: may crash with 'array index out of range' if a entity is deactivated in its prefab
			p.onSerializeTransformOption = OnSerializeTransform.OnlyPosition;
			p.ObservedComponents = new List<Component>();
			p.ObservedComponents.Add(_entity);
			p.synchronization = ViewSynchronization.UnreliableOnChange;

			PhotonNetwork.networkingPeer.RegisterPhotonView(p);
		}*/

		/*
		public void TransmitWakeUp()
		{
			GetComponent<PhotonView>().RPC("ReceiveWakeUp", PhotonTargets.AllBufferedViaServer );
		}




		[PunRPC]
		public void ReceiveWakeUp(PhotonMessageInfo info){

			//if( Application.isEditor )
			//	Debug.Break();

			Debug.Log("ReceivedWakeUp " + transform.name + " (" + transform.GetInstanceID() + " = " + photonView.viewID + " isMine:" + photonView.isMine + " MASTER :" + PhotonNetwork.isMasterClient + ")" );
		}*/



		/// <summary>
		/// deactivates any creature (local or remote) that are present
		/// in the scene upon Awake, because Creature objects may be placed in 
		/// the scene to facilitate updating their prefabs, however once 
		/// a multiplayer session starts only instantiated creature 
		/// are allowed
		/// </summary>
		protected virtual void DeactivateSceneCreatures()
		{
	#if ICE_CC
			if( UseDeactivateSceneCreatures )
			{
				ICE.Creatures.ICECreatureControl[] _creatures = FindObjectsOfType<ICE.Creatures.ICECreatureControl>();
				foreach( ICE.Creatures.ICECreatureControl _creature in _creatures )
					_creature.gameObject.SetActive( false );
			}
	#endif
		}

		protected static Dictionary<int, ICEWorldEntity> m_EntitiesByViewID = new Dictionary<int, ICEWorldEntity>();

		public static ICEWorldEntity GetEntityOfViewID( int _id )
		{

			ICEWorldEntity _entity = null;

			if( ! m_EntitiesByViewID.TryGetValue( _id, out _entity ) )
			{
				PhotonView _view = PhotonView.Find( _id );
				if( _view != null )
				{
					_entity = _view.transform.GetComponent<ICEWorldEntity>();
					if( _entity != null )
						m_EntitiesByViewID.Add( _id, _entity );
					return _entity;
				}
			}

			return _entity;
		}

		protected virtual void TransmitEntityDurability( Transform _target, Transform sourceTransform )
		{
			if( ! PhotonNetwork.isMasterClient )
				return;

			// abort if the status will be synchronized by PhotonSerializeView
			//if( SynchronizeStatus )
			//	return;

			PhotonView _target_view = _target.GetComponent<PhotonView>();
			if( _target_view == null )
				return;

			int _target_view_id = _target_view.viewID;
			if (_target_view_id == 0)
				return;

			ICEWorldEntity _entity = GetEntityOfViewID( _target_view_id );
			if( _entity == null || _entity.IsDestroyed )
				return;

			photonView.RPC( "ReceiveEntityDurability", PhotonTargets.Others, _target_view_id, (float)_entity.Durability ); 
		}

		[PunRPC]
		protected virtual void ReceiveEntityDurability( int _view_id, float _durability, PhotonMessageInfo info )
		{
			if( ( info.sender != PhotonNetwork.masterClient ) || ( info.sender.IsLocal ) )
				return;

			ICEWorldEntity _entity = GetEntityOfViewID( _view_id );
			if( _entity == null )
				return;

			_entity.Status.SetDurability( _durability );

		}
	}
#else
	public class ICEWorldNetworkSpawner : ICEWorldBehaviour {}
#endif
}
