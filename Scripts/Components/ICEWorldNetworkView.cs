// ##############################################################################
//
// ICEWorldNetworkView.cs
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
	public class ICEWorldNetworkView : Photon.MonoBehaviour, IPunObservable 
	{

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

		public bool SynchronizeMovement = false;
		public bool SynchronizeStatus = false;

		public float SycnMovementSmoothingDelay = 5;

		protected float m_LastSynchronizationTime = 0f;
		protected float m_SyncDelay = 0f;
		protected float m_SyncTime = 0f;
		protected Vector3 m_SyncStartPosition = Vector3.zero;

		private Vector3 m_SyncEndPosition = Vector3.zero; //We lerp towards this
		private Quaternion m_SyncEndRotation = Quaternion.identity; //We lerp towards this

		public void Awake()
		{
			bool _observed = false;
			foreach( Component _observed_component in this.photonView.ObservedComponents )
			{
				if( _observed_component == this )
				{
					_observed = true;
					break;
				}
			}

			if( ! _observed )
			{
				ICEDebug.LogWarning( this + " is not observed by this object's photonView! OnPhotonSerializeView() in this class won't be used." );
			}

			
			TransmitWakeUp();

			if( Entity != null )
				Entity.IsLocal = photonView.isMine;
		}

		public void Update()
		{
			if( Entity != null )
				Entity.IsLocal = photonView.isMine;
			
			if( SynchronizeMovement && ! photonView.isMine && this.SycnMovementSmoothingDelay > 0 )
			{
				transform.position = Vector3.Lerp( transform.position, m_SyncEndPosition, Time.deltaTime * this.SycnMovementSmoothingDelay );
				transform.rotation = Quaternion.Lerp( transform.rotation, m_SyncEndRotation, Time.deltaTime * this.SycnMovementSmoothingDelay );
			}
		}

		public void TransmitWakeUp()
		{
			GetComponent<PhotonView>().RPC("ReceiveWakeUp", PhotonTargets.AllBufferedViaServer );
		}




		[PunRPC]
		public void ReceiveWakeUp(PhotonMessageInfo info){

			//if( Application.isEditor )
			//	Debug.Break();
			
			Debug.Log("ReceivedWakeUp " + transform.name + " (" + transform.GetInstanceID() + " = " + photonView.viewID + " isMine:" + photonView.isMine + " MASTER :" + PhotonNetwork.isMasterClient + ")" );
		}

		public virtual void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
		{
			if( stream.isWriting )
			{
				if( Creature != null )
				{
					stream.SendNext( Creature.BehaviourModeKey );

					if( SynchronizeMovement )
					{
						stream.SendNext(transform.position);
						stream.SendNext(transform.rotation);
					}

					if( SynchronizeStatus )
					{
						stream.SendNext( Creature.Age );
						stream.SendNext( Creature.StatusDamageInPercent );
						stream.SendNext( Creature.StatusStressInPercent );
						stream.SendNext( Creature.StatusDebilityInPercent );
						stream.SendNext( Creature.StatusThirstInPercent );
						stream.SendNext( Creature.StatusHungerInPercent );
					}
				}
				else if( Player != null )
				{
					if( SynchronizeMovement )
					{
						stream.SendNext( transform.position );
						stream.SendNext( transform.rotation );
					}

					if( SynchronizeStatus )
					{
						stream.SendNext( Player.Age );
						stream.SendNext( Player.DurabilityInPercent );
					}
				}
				else if( Entity != null )
				{
					if( SynchronizeStatus )
					{
						stream.SendNext( Entity.Age );
						stream.SendNext( Entity.DurabilityInPercent );
					}
				}	
			}
			else     												// !stream.isWriting
			{
				if( Creature != null )
				{
					Creature.BehaviourModeKey = (string)stream.ReceiveNext();

					if( SynchronizeMovement )
					{
						m_SyncEndPosition = (Vector3)stream.ReceiveNext();
						m_SyncEndRotation = (Quaternion)stream.ReceiveNext();
					}

					if( SynchronizeStatus )
					{
						Creature.Status.SetAge( (float)stream.ReceiveNext() );
						Creature.StatusDamageInPercent = (float)stream.ReceiveNext();
						Creature.StatusStressInPercent = (float)stream.ReceiveNext();
						Creature.StatusDebilityInPercent = (float)stream.ReceiveNext();
						Creature.StatusThirstInPercent = (float)stream.ReceiveNext();
						Creature.StatusHungerInPercent = (float)stream.ReceiveNext();
					}
				}
				else if( Player != null )
				{
					if( SynchronizeMovement )
					{
						m_SyncEndPosition = (Vector3)stream.ReceiveNext();
						m_SyncEndRotation = (Quaternion)stream.ReceiveNext();
					}

					if( SynchronizeStatus )
					{
						Player.Status.SetAge( (float)stream.ReceiveNext() );
						Player.Status.UpdateDurabilityByPercent( (float)stream.ReceiveNext() );
					}
				}
				else if( Entity != null )
				{
					if( SynchronizeStatus )
					{
						Entity.Status.SetAge( (float)stream.ReceiveNext() );
						Entity.Status.UpdateDurabilityByPercent( (float)stream.ReceiveNext() );
					}
				}

				m_SyncTime = 0f;
				m_SyncDelay = Time.time - m_LastSynchronizationTime;
				m_LastSynchronizationTime = Time.time;
			}
		}
		/*
		[PunRPC]
		public void RequestInitialSpawnInfo(PhotonPlayer player, int id, string name)
		{
		}*/




	}
#else
	public class ICEWorldNetworkView : ICEWorldBehaviour {}
#endif
}
