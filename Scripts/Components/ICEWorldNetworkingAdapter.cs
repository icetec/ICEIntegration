// ##############################################################################
//
// ICEWorldPathfindingAdapter.cs
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

using ICE;
using ICE.World;

#if ICE_CC
using ICE.Creatures;
using ICE.Creatures.Objects;
using ICE.Creatures.EnumTypes;
#endif

#if ICE_PUN

#endif

namespace ICE.Integration.Adapter
{
#if ICE_PUN && ICE_CC
	public class ICEWorldNetworkingAdapter : Photon.MonoBehaviour {

		private ICECreatureControl m_Creature = null;
		protected ICECreatureControl Creature{
			get{ return m_Creature = ( m_Creature == null ? GetComponent<ICECreatureControl>() : m_Creature ); }
		}

		private ICECreaturePlayer m_Player = null;
		protected ICECreaturePlayer Player{
			get{ return m_Player = ( m_Player == null ? GetComponent<ICECreaturePlayer>() : m_Player ); }
		}

		private ICECreatureEntity m_Entity = null;
		protected ICECreatureEntity Entity{
			get{ return m_Entity = ( m_Entity == null ? GetComponent<ICECreatureEntity>() : m_Entity ); }
		}

		protected float m_LastSynchronizationTime = 0f;
		protected float m_SyncDelay = 0f;
		protected float m_SyncTime = 0f;
		protected Vector3 m_SyncStartPosition = Vector3.zero;
		protected Vector3 m_SyncEndPosition = Vector3.zero;

		public virtual void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
		{
			if( stream.isWriting )
			{
				if( Creature != null )
				{
					stream.SendNext( Creature.BehaviourModeKey );
				}
				else if( Player != null )
				{
				}
				else if( Entity != null )
				{
				}	
			}
			else     												// !stream.isWriting
			{
				if( Creature != null )
				{
					Creature.BehaviourModeKey = (string)stream.ReceiveNext();
				}
				else if( Player != null )
				{
				}
				else if( Entity != null )
				{
				}
				/*m_SyncEndPosition = (Vector3)stream.ReceiveNext();
				m_SyncStartPosition = transform.position;*/

				m_SyncTime = 0f;
				m_SyncDelay = Time.time - m_LastSynchronizationTime;
				m_LastSynchronizationTime = Time.time;
			}
		}

	}
#else
	public class ICEWorldNetworkingAdapter : ICEWorldBehaviour {}
#endif
}
