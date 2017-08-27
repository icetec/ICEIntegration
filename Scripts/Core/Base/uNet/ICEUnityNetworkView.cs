// ##############################################################################
//
// ICEUnityNetworkView.cs
// Version 1.4.0
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_5_5_OR_NEWER 
using UnityEngine.AI;
#endif

using ICE;
using ICE.World;
using ICE.World.Objects;
using ICE.World.Utilities;
using ICE.World.EnumTypes;

using ICE.Integration;
using ICE.Integration.Objects;


[NetworkSettings(channel=Channels.DefaultUnreliable, sendInterval=0)]
#if ICE_UMMORPG
public class ICEUnityNetworkView : NetworkNavMeshAgent 
#else
public class ICEUnityNetworkView : NetworkBehaviour 
#endif
{

	#region Public Objects

	protected ICEWorldEntity m_AttachedEntity = null;
	public ICEWorldEntity AttachedEntity{
		get{ return m_AttachedEntity = ( m_AttachedEntity == null ? ICEWorldEntity.GetWorldEntity( this.gameObject ) : m_AttachedEntity ); }
	}

	protected Rigidbody m_AttachedRigidbody = null;
	public Rigidbody AttachedRigidbody{
		get{ return m_AttachedRigidbody = ( m_AttachedRigidbody == null ? this.gameObject.GetComponent<Rigidbody>() : m_AttachedRigidbody ); }
	}

	protected NavMeshAgent m_AttachedNavMeshAgent = null;
	public NavMeshAgent AttachedNavMeshAgent{
		get{ return m_AttachedNavMeshAgent = ( m_AttachedNavMeshAgent == null ? this.gameObject.GetComponent<NavMeshAgent>() : m_AttachedNavMeshAgent ); }
	}

	#endregion

	#region Public Variables
	public bool IsMasterClient{
		get{ return Network.isServer; }
	}

	#endregion

	#region Private Variables

	private Vector3 m_LastDestination = Vector3.zero; 
	private bool m_HasPath = false; 

	#endregion


	[ServerCallback]
	void Update() {


		if( AttachedNavMeshAgent.hasPath || AttachedNavMeshAgent.pathPending ) 
			m_HasPath = true;
		
		if( AttachedNavMeshAgent.destination != m_LastDestination )
			SetDirtyBit(1);
	}

	// server-side serialization
	//
	// I M P O R T A N T
	//
	// always read and write the same amount of bytes. never let any errors
	// happen. otherwise readstr/readbytes out of range bugs happen.
	public override bool OnSerialize( NetworkWriter _writer, bool _initial_state ) 
	{
		_writer.Write( transform.position );
		_writer.Write( AttachedNavMeshAgent.destination );
		_writer.Write( AttachedNavMeshAgent.speed );
		_writer.Write( AttachedNavMeshAgent.stoppingDistance );
		_writer.Write( AttachedNavMeshAgent.destination != m_LastDestination && ! m_HasPath ); // warped? avoid sliding to respawn point etc.

		// reset helpers
		m_LastDestination = AttachedNavMeshAgent.destination;
		m_HasPath = false;

		return true;
	}

	// client-side deserialization
	//
	// I M P O R T A N T
	//
	// always read and write the same amount of bytes. never let any errors
	// happen. otherwise readstr/readbytes out of range bugs happen.
	public override void OnDeserialize( NetworkReader _reader, bool _initial_state ) 
	{
		var _wrap_position                		= _reader.ReadVector3();
		var _destination               			= _reader.ReadVector3();
		AttachedNavMeshAgent.speed           	= _reader.ReadSingle();
		AttachedNavMeshAgent.stoppingDistance 	= _reader.ReadSingle();
		bool warped            					= _reader.ReadBoolean();

		// OnDeserialize must always return so that next one is called too
		try {
			// only try to set the destination if the agent is on a navmesh already
			// (it might not when falling from the sky after joining)
			if ( AttachedNavMeshAgent.isOnNavMesh ) 
			{
				// warp if necessary. distance check to filter out false positives
				if (warped && Vector3.Distance( _wrap_position, transform.position ) > AttachedNavMeshAgent.radius )
					AttachedNavMeshAgent.Warp( _wrap_position ); // to pos is always smoother

				// rubberbanding: if we are too far off because of a rapid position
				// change or latency, then warp
				// -> agent moves 'speed' meter per seconds
				// -> if we are 2 speed units behind, then we teleport
				//    (using speed is better than using a hardcoded value)
				if( Vector3.Distance( transform.position, _wrap_position ) > AttachedNavMeshAgent.speed * 2 )
					AttachedNavMeshAgent.Warp( _wrap_position );

				// set destination afterwards, so that we never stop going there
				// even after being warped etc.
				AttachedNavMeshAgent.destination = _destination;
			}
		} catch {}
	}
}
