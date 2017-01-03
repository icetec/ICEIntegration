// ##############################################################################
//
// ICEWorldNetworkManager.cs
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
	[RequireComponent(typeof(ICEWorldRegister))]
	public class ICEWorldNetworkManager : Photon.MonoBehaviour {

		private ICEWorldRegister m_Register = null;
		protected ICEWorldRegister Register{
			get{ return m_Register = ( m_Register == null ? GetComponent<ICEWorldRegister>() : m_Register ); }
		}

		public bool AutoConnect = true;

		public bool UseMultiplayer = true;

		public byte Version = 1;

		private bool ConnectInUpdate = true;

		void Awake()
		{
			if( Register == null )
				return;

			// deactivates the pool management if it is not the master
			if( PhotonNetwork.isMasterClient == false )
			{
				//m_Register.UsePoolManagement = false;
			}
		}
		// Use this for initialization
		void Start () {


		}


		/// <summary>
		/// Raises the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			ICEWorldInfo.IsMultiplayer = UseMultiplayer;
			ICEWorldInfo.NetworkConnectedAndReady = false;

			Register.OnRemoveObject += OnRemoveObject;
			//Register.OnSpawnObject += OnSpawnObject;
			Register.OnInstantiateObject += OnInstantiateObject; 
		}

		/// <summary>
		/// Raises the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			ICEWorldInfo.IsMultiplayer = false;
			ICEWorldInfo.NetworkConnectedAndReady = false;

			Register.OnRemoveObject -= OnRemoveObject;
			//Register.OnSpawnObject -= OnSpawnObject;
			Register.OnInstantiateObject -= OnInstantiateObject; 
		}

		protected virtual void Update()
		{
			if( ConnectInUpdate && AutoConnect && ! PhotonNetwork.connected )
			{
				Debug.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");

				ConnectInUpdate = false;
				PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
			}

			if( ICEWorldInfo.IsMasterClient != PhotonNetwork.isMasterClient )
				ICEWorldInfo.IsMasterClient = PhotonNetwork.isMasterClient;

			if( ICEWorldInfo.NetworkConnectedAndReady != PhotonNetwork.inRoom )
				ICEWorldInfo.NetworkConnectedAndReady = PhotonNetwork.inRoom;
		}

		public virtual void OnConnectedToMaster()
		{
			Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
			PhotonNetwork.JoinRandomRoom();
		}

		public virtual void OnJoinedLobby()
		{
			Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
			PhotonNetwork.JoinRandomRoom();
		}

		public virtual void OnPhotonRandomJoinFailed()
		{
			Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
			PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 }, null);
		}

		public virtual void OnFailedToConnectToPhoton( DisconnectCause _cause )
		{
			Debug.LogError( "Cause: " + _cause );
		}

		public virtual void OnJoinedRoom()
		{
			Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");

			ICEWorldInfo.NetworkConnectedAndReady = true;
		}



		public void OnInstantiateObject( out GameObject _object, GameObject _reference, Vector3 _position, Quaternion _rotation )
		{
			_object = null;

			ICEWorldEntity _entity = _reference.GetComponent<ICEWorldEntity>();

			// a player can spawn its own player only
			if( _entity != null && _entity.EntityType == ICE.World.EnumTypes.EntityClassType.Player )
				_object = PhotonNetwork.Instantiate( _reference.name, _position, _rotation, 0 );

			// only the master client can spawn scene objects
			else if( PhotonNetwork.isMasterClient == true )
				_object = PhotonNetwork.InstantiateSceneObject( _reference.name, _position, _rotation, 0, null ); 
		}

		/*
		/// <summary>
		/// Raises the spawn object event.
		/// </summary>
		/// <param name="_object">Object.</param>
		/// <param name="_reference">Reference.</param>
		/// <param name="_position">Position.</param>
		/// <param name="_rotation">Rotation.</param>
		public void OnSpawnObject( out GameObject _object, GameObject _reference, Vector3 _position, Quaternion _rotation )
		{
			_object = null;

			if( _reference != null && PhotonNetwork.isMasterClient == true )
			{
				//if( _reference.GetComponent<ICECreaturePlayer>() == null )
					_object = PhotonNetwork.InstantiateSceneObject( _reference.name, _position, _rotation, 0, null ); 
				//else
				//	_object = PhotonNetwork.Instantiate( _reference.name, _position, _rotation, 0 );
			}
		}
		*/

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
	}
#else
	public class ICEWorldNetworkManager : ICEWorldBehaviour {}
#endif
}
