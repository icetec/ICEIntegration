// ##############################################################################
//
// ICEWorldNetworkManager.cs
// Version 1.3.7
//
// © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.ice-technologies.com
// mailto:support@ice-technologies.com
// 
// Unity Asset Store End User License Agreement (EULA)
// http://unity3d.com/legal/as_terms
//
// ##############################################################################

using UnityEngine;
using UnityEngine.UI;
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;


using ICE;
using ICE.World;
using ICE.World.Objects;
using ICE.World.Utilities;

using ICE.Integration;
using ICE.Integration.Objects;


#if ICE_PUN

#endif

namespace ICE.Integration.Adapter
{

#if ICE_PUN

	public class ICEWorldNetworkManager : Photon.PunBehaviour
	{
		protected static ICEWorldNetworkManager m_Instance = null;
		public static ICEWorldNetworkManager Instance{
			get{ return m_Instance = ( m_Instance == null?GameObject.FindObjectOfType<ICEWorldNetworkManager>():m_Instance ); }
		}

		[SerializeField]
		private NetworkManagerDisplayOptionsObject m_DisplayOptions = null;
		public NetworkManagerDisplayOptionsObject DisplayOptions{
			get{ return m_DisplayOptions = ( m_DisplayOptions== null?new NetworkManagerDisplayOptionsObject():m_DisplayOptions ); }
			set{ DisplayOptions.Copy( value ); }
		}

		[SerializeField]
		private NetworkManagerSceneManagementObject m_SceneManagement = null;
		public NetworkManagerSceneManagementObject SceneManagement{
			get{ return m_SceneManagement = ( m_SceneManagement== null?new NetworkManagerSceneManagementObject():m_SceneManagement ); }
			set{ SceneManagement.Copy( value ); }
		}

		[SerializeField]
		private NetworkManagerConnectionManagementObject m_Connection = null;
		public NetworkManagerConnectionManagementObject Connection{
			get{ return m_Connection = ( m_Connection== null?new NetworkManagerConnectionManagementObject():m_Connection ); }
			set{ Connection.Copy( value ); }
		}

		private Camera m_ChildCamera = null;
		private Camera ChildCamera{
			get{ return m_ChildCamera = ( m_ChildCamera == null ? gameObject.GetComponentInChildren<Camera>() : m_ChildCamera ); }
		}

		public bool UseDebugLogs = false;
		public bool UsePhotonDebugLogs = false;
		public bool UseDontDestroyOnLoad = false;

		private ClientState m_ClientState = ClientState.Uninitialized;
		protected ClientState ClientConnectionState{
			get{ return m_ClientState; }
		}
			
		private bool m_HasJoinedLobby = false;
		private bool m_ReceivedRoomListUpdate = false;

		private bool InsideLobby{
			get{ return ( PhotonNetwork.insideLobby ? true : false ); }
		}

		private bool InRoom{
			get{ return ( PhotonNetwork.inRoom ? true : false ); }
		}

		private static bool m_StayConnected = false;

		public virtual void Awake()
		{
			if( m_Instance == null )
				m_Instance = this;
			else if( m_Instance != this )
			{
				ICEDebug.LogWarning( "Multiple ICEWorldNetworkManagers was detected in scene, this is not supported and the redundant one (" + gameObject.name + ") will be removed!" );
				Destroy( gameObject ); 
			}


			ICEWorldInfo.IsMultiplayer = Connection.UseMultiplayer;
		}

		public virtual void Start()
		{
			if( UseDontDestroyOnLoad )
				DontDestroyOnLoad( transform.root.gameObject );


			PhotonNetwork.logLevel = ( UsePhotonDebugLogs ? PhotonLogLevel.Full : PhotonLogLevel.ErrorsOnly );

			if( Connection.AutoConnect || m_StayConnected )
				Connect();

		}
			
		/// <summary>
		/// Raises the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			if( Connection.Enabled && Connection.AutoConnect == false && ( DisplayOptions.ConnectionPanel == null || DisplayOptions.ButtonConnect == null ) ){
				ICEDebug.LogWarning( "Connection GUI elements missing! Check 'ConnectionPanel' and 'ButtonConnect' or activate 'AutoConnect'!" );
			}

			if( Connection.Enabled && ( ! Connection.AutoJoinLobby || ! Connection.JoinRandomRoom ) && ( DisplayOptions.LobbyPanel == null || DisplayOptions.ButtonJoin == null ) ){
				ICEDebug.LogWarning( "Lobby GUI elements missing! Check 'LobbyPanel' and 'ButtonJoin' or activate 'AutoJoinLobby' and 'JoinRandomRoom'!" );
			}

			if( ! DisplayOptions.UseStatusLog  && ( DisplayOptions.ImageStatus == null || DisplayOptions.TextStatus == null ) ){
				ICEDebug.LogWarning( "Status GUI elements missing! Check 'ImageStatus' and 'TextStatus' or the connection state can not be displayed!" );
			}
				
			if( DisplayOptions.DropdownRooms != null ){
				DisplayOptions.DropdownRooms.ClearOptions(); 
				DisplayOptions.DropdownRooms.AddOptions( Connection.Rooms );
			}

			if( DisplayOptions.DropdownScreen != null ){
				DisplayOptions.DropdownScreen.onValueChanged.RemoveAllListeners();
				DisplayOptions.DropdownScreen.onValueChanged.AddListener( delegate { OnGUIChangedScreen(); });
			}

			if( DisplayOptions.DropdownQuality != null ){
				DisplayOptions.DropdownQuality.onValueChanged.RemoveAllListeners();
				DisplayOptions.DropdownQuality.onValueChanged.AddListener( delegate { OnGUIChangedQuality(); });
			}

			if( DisplayOptions.ButtonConnect != null ){
				DisplayOptions.ButtonConnect.onClick.RemoveAllListeners();
				DisplayOptions.ButtonConnect.onClick.AddListener( delegate { OnGUIConnect(); });
			}

			if( DisplayOptions.ButtonJoin != null ){
				DisplayOptions.ButtonJoin.onClick.RemoveAllListeners();
				DisplayOptions.ButtonJoin.onClick.AddListener( delegate { OnGUIJoinRoom(); });
			}

			if( DisplayOptions.ButtonDisconnect != null ){
				DisplayOptions.ButtonDisconnect.onClick.RemoveAllListeners();
				DisplayOptions.ButtonDisconnect.onClick.AddListener( delegate { OnGUIDisconnect(); });
			}

			if( DisplayOptions.ButtonQuit != null ){
				DisplayOptions.ButtonQuit.onClick.RemoveAllListeners();
				DisplayOptions.ButtonQuit.onClick.AddListener( delegate { OnGUIQuit(); });
			}

			if( DisplayOptions.InputFieldPlayerName != null ){
				DisplayOptions.InputFieldPlayerName.onValueChanged.RemoveAllListeners();
				DisplayOptions.InputFieldPlayerName.onValueChanged.AddListener( delegate { OnGUIPlayerNameChanged(); });
			}

			if( DisplayOptions.ToggleJoinRandomRoom != null ){
				DisplayOptions.ToggleJoinRandomRoom.onValueChanged.RemoveAllListeners();
				DisplayOptions.ToggleJoinRandomRoom.onValueChanged.AddListener( delegate { OnGUIJoinRandomRoomToggleChanged(); });
			}

			ICEWorldInfo.IsMasterClient = PhotonNetwork.isMasterClient;
			if( PhotonNetwork.inRoom )
				ICEWorldInfo.NetworkConnectedAndReady = true;
		}

		/// <summary>
		/// Raises the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			ICEWorldInfo.NetworkConnectedAndReady = false;
		}

		public virtual void Update()
		{
			UpdateConnectionState();
			DisplayOptions.UpdateStatusColor();
		}

		private float m_LogOnTimeOut = 5.0f; 
		private int m_ConnectionAttemptsMaximum = 10;
		private int m_ConnectionAttempts = 0;
		private bool m_ConnectionTimerActive = false;
		private float m_ConnectionTimer = 0;

		private void UpdateDisplay( BasicNetworkStatus _state = BasicNetworkStatus.Undefined )
		{
			if( _state == BasicNetworkStatus.Undefined )
			{
				switch( PhotonNetwork.connectionStateDetailed )
				{
				case ClientState.Queued:
				case ClientState.QueuedComingFromGameserver:
				case ClientState.Disconnecting:
				case ClientState.DisconnectingFromMasterserver:
				case ClientState.DisconnectingFromNameServer:
				case ClientState.DisconnectingFromGameserver:
				case ClientState.Authenticating:
				case ClientState.ConnectingToMasterserver:
				case ClientState.ConnectingToNameServer:
				case ClientState.ConnectingToGameserver:		
				case ClientState.Joining:
				case ClientState.Leaving:
					_state = BasicNetworkStatus.Connecting;
					break;
				case ClientState.Authenticated:
				case ClientState.ConnectedToNameServer:			
				case ClientState.ConnectedToMaster:
				case ClientState.ConnectedToGameserver:
					_state = BasicNetworkStatus.Connected;
					break;
				case ClientState.Uninitialized:
					_state = BasicNetworkStatus.Disconnected;
					break;
				case ClientState.PeerCreated:
				case ClientState.Disconnected:
					_state = BasicNetworkStatus.Disconnected;
					break;
				case ClientState.JoinedLobby:
					_state = BasicNetworkStatus.LobbyJoined;
					break;
				case ClientState.Joined:
					_state = BasicNetworkStatus.RoomJoined;
					break;
				}
			}

			DisplayOptions.SetColorByNetworkStatus( _state );
		}

		protected virtual void UpdateConnectionState()
		{
			ClientState _state = PhotonNetwork.connectionStateDetailed;

			if( ICEWorldInfo.IsMultiplayer != Connection.UseMultiplayer )
			{
				ICEDebug.LogWarning( "Multiplayer state changed during the runtime to '" + Connection.UseMultiplayer.ToString() + "'!" );
				ICEWorldInfo.IsMultiplayer = Connection.UseMultiplayer;
			}

			if( ICEWorldInfo.IsMasterClient != PhotonNetwork.isMasterClient )
			{
				ICEDebug.LogStatus( "Client state changed to " + ( PhotonNetwork.isMasterClient ? "MASTER" : "REMOTE" ) + "!", UseDebugLogs );
				ICEWorldInfo.IsMasterClient = PhotonNetwork.isMasterClient;
			}

			if( _state != m_ClientState )
			{
				string _msg = _state.ToString();

				switch( _state )
				{			
					case ClientState.PeerCreated:						
						_msg = ( ! Connection.AutoConnect ? "Peer Created, ready to connect!" : "Connecting to the " + PhotonNetwork.PhotonServerSettings.PreferredRegion.ToString().ToUpper() + " cloud ..." );
						break;
					case ClientState.ConnectedToMaster:
					case ClientState.Authenticated:
					case ClientState.JoinedLobby:
						_msg =  "--- " + _msg + " ---";
						break;
					case ClientState.Joining:
						_msg =  "Joining ...";
						break;
					case ClientState.Joined:
						_msg =  "*** " + _msg + ( m_HasJoinedLobby && PhotonNetwork.room != null ? " " + PhotonNetwork.room.Name : "Random Room" ) + " ***";
						break;
				}

				UpdateDisplay();

				DisplayOptions.Log( _msg );

				if( _state == ClientState.Joined && ! Connection.HandleConnection )
					DisplayOptions.Log( "" );
					
				ICEDebug.LogStatus( "Connection state changed from " + m_ClientState.ToString() + " to " + PhotonNetwork.connectionStateDetailed.ToString(), UseDebugLogs );

				m_ClientState = _state;
			}


			if( DisplayOptions.StatusPanel != null && ! DisplayOptions.StatusPanel.gameObject.activeSelf ) 
				DisplayOptions.StatusPanel.gameObject.SetActive( true );

			if( _state != ClientState.Joined )
			{									
				if( _state == ClientState.PeerCreated || _state == ClientState.Disconnected )
				{
					if( DisplayOptions.ConnectionPanel != null ) DisplayOptions.ConnectionPanel.gameObject.SetActive( true );
					if( DisplayOptions.LobbyPanel != null ) DisplayOptions.LobbyPanel.gameObject.SetActive( false );

					if( DisplayOptions.ButtonDisconnect != null ) DisplayOptions.ButtonDisconnect.gameObject.SetActive( false );
				}
				else
				{
					if( DisplayOptions.ButtonDisconnect != null ) DisplayOptions.ButtonDisconnect.gameObject.SetActive( true );
					/*
					 * all connection without autoJoinLobby will be currently handled within OnConnectedToMaster() by joining a random room
					if( ( _state == ClientState.ConnectedToMaster || _state == ClientState.Authenticated ) && ! PhotonNetwork.autoJoinLobby )
					{
						if( DisplayOptions.ConnectionPanel != null ) DisplayOptions.ConnectionPanel.gameObject.SetActive( true );
						if( DisplayOptions.LobbyPanel != null ) DisplayOptions.LobbyPanel.gameObject.SetActive( false );
					}
					else */
					if( _state == ClientState.JoinedLobby && ! Connection.JoinRandomRoom )
					{
						if( DisplayOptions.ConnectionPanel != null ) DisplayOptions.ConnectionPanel.gameObject.SetActive( false );
						if( DisplayOptions.LobbyPanel != null ) DisplayOptions.LobbyPanel.gameObject.SetActive( true );
					}
					else
					{
						if( DisplayOptions.ConnectionPanel != null ) DisplayOptions.ConnectionPanel.gameObject.SetActive( false );
						if( DisplayOptions.LobbyPanel != null ) DisplayOptions.LobbyPanel.gameObject.SetActive( false );
					}
				}
			}
			else
			{
				if( DisplayOptions.ConnectionPanel != null && ! DisplayOptions.ConnectionPanel.gameObject.activeSelf ) 
					DisplayOptions.ConnectionPanel.gameObject.SetActive( false );
				
				if( DisplayOptions.LobbyPanel != null && ! DisplayOptions.LobbyPanel.gameObject.activeSelf ) 
					DisplayOptions.LobbyPanel.gameObject.SetActive( false );
			}
		}

		private int m_DefaultScreenWidth = 640;
		private int m_DefaultScreenHeight = 480;

		public void OnGUIJoinRandomRoomToggleChanged(){

			if( DisplayOptions.ToggleJoinRandomRoom != null && DisplayOptions.DropdownRooms != null  )
			{ 
				if( DisplayOptions.ToggleJoinRandomRoom.isOn )
					DisplayOptions.DropdownRooms.interactable = false;
				else
					DisplayOptions.DropdownRooms.interactable = true;					
			}
		}


		public void OnGUIPlayerNameChanged(){

			if( DisplayOptions.InputFieldPlayerName != null )
			{
				string _name = DisplayOptions.InputFieldPlayerName.text;

				if( string.IsNullOrEmpty( _name ) )
					_name = Connection.DefaultPlayerName;

				_name = RemoveIllegalCharacters( _name );

				string _r = "#";
				_name = _name.Replace( _r , "" );
				while( _name.Length < 5 )
					_name = _name + _r;

				DisplayOptions.InputFieldPlayerName.text = _name;

				if( InsideLobby )
					PhotonNetwork.player.NickName = _name;
			}			
		}

		public void OnGUIConnect(){

			ClientState _state = PhotonNetwork.connectionStateDetailed;
			if( _state == ClientState.PeerCreated || _state == ClientState.Disconnected )
				Connect();	
		}

		public void OnGUIDisconnect(){
			Disconnect();
		}

		public void OnGUIQuit(){
			Application.Quit();
		}


		public void OnGUIJoinRoom(){

			if( ( DisplayOptions.ToggleJoinRandomRoom != null && DisplayOptions.ToggleJoinRandomRoom.isOn ) )
				TryJoinOrCreateRandomRoom( true );
			else 
				TryJoinCertainRoom();				
		}

		/// <summary>
		/// GUI changed quality mode.
		/// </summary>
		public void OnGUIChangedQuality()
		{
			if( DisplayOptions.DropdownQuality != null )
			{
				int _quality = Mathf.Clamp( DisplayOptions.DropdownQuality.value, 0, 5 );
				QualitySettings.SetQualityLevel( _quality , false );
			}
		}

		/// <summary>
		/// GUI changed sreen mode.
		/// </summary>
		public void OnGUIChangedScreen()
		{
			if( DisplayOptions.DropdownScreen != null )
			{
				int _value = Mathf.Clamp( DisplayOptions.DropdownScreen.value, 0, 1 );

				if( _value == 0 && Screen.fullScreen )
				{
					m_DefaultScreenWidth = Mathf.Clamp( m_DefaultScreenWidth, 640, 800 );
					m_DefaultScreenHeight = Mathf.Clamp( m_DefaultScreenHeight, 480, 600 );

					Screen.SetResolution( m_DefaultScreenWidth, m_DefaultScreenHeight, false );
				}
				else if( _value == 1 && ! Screen.fullScreen )
				{
					Resolution _current_res = Screen.currentResolution;
					m_DefaultScreenWidth = _current_res.width;
					m_DefaultScreenHeight = _current_res.height;

					Resolution _resolution = new Resolution();
					foreach( Resolution _screen_res in Screen.resolutions )
					{
						if( _screen_res.width > _resolution.width )
						{
							_resolution.width = _screen_res.width;
							_resolution.height = _screen_res.height;
						}
					}

					Screen.SetResolution( _resolution.width, _resolution.height, true );
				}
			}
		}

		/// <summary>
		/// Connect this instance.
		/// </summary>
		protected virtual void Connect()
		{
			m_HasJoinedLobby = false;
			m_ReceivedRoomListUpdate = false;

			if( ! Connection.HandleConnection )
				return;

			ICEDebug.LogAction( "Connect() - This client will be connecting now to the " + ( Connection.AutoJoinLobby ? "Lobby" : "MasterServer" ) + "." );

			PhotonNetwork.automaticallySyncScene = Connection.AutomaticallySyncScene;
			PhotonNetwork.autoCleanUpPlayerObjects = Connection.AutomaticallyCleanUpPlayerObjects;
			PhotonNetwork.autoJoinLobby = Connection.AutoJoinLobby;
			PhotonNetwork.ConnectUsingSettings( Connection.Version );
			m_ClientState = ClientState.Uninitialized;
		}

		/// <summary>
		/// Reconnect this instance.
		/// </summary>
		protected virtual void Reconnect()
		{
			if( ! Connection.HandleConnection )
				return;

			if( PhotonNetwork.connectionStateDetailed != ClientState.Disconnected && PhotonNetwork.connectionStateDetailed != ClientState.PeerCreated )
				PhotonNetwork.Disconnect();

			Connect();
		}

		/// <summary>
		/// Disconnect this instance.
		/// </summary>
		public virtual void Disconnect()
		{
			if( ! Connection.HandleConnection )
				return;

			if( PhotonNetwork.connectionStateDetailed == ClientState.Disconnected || PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated )
				return;

			ICEDebug.LogAction( "Disconnect() - This client will disconnect now the connection." );

	#if ICE_UFPS_MP
			if( vp_MPConnection.Instance != null )
				vp_MPConnection.Instance.Disconnect();		
	#endif
			PhotonNetwork.Disconnect();
			m_StayConnected = false;

			//m_ConnectionAttempts = 0;
			m_ClientState = ClientState.Disconnected;

			LoadMenu();
		}

		protected void ShowLobby()
		{
			if( ! Connection.HandleConnection || DisplayOptions.LobbyPanel == null ) 
				return;

			ICEDebug.LogAction( "ShowLobby() - This client will display the lobby window." );


			if( DisplayOptions.InputFieldPlayerName != null && string.IsNullOrEmpty( DisplayOptions.InputFieldPlayerName.text ) )
				DisplayOptions.InputFieldPlayerName.text = Connection.DefaultPlayerName;

			if( DisplayOptions.DropdownRooms != null )
			{
				List<string> _rooms = new List<string>();

				// add default rooms ...
				foreach( string _room in Connection.Rooms )
					_rooms.Add( _room );
					
				// add existing rooms ...
				foreach( RoomInfo _info in PhotonNetwork.GetRoomList() )
				{
					// will be added only if not already exists ...
					if( _rooms.IndexOf( _info.Name ) == -1 )
						_rooms.Add( _info.Name );
					/*
					if( _info.IsOpen && _info.PlayerCount < _info.MaxPlayers )
					{
				
					}*/
				}
					
				DisplayOptions.DropdownRooms.ClearOptions();
				DisplayOptions.DropdownRooms.AddOptions( _rooms );
			}

			DisplayOptions.LobbyPanel.gameObject.SetActive( true );
		}


		/// <summary>
		/// Called after the connection to the master is established and authenticated but only when
		/// PhotonNetwork.autoJoinLobby is false.
		/// </summary>
		/// <remarks>If you set PhotonNetwork.autoJoinLobby to true, OnJoinedLobby() will be called instead of this.
		/// 
		/// You can join rooms and create them even without being in a lobby. The default lobby is used in that case.
		/// The list of available rooms won't become available unless you join a lobby via PhotonNetwork.joinLobby.</remarks>
		public override void OnConnectedToMaster()
		{
			ICEDebug.LogInfo("OnConnectedToMaster() - This client is connected now with the master " + ( Connection.JoinRandomRoom ? "and will join a random room" : "" ) + ".", UseDebugLogs );
			if( ! Connection.HandleConnection )
				return;

			ICEDebug.LogAction( "TryJoinRandomRoom() - This client will try now to join a random room directly while connected to master." );
			PhotonNetwork.JoinRandomRoom();
		}

		/// <summary>
		/// Called on entering a lobby on the Master Server. The actual room-list updates will call OnReceivedRoomListUpdate().
		/// </summary>
		/// <remarks>Note: When PhotonNetwork.autoJoinLobby is false, OnConnectedToMaster() will be called and the room list won't
		/// become available.
		/// 
		/// While in the lobby, the roomlist is automatically updated in fixed intervals (which you can't modify).
		/// The room list gets available when OnReceivedRoomListUpdate() gets called after OnJoinedLobby().</remarks>
		public override void OnJoinedLobby()
		{
			PhotonNetwork.player.NickName = Connection.DefaultPlayerName;

			m_HasJoinedLobby = true;
			ICEDebug.LogInfo("OnJoinedLobby() - This client has joined the lobby and will waiting now to receive the room list update." , UseDebugLogs );
			if( ! Connection.HandleConnection )
				return;

		}
			
		/// <summary>
		/// Called for any update of the room-listing while in a lobby (PhotonNetwork.insideLobby) on the Master Server.
		/// </summary>
		public override void OnReceivedRoomListUpdate()
		{
			m_ReceivedRoomListUpdate = true;
			ICEDebug.LogInfo( "OnReceivedRoomListUpdate() - This client has received the updated room list from the joined lobby.", UseDebugLogs );
			if( ! Connection.HandleConnection )
				return;

			if( Connection.JoinRandomRoom )
				TryJoinOrCreateRandomRoom( false );		
			else
				ShowLobby();
		}
		/*
		protected virtual bool TryJoinLobby()
		{			
			if( ! Connection.HandleConnection )
				return false;

			DisplayOptions.SetColorByNetworkStatus( BasicNetworkStatus.Connecting );
			ICEDebug.LogAction( "TryJoinLobby() - This client will be trying now to join the specified lobby." );
			return PhotonNetwork.JoinLobby();
		}*/
			
		/// <summary>
		/// Tries the join or create a random room based on the DefaultRoomName 
		/// </summary>
		/// <returns><c>true</c>, if join or create random room was tryed, <c>false</c> otherwise.</returns>
		/// <param name="_forced">If set to <c>true</c> forced.</param>
		protected virtual bool TryJoinOrCreateRandomRoom( bool _forced )
		{
			if( ! Connection.HandleConnection || ( ! Connection.JoinRandomRoom && ! _forced ) || ! InsideLobby )
				return false;

			if( ( PhotonNetwork.countOfPlayersInRooms % Connection.MaxPlayersPerRoom ) == 0 )
			{
				string _room_name = Connection.DefaultRoomName + ( PhotonNetwork.countOfRooms + 1 ).ToString();

				foreach( RoomInfo _info in PhotonNetwork.GetRoomList() )
				{
					if( _info.Name == _room_name )
					{
						ICEDebug.LogAction( "TryJoinOrCreateRandomRoom() - This client will try now to join room '" + _info.Name + "'." );
						return PhotonNetwork.JoinRoom( _info.Name );
					}
				}

				ICEDebug.LogAction( "TryJoinOrCreateRandomRoom() - This client will try now to create room '" + _room_name + "'." );
				return PhotonNetwork.CreateRoom( _room_name );
			}
			else
			{
				string _room_name = Connection.DefaultRoomName + ( PhotonNetwork.countOfRooms ).ToString();
				ICEDebug.LogAction( "TryJoinOrCreateRandomRoom() - This client will try now to join room '" + _room_name + "'." );
				return PhotonNetwork.JoinRoom( _room_name );
			}
		}

		/// <summary>
		/// Tries the join certain room according to the specified room list 
		/// </summary>
		/// <returns><c>true</c>, if join certain room was tryed, <c>false</c> otherwise.</returns>
		protected virtual bool TryJoinCertainRoom()
		{
			if( ! Connection.HandleConnection || Connection.JoinRandomRoom || ! InsideLobby )
				return false;

			string _desired_room_name = DisplayOptions.SelectedRoomName;

			foreach( RoomInfo _info in PhotonNetwork.GetRoomList() )
			{
				if( _info.Name == _desired_room_name )
				{
					if( _info.IsOpen && _info.PlayerCount < _info.MaxPlayers )
					{
						ICEDebug.LogAction( "TryJoinCertainRoom() - This client will try now to join room '" + _info.Name  + "'." );
						return PhotonNetwork.JoinRoom( _info.Name );
					}
					else
					{
						DisplayOptions.Log( "Joining " + _desired_room_name + " is not available!" );
						return false;
					}
				}
			}

			DisplayOptions.Log( _desired_room_name + " could not be found and will be created now!" );
			return PhotonNetwork.CreateRoom( _desired_room_name );;
		}

		/// <summary>
		/// Raises the photon random join failed event.
		/// </summary>
		void OnPhotonRandomJoinFailed()
		{
			ICEDebug.LogInfo("OnPhotonRandomJoinFailed() - This client failed to join a random room and will create an own random room now.", UseDebugLogs );
			PhotonNetwork.CreateRoom( null, new RoomOptions() { MaxPlayers = (byte)Connection.MaxPlayersPerRoom }, null);
		}

		public override void OnFailedToConnectToPhoton(DisconnectCause cause)
		{
			DisplayOptions.SetColorByNetworkStatus( BasicNetworkStatus.Error );
			ICEDebug.LogError( "OnFailedToConnectToPhoton() - The connection to Photon failed : " + cause );
		}

		public bool UseUFPSMP_vp_MPMaster = false;

		public override void OnJoinedRoom()
		{
			ICEDebug.LogInfo( "OnJoinedRoom() - This client is in a room. From here on, your game would be running." , UseDebugLogs );
			if( ! Connection.HandleConnection )
				return;
			
			if( PhotonNetwork.isMasterClient )
				PhotonNetwork.room.MaxPlayers = Connection.MaxPlayersPerRoom;

			#if ICE_UFPS_MP
			if( UseUFPSMP_vp_MPMaster )
			{
				if( ! PhotonNetwork.isMasterClient )
					PhotonNetwork.LoadLevel( vp_MPMaster.Instance.CurrentLevel );

				// send spawn request to master client
				string name = "Unnamed";

				// sent as RPC instead of in 'OnPhotonPlayerConnected' because the
				// MasterClient does not run the latter for itself + we don't want
				// to do the request on all clients

				if( FindObjectOfType<vp_MPMaster>() )	// in rare cases there might not be a vp_MPMaster, for example: a chat lobby
					photonView.RPC( "RequestInitialSpawnInfo", PhotonTargets.MasterClient, PhotonNetwork.player, 0, name );

				vp_Gameplay.IsMaster = PhotonNetwork.isMasterClient;

				return;
			}
			#endif
			
			LoadScene();


		}



		void OnPhotonCreateRoomFailed()
		{
			UpdateDisplay( BasicNetworkStatus.Error );
			ICEDebug.LogInfo("OnPhotonCreateRoomFailed() got called. This can happen if the room exists (even if not visible). Try another room name.", UseDebugLogs );
		}

		public override void OnPhotonJoinRoomFailed(object[] cause)
		{
			UpdateDisplay( BasicNetworkStatus.Error );
			ICEDebug.LogInfo( "OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.", UseDebugLogs );
		}

		public override void OnCreatedRoom()
		{
			ICEDebug.LogInfo( "OnCreatedRoom()", UseDebugLogs );
			LoadScene();
		}

		public override void OnDisconnectedFromPhoton()
		{
			ICEDebug.LogInfo( "OnDisconnectedFromPhoton() ... try to load the menu scene!", UseDebugLogs );
			LoadMenu();
		}

		void OnFailedToConnectToPhoton()
		{
			ICEDebug.LogInfo( "OnFailedToConnectToPhoton() ... try to load the menu scene!", UseDebugLogs );
			LoadMenu();
		}

		public override void OnPhotonPlayerConnected( PhotonPlayer _player )
		{
			ICEDebug.LogInfo( "OnPhotonPlayerConnected() - " + _player.NickName + " join the game!", UseDebugLogs );
		}

		public override void OnPhotonPlayerDisconnected( PhotonPlayer _player )
		{
			ICEDebug.LogInfo( "OnPhotonPlayerDisconnected() - " + _player.NickName + " left the game!", UseDebugLogs );

			//ICEWorldInfo.IsMasterClient = PhotonNetwork.isMasterClient;
		}

		/// <summary>
		/// Loads the menu.
		/// </summary>
		private void LoadMenu()
		{
			if( ! SceneManagement.UseSceneManagement || string.IsNullOrEmpty( SceneManagement.SceneNameMenu ) )
				return;
			
			ICEDebug.LogInfo("LoadMenu() ... loading menu scene '" + SceneManagement.SceneNameMenu + "'!", UseDebugLogs );
			SceneManager.LoadScene( SceneManagement.SceneNameMenu );
		}

		/// <summary>
		/// Loads the scene.
		/// </summary>
		private void LoadScene()
		{
			if( ! SceneManagement.UseSceneManagement || string.IsNullOrEmpty( SceneManagement.SceneNameGame ) )
				return;

			if ( ! PhotonNetwork.isMasterClient ) 
				ICEDebug.LogWarning( "LoadScene() : This Client is trying to Load the '" + SceneManagement.SceneNameGame + "' level but we are not the master client." );
		

			ICEDebug.LogAction("LoadScene() : This Client is loading now the game scene '" + SceneManagement.SceneNameGame + "'!", UseDebugLogs );

			if( ChildCamera != null )
				ChildCamera.enabled = false;

			if( ICEWorldInfo.IsMultiplayer )
				PhotonNetwork.LoadLevel( SceneManagement.SceneNameGame );
			else
				SceneManager.LoadScene( SceneManagement.SceneNameGame );
		}

	#if UNITY_5_4_OR_NEWER
		protected void OnLevelLoad(Scene scene, LoadSceneMode mode)
	#else
	protected void OnLevelWasLoaded()
	#endif
		{
		}






		public void LeaveRoom(){
			if( PhotonNetwork.connectionStateDetailed == ClientState.Joined )
				PhotonNetwork.LeaveRoom();
		}

		private string RemoveIllegalCharacters( string _text )
		{
			if( ! Connection.RemoveIllegalCharacters )
				return _text;

			return SystemTools.RemoveIllegalCharacters( _text, ( Connection.UseRegularExpressions ? Connection.IllegalCharactersRegex : Connection.IllegalCharactersSimple ), Connection.UseRegularExpressions );
		}

		// CHAT SYSTEM

			
	}
#else
	public class ICEWorldNetworkManager : MonoBehaviour{

		protected static ICEWorldNetworkManager m_Instance = null;
		public static ICEWorldNetworkManager Instance{
			get{ return m_Instance = ( m_Instance == null?GameObject.FindObjectOfType<ICEWorldNetworkManager>():m_Instance ); }
		}
	}
#endif
}