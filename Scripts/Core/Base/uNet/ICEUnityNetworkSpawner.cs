// ##############################################################################
//
// ICEPhotonNetworkSpawner.cs
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

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

using ICE;
using ICE.World;

#if ICE_PUN

#endif

namespace ICE.Integration.Adapter
{

	public abstract class ICEUnityNetworkSpawner : ICEWorldNetworkBehaviour
	{
		public bool AddViewIDToName = true;

		public bool InRoom{
			get{ return true ; }
		}

		public bool IsServer{
			get{ return Network.isServer; }
		}


		protected virtual void Update()
		{
			ICEWorldInfo.IsMultiplayer = true;
			ICEWorldInfo.IsServer = isServer;

			if( ! ICEWorldInfo.NetworkSpawnerReady && InRoom )
				ICEWorldInfo.NetworkSpawnerReady = true;

		}


		public virtual void OnJoinedRoom()
		{
		}

		public void OnInstantiateObject( out GameObject _object, GameObject _reference, Vector3 _position, Quaternion _rotation )
		{
			_object = null;

			ICEWorldEntity _entity = _reference.GetComponent<ICEWorldEntity>();

			if( InRoom )
			{
				if( ! ICEWorldInfo.NetworkSpawnerReady )
					ICEDebug.LogWarning( "NetworkSpawner in Room but not ready!" );

				// a player can spawn its own player only
				if( _entity != null && _entity.EntityType == ICE.World.EnumTypes.EntityClassType.Player )
				{
					ICEDebug.LogInfo( "Spawning Local Player '" + _reference.name + "' via PhotonNetwork." );
					_object = (GameObject)Network.Instantiate( _reference, _position, _rotation, 0 );
				}

				// only the master client can spawn scene objects
				else if( isServer )
				{
					if( _entity != null )
						ICEDebug.LogInfo( "Spawning Scene " + _entity.EntityType.ToString() + " Entity '" + _reference.name + "' via PhotonNetwork." );
					else
						ICEDebug.LogInfo( "Spawning Scene Object '" + _reference.name + "' via PhotonNetwork." );

					//_object = (GameObject)Network.Instantiate( _reference, _position, _rotation, 0 );

					_object = (GameObject)Instantiate( _reference, _position, _rotation );
					NetworkServer.Spawn( _object );
				}
				/*
				if( _object != null )
				{
					if( AddViewIDToName )
					{
						PhotonView _view = _object.GetPhotonView();
						if( _view != null )
							_object.name = _object.name + _view.viewID;
					}
				}*/
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

		public void OnRemoveObject( GameObject _object, out bool _destroyed )
		{
			_destroyed = false;
		}

		public void OnDestroyObject( GameObject _object, out bool _destroyed )
		{
			_destroyed = false;
		}
	}
}
