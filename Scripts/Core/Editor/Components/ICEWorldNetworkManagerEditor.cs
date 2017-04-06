// ##############################################################################
//
// ICEWorldNetworkManagerEditor.cs
// Version 1.4.0
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
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;

using ICE;
using ICE.World;
using ICE.World.EditorUtilities;

using ICE.Integration;
using ICE.Integration.Objects;
using ICE.Integration.EditorInfos;

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
	[CustomEditor(typeof(ICEWorldNetworkManager))]
	public class ICEWorldNetworkManagerEditor : ICEWorldBehaviourEditor 
	{

		public override void OnInspectorGUI()
		{
			ICEWorldNetworkManager _target = DrawMonoHeader<ICEWorldNetworkManager>();
			DrawAdapterContent( _target );
			DrawMonoFooter( _target );
		}

		public void DrawAdapterContent( ICEWorldNetworkManager _adapter )
		{
			ICECreatureEntity m_Entity = _adapter.GetComponent<ICECreatureEntity>(); 

			PhotonView m_PhotonView = _adapter.GetComponent<PhotonView>(); 
			EditorGUI.BeginDisabledGroup( m_PhotonView != null );
				GUI.backgroundColor = ( m_PhotonView == null ? Color.yellow : Color.green );			
				if( ICEEditorLayout.Button( "Photon View", "", ICEEditorStyle.ButtonExtraLarge ) )
					m_PhotonView = _adapter.gameObject.AddComponent<PhotonView>();
				GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.Separator();

			EditorGUI.indentLevel++;
				DrawConnectionManagement( _adapter, _adapter.Connection, EditorHeaderType.FOLDOUT_ENABLED_BOLD );
				DrawSceneManagementObject( _adapter, _adapter.SceneManagement, EditorHeaderType.FOLDOUT_ENABLED_BOLD );
				DrawNetworkManagerDisplayOptionsObject( _adapter, _adapter.DisplayOptions, EditorHeaderType.FOLDOUT_BOLD );

				EditorGUILayout.Separator();
					_adapter.UseDebugLogs = ICEEditorLayout.Toggle( "Debug Logs", "", _adapter.UseDebugLogs );
					EditorGUI.BeginDisabledGroup( Application.isPlaying == true );
						_adapter.UsePhotonDebugLogs = ICEEditorLayout.Toggle( "Photon Debug Logs", "", _adapter.UsePhotonDebugLogs );
					EditorGUI.EndDisabledGroup();
				EditorGUILayout.Separator();
					_adapter.UseDontDestroyOnLoad = ICEEditorLayout.Toggle( "Dont Destroy On Load", "", _adapter.UseDontDestroyOnLoad );
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();

		}


		public static void DrawConnectionManagement( ICEWorldNetworkManager _control, NetworkManagerConnectionManagementObject _options, EditorHeaderType _type, string _help = "", string _title = "", string _hint = "" )
		{
			if( _options == null )
				return;

			EditorGUI.BeginDisabledGroup( _options.Enabled == true );
			_options.UseMultiplayer = ICEEditorLayout.LabelEnableButton( "Use Multiplayer", "", ( _options.Enabled ? "multiplayer automatically enabled" : "" ), _options.UseMultiplayer );
			EditorGUI.EndDisabledGroup();

			if( ! _options.Enabled )
			{
				EditorGUILayout.HelpBox( "While the Connection Management is disabled you could handle the network connection " +
					"by using external scripts, in such a case you should flag 'Use Multiplayer' so ICE can receive the connection " +
					"state, also you could activate the DebugLogs to observe the connection behaviour.", MessageType.Info );
			}

			EditorGUILayout.Separator();

			if( string.IsNullOrEmpty( _title ) )
				_title = "Connection Management";
			if( string.IsNullOrEmpty( _hint ) )
				_hint = "";
			if( string.IsNullOrEmpty( _help ) )
				_help = Info.NETWORK_MANAGER_SCENE_OPTIONS;




			WorldObjectEditor.DrawObjectHeader( _options, _type, _title, _hint, _help );

			// CONTENT BEGIN
			if( WorldObjectEditor.BeginObjectContentOrReturn( _type, _options ) )
				return;

			EditorGUI.BeginDisabledGroup( _options.UseMultiplayer == false );
				_options.AutoConnect = ICEEditorLayout.LabelEnableButton( "Auto Connect", "", _options.AutoConnect );
				_options.AutoJoinLobby = ICEEditorLayout.LabelEnableButton( "Auto Join Lobby", "", ( ! _options.AutoJoinLobby ? "warning: no room list update" : "" ), _options.AutoJoinLobby );
			EditorGUI.EndDisabledGroup();

			// TODO : LOBBY HANDLING

			ICEEditorLayout.BeginHorizontal();
				EditorGUI.BeginDisabledGroup( _options.JoinRandomRoom == true );
				_options.DefaultRoomName = ICEEditorLayout.DrawListPopup( "Default Room Name", _options.DefaultRoomName, _options.Rooms );
				EditorGUI.EndDisabledGroup();
				_options.JoinRandomRoom = ICEEditorLayout.CheckButtonMiddle( "RANDOM", "", _options.JoinRandomRoom );
			ICEEditorLayout.EndHorizontal( "" );
			ICEEditorLayout.DrawStringList( "Room", "", _options.Rooms );
			EditorGUILayout.Separator();

			ICEEditorLayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup( _options.RemoveIllegalCharacters == false );
				if( _options.UseRegularExpressions )
					_options.IllegalCharactersRegex = ICEEditorLayout.Text( "Illegal Characters (regex)", "", _options.IllegalCharactersRegex, "" );
				else
					_options.IllegalCharactersSimple = ICEEditorLayout.Text( "Illegal Characters (simple)", "", _options.IllegalCharactersSimple, "" );

					_options.UseRegularExpressions = ICEEditorLayout.CheckButtonSmall( "REG", "", _options.UseRegularExpressions );
			EditorGUI.EndDisabledGroup();
			_options.RemoveIllegalCharacters = ICEEditorLayout.EnableButton( "Removes illegal characters from the player name", _options.RemoveIllegalCharacters );
			ICEEditorLayout.EndHorizontal( "" );
			EditorGUILayout.Separator();

			_options.DefaultPlayerName = ICEEditorLayout.Text( "Default Player Name" , "", _options.DefaultPlayerName, "" );
			_options.MaxPlayersPerRoom = ICEEditorLayout.IntField( "Max. Players Per Room", "", _options.MaxPlayersPerRoom, "" );
			EditorGUILayout.Separator();

			// TODO : SCENE HANDLING

			_options.AutomaticallySyncScene = ICEEditorLayout.Toggle( "Automatically Sync Scene", "", _options.AutomaticallySyncScene );
			_options.AutomaticallyCleanUpPlayerObjects = ICEEditorLayout.Toggle( "Automatically CleanUp Player Objects", "", _options.AutomaticallyCleanUpPlayerObjects );

			EditorGUILayout.Separator();
			_options.Version = ICEEditorLayout.Text( "Version", "", _options.Version, "" );
			EditorGUILayout.Separator();


			WorldObjectEditor.EndObjectContent();
			// CONTENT END
		}

		public static void DrawSceneManagementObject( ICEWorldNetworkManager _control, NetworkManagerSceneManagementObject _options, EditorHeaderType _type, string _help = "", string _title = "", string _hint = "" )
		{
			if( _options == null )
				return;

			if( string.IsNullOrEmpty( _title ) )
				_title = "Scene Management";
			if( string.IsNullOrEmpty( _hint ) )
				_hint = "";
			if( string.IsNullOrEmpty( _help ) )
				_help = Info.NETWORK_MANAGER_SCENE_OPTIONS;


			WorldObjectEditor.DrawObjectHeader( _options, _type, _title, _hint, _help );

			// CONTENT BEGIN
			if( WorldObjectEditor.BeginObjectContentOrReturn( _type, _options ) )
				return;

			_options.SceneNameMenu = ICEEditorLayout.Text( "Menu (SceneName)", "", _options.SceneNameMenu, "" );
			_options.SceneNameGame = ICEEditorLayout.Text( "Game (SceneName)", "", _options.SceneNameGame, "" );
			
			WorldObjectEditor.EndObjectContent();
			// CONTENT END
		}

		public static void DrawNetworkManagerDisplayOptionsObject( ICEWorldNetworkManager _control, NetworkManagerDisplayOptionsObject _options, EditorHeaderType _type, string _help = "", string _title = "", string _hint = "" )
		{
			if( _options == null )
				return;

			if( string.IsNullOrEmpty( _title ) )
				_title = "Display Options";
			if( string.IsNullOrEmpty( _hint ) )
				_hint = "";
			if( string.IsNullOrEmpty( _help ) )
				_help = Info.NETWORK_MANAGER_DISPLAY_OPTIONS;


			WorldObjectEditor.DrawObjectHeader( _options, _type, _title, _hint, _help );

			//WorldObjectEditor.DrawAudioObject();

			// CONTENT BEGIN
			if( WorldObjectEditor.BeginObjectContentOrReturn( _type, _options ) )
				return;

			_options.FoldoutStatusPanel = ICEEditorLayout.Foldout( _options.FoldoutStatusPanel, "Status", false );
			if( _options.FoldoutStatusPanel )
			{		
				EditorGUI.indentLevel++;

					GUI.backgroundColor = ( _options.StatusPanel == null ? Color.red : Color.green );
					_options.StatusPanel = (RectTransform)EditorGUILayout.ObjectField("Status Panel", _options.StatusPanel, typeof(RectTransform), true);
	
					GUI.backgroundColor = ( _options.ImageStatus == null ? Color.yellow : Color.green );
					_options.ImageStatus = (Image)EditorGUILayout.ObjectField("Status Image", _options.ImageStatus, typeof(Image), true);
	
					GUI.backgroundColor = ( _options.ImageMaster == null ? Color.yellow : Color.green );
					_options.ImageMaster = (Image)EditorGUILayout.ObjectField("Master Image", _options.ImageMaster, typeof(Image), true);

					GUI.backgroundColor = ( _options.TextStatus == null ? Color.yellow : Color.green );
					_options.TextStatus = (Text)EditorGUILayout.ObjectField("Status Text", _options.TextStatus, typeof(Text), true);
	
					GUI.backgroundColor = ( _options.ButtonDisconnect == null ? Color.yellow : Color.green );
					_options.ButtonDisconnect = (Button)EditorGUILayout.ObjectField("Disconnect Button", _options.ButtonDisconnect, typeof(Button), true);

					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
					EditorGUILayout.Separator();

					EditorGUI.BeginDisabledGroup( _options.ImageStatus == null );
					_options.UseStatusColor = ICEEditorLayout.LabelEnableButton( "Status Color", "", ( _options.ImageStatus == null ? "required 'Status Image'" : "" ), _options.UseStatusColor );
					if( _options.UseStatusColor )
					{
						EditorGUI.indentLevel++;
						_options.ColorDisconnected = ICEEditorLayout.ColorField( "Disconnected", "", _options.ColorDisconnected , "" );
						_options.ColorConnecting = ICEEditorLayout.ColorField( "Connecting", "", _options.ColorConnecting , "" );
						_options.ColorConnected = ICEEditorLayout.ColorField( "Connected", "", _options.ColorConnected , "" );
						_options.ColorLobbyJoined = ICEEditorLayout.ColorField( "Lobby", "", _options.ColorLobbyJoined , "" );
						_options.ColorRoomJoined = ICEEditorLayout.ColorField( "Room", "", _options.ColorRoomJoined , "" );
						_options.ColorWarning = ICEEditorLayout.ColorField( "Warning", "", _options.ColorWarning , "" );
						_options.ColorError = ICEEditorLayout.ColorField( "Error", "", _options.ColorError , "" );
						EditorGUI.indentLevel--;

					}
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.Separator();

					EditorGUI.BeginDisabledGroup( _options.TextStatus == null );
					_options.UseStatusLog = ICEEditorLayout.LabelEnableButton( "Status Log", "", ( _options.TextStatus == null ? "required 'Status Text'" : "" ), _options.UseStatusLog );
					if( _options.UseStatusColor )
					{
						EditorGUI.indentLevel++;
						_options.UseStatusLogTime = ICEEditorLayout.Toggle( "Time", "", _options.UseStatusLogTime );
						_options.StatusLogMaxLines = ICEEditorLayout.IntField( "Max. Lines", "", _options.StatusLogMaxLines );
						EditorGUI.indentLevel--;
					}
					EditorGUI.EndDisabledGroup();
				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();
			}


			_options.FoldoutLobbyPanel = ICEEditorLayout.Foldout( _options.FoldoutLobbyPanel, "Lobby", false );
			if( _options.FoldoutLobbyPanel )
			{
				EditorGUI.indentLevel++;
					GUI.backgroundColor = ( _options.LobbyPanel == null ? Color.red : Color.green );
					_options.LobbyPanel = (RectTransform)EditorGUILayout.ObjectField("Lobby Panel", _options.LobbyPanel, typeof(RectTransform), true);

					GUI.backgroundColor = ( _options.InputFieldPlayerName == null ? Color.yellow : Color.green );
					_options.InputFieldPlayerName = (InputField)EditorGUILayout.ObjectField("Input Field", _options.InputFieldPlayerName, typeof(InputField), true);
					
					GUI.backgroundColor = ( _options.DropdownRooms == null ? Color.yellow : Color.green );		
					_options.DropdownRooms = (Dropdown)EditorGUILayout.ObjectField("Rooms Dropdown", _options.DropdownRooms, typeof(Dropdown), true);

					GUI.backgroundColor = ( _options.ToggleJoinRandomRoom == null ? Color.yellow : Color.green );
					_options.ToggleJoinRandomRoom = (Toggle)EditorGUILayout.ObjectField("Random Room Toggle", _options.ToggleJoinRandomRoom, typeof(Toggle), true);

					GUI.backgroundColor = ( _options.DropdownQuality == null ? Color.yellow : Color.green );
					_options.DropdownQuality = (Dropdown)EditorGUILayout.ObjectField("Quality Dropdown", _options.DropdownQuality, typeof(Dropdown), true);
					
					GUI.backgroundColor = ( _options.DropdownScreen == null ? Color.yellow : Color.green );
					_options.DropdownScreen = (Dropdown)EditorGUILayout.ObjectField("Screen Dropdown", _options.DropdownScreen, typeof(Dropdown), true);

					GUI.backgroundColor = ( _options.ButtonJoin == null ? Color.yellow : Color.green );
					_options.ButtonJoin = (Button)EditorGUILayout.ObjectField("Join Button", _options.ButtonJoin, typeof(Button), true);

					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
					EditorGUILayout.Separator();
				EditorGUI.indentLevel--;
			}

			_options.FoldoutConnectPanel = ICEEditorLayout.Foldout( _options.FoldoutConnectPanel, "Connect",  false );
			if( _options.FoldoutConnectPanel )
			{
				EditorGUI.indentLevel++;
					GUI.backgroundColor = ( _options.ConnectionPanel == null ? Color.red : Color.green );
					_options.ConnectionPanel = (RectTransform)EditorGUILayout.ObjectField("Connect Panel", _options.ConnectionPanel, typeof(RectTransform), true);

					GUI.backgroundColor = ( _options.ToggleRandomConnect == null ? Color.yellow : Color.green );
					_options.ToggleRandomConnect = (Toggle)EditorGUILayout.ObjectField("Random Connect Toggle", _options.ToggleRandomConnect, typeof(Toggle), true);

					GUI.backgroundColor = ( _options.ButtonConnect == null ? Color.yellow : Color.green );
					_options.ButtonConnect = (Button)EditorGUILayout.ObjectField("Connect Button", _options.ButtonConnect, typeof(Button), true);

					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
				EditorGUILayout.Separator();
				EditorGUI.indentLevel--;
			}
			

		
			WorldObjectEditor.EndObjectContent();
			// CONTENT END
		}
	}
#else
	[CustomEditor(typeof(ICEWorldNetworkManager))]
	public class ICEWorldNetworkManagerEditor : ICEWorldBehaviourEditor 
	{
		public override void OnInspectorGUI()
		{
			ICEWorldNetworkManager _target = DrawMonoHeader<ICEWorldNetworkManager>();
			DrawAdapterContent( _target );
			DrawMonoFooter( _target );
		}

		public void DrawAdapterContent( ICEWorldNetworkManager _adapter )
		{
			string _info = "This Adapter requires the Photon Unity Network and ICECreatureControl packages. " +
			"If both assets are correct installed please add 'ICE_PUN' and 'ICE_CC' to your custom defines.";
			EditorGUILayout.HelpBox( _info, MessageType.Info );
		}
	}
#endif
}
