using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;

using ICE;
using ICE.World;
using ICE.World.Utilities;
using ICE.World.EditorUtilities;
using ICE.World.EnumTypes;

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
	[CustomEditor(typeof(ICEPhotonNetworkManager))]
	public class ICEPhotonNetworkManagerEditor : ICEWorldBehaviourEditor 
	{
		public void DrawPhotonAdapterContent( ICEWorldNetworkManager _adapter )
		{
			//ICECreatureEntity m_Entity = _adapter.GetComponent<ICECreatureEntity>(); 

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


		private void DrawConnectionManagement( ICEWorldNetworkManager _control, NetworkManagerConnectionManagementObject _options, EditorHeaderType _type, string _help = "", string _title = "", string _hint = "" )
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

		private void DrawSceneManagementObject( ICEWorldNetworkManager _control, NetworkManagerSceneManagementObject _options, EditorHeaderType _type, string _help = "", string _title = "", string _hint = "" )
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

		private void DrawNetworkManagerDisplayOptionsObject( ICEWorldNetworkManager _control, NetworkManagerDisplayOptionsObject _options, EditorHeaderType _type, string _help = "", string _title = "", string _hint = "" )
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
			
			ICEEditorLayout.BeginHorizontal();
				_options.FoldoutStatusPanel = ICEEditorLayout.Foldout( _options.FoldoutStatusPanel, "Status", false );
				//if( ICEEditorLayout.Button( "CREATE", "", ICEEditorStyle.ButtonMiddle  ) )
				//	CreateStatusPanel( _control, _options );			
			ICEEditorLayout.EndHorizontal();


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

		
			ICEEditorLayout.BeginHorizontal();
				_options.FoldoutConnectPanel = ICEEditorLayout.Foldout( _options.FoldoutConnectPanel, "Connect",  false );
				//if( ICEEditorLayout.Button( "CREATE", "", ICEEditorStyle.ButtonMiddle  ) )
				//	CreateConnectionPanel( _control, _options );			
			ICEEditorLayout.EndHorizontal();

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
	

		#region UI Elements
		private void CreateStatusPanel( ICEWorldNetworkManager _control, NetworkManagerDisplayOptionsObject _options )
		{
			Canvas _canvas = SystemTools.FindChildOfTypeByName<Canvas>( "NetworkManagerUI", _control.transform );
			if( _canvas == null )
				_canvas = InterfaceTools.AddCanvas( "NetworkManagerUI", _control.transform, Vector2.one , AnchorPresets.StretchAll, PivotPresets.MiddleCenter, new Rect(0,0,0,0), RenderMode.ScreenSpaceOverlay );

			if( _canvas == null )
				return;

			Image _status_panel = SystemTools.FindChildOfTypeByName<Image>( "StatusPanel", _canvas.transform );
			if( _status_panel == null || ! _status_panel.transform.IsChildOf( _canvas.transform ) )
				_status_panel = InterfaceTools.AddImage( "StatusPanel", _canvas.transform, new Vector2( 0, 32 ), AnchorPresets.HorStretchTop, PivotPresets.TopCenter, new Rect( 10,-42,-10,-10 ), Color.clear );

			if( _status_panel == null )
				return;
			
			Image _status_image = SystemTools.FindChildOfTypeByName<Image>( "StatusImage", _status_panel.transform );
			if( _status_image == null || ! _status_image.transform.IsChildOf( _status_panel.transform ) )
				_status_image = InterfaceTools.AddImage( "StatusImage", _status_panel.transform, new Vector2( 32, 32 ), AnchorPresets.TopLeft, PivotPresets.TopLeft, new Rect( 0,0,0,0 ), Color.clear );

			Text _status_text = SystemTools.FindChildOfTypeByName<Text>( "StatusText", _status_panel.transform );
			if( _status_text == null || ! _status_text.transform.IsChildOf( _status_panel.transform ) )
				 _status_text = InterfaceTools.AddText( "StatusText", _status_panel.transform, new Vector2( 10, 10 ), AnchorPresets.TopLeft, PivotPresets.TopLeft, new Rect( 0,0,0,0 ), Color.white, "" );

			Button _status_button = SystemTools.FindChildOfTypeByName<Button>( "ButtonDisconnect", _status_panel.transform );
			if( _status_button == null || ! _status_button.transform.IsChildOf( _status_panel.transform ) )
			 	_status_button = InterfaceTools.AddButton( "ButtonDisconnect", _status_panel.transform, new Vector2( 32, 32 ), AnchorPresets.TopRight, PivotPresets.TopRight, new Rect( 0,0,0,0 ), Color.clear );
		
			/*
			_options.StatusPanel = _status_panel.rectTransform;
			_options.ImageStatus = _status_image;
			_options.ImageMaster = _status_image;
			_options.TextStatus = _status_text;
			_options.ButtonDisconnect = _status_button;
			*/

	



		}

		private void CreateLobbyPanel( ICEWorldNetworkManager _control, NetworkManagerDisplayOptionsObject _options )
		{
			Canvas _canvas = SystemTools.FindChildOfTypeByName<Canvas>( "NetworkManagerUI", _control.transform );
			if( _canvas == null )
				_canvas = InterfaceTools.AddCanvas( "NetworkManagerUI", _control.transform, Vector2.one , AnchorPresets.StretchAll, PivotPresets.MiddleCenter, Rect.zero, RenderMode.ScreenSpaceOverlay );

			if( _canvas == null )
				return;

			Image _panel = SystemTools.FindChildOfTypeByName<Image>( "LobbyPanel", _canvas.transform );
			if( _panel == null || ! _panel.transform.IsChildOf( _canvas.transform ) )
				_panel = InterfaceTools.AddImage( "LobbyPanel", _canvas.transform, new Vector2( 300, 250 ), AnchorPresets.MiddleCenter, PivotPresets.MiddleCenter, Rect.zero, Color.white );

			if( _panel == null )
				return;

			Image _title = SystemTools.FindChildOfTypeByName<Image>( "Title", _panel.transform );
			if( _title == null || ! _title.transform.IsChildOf( _panel.transform ) )
				_title = InterfaceTools.AddImage( "Title", _panel.transform, new Vector2( 0, 30 ), AnchorPresets.HorStretchTop, PivotPresets.TopCenter, Rect.zero, Color.clear );

				Text _title_text = SystemTools.FindChildOfTypeByName<Text>( "Text", _title.transform );
				if( _title_text == null || ! _title_text.transform.IsChildOf( _title.transform ) )
					_title_text = InterfaceTools.AddText( "Text", _title.transform, new Vector2( 0, 0 ), AnchorPresets.StretchAll, PivotPresets.MiddleCenter, Rect.zero, Color.grey, "Lobby" );

			Dropdown _rooms = InterfaceTools.AddDropdown( "Text", _title.transform, new Vector2( 0, 0 ), AnchorPresets.StretchAll, PivotPresets.MiddleCenter, Rect.zero, Color.white, "" );


		}

		private void CreateConnectionPanel( ICEWorldNetworkManager _control, NetworkManagerDisplayOptionsObject _options )
		{
			Canvas _canvas = SystemTools.FindChildOfTypeByName<Canvas>( "NetworkManagerUI", _control.transform );
			if( _canvas == null )
				_canvas = InterfaceTools.AddCanvas( "NetworkManagerUI", _control.transform, Vector2.one , AnchorPresets.StretchAll, PivotPresets.MiddleCenter, Rect.zero, RenderMode.ScreenSpaceOverlay );

			if( _canvas == null )
				return;

			Image _panel = SystemTools.FindChildOfTypeByName<Image>( "ConnectionPanel", _canvas.transform );
			if( _panel == null || ! _panel.transform.IsChildOf( _canvas.transform ) )
				_panel = InterfaceTools.AddImage( "ConnectionPanel", _canvas.transform, new Vector2( 300, 115 ), AnchorPresets.MiddleCenter, PivotPresets.MiddleCenter, Rect.zero, Color.white );

			if( _panel == null )
				return;

			Image _title = SystemTools.FindChildOfTypeByName<Image>( "Title", _panel.transform );
			if( _title == null || ! _title.transform.IsChildOf( _panel.transform ) )
				_title = InterfaceTools.AddImage( "Title", _panel.transform, new Vector2( 0, 30 ), AnchorPresets.HorStretchTop, PivotPresets.TopCenter, Rect.zero, Color.clear );

				Text _title_text = SystemTools.FindChildOfTypeByName<Text>( "Text", _title.transform );
				if( _title_text == null || ! _title_text.transform.IsChildOf( _title.transform ) )
					_title_text = InterfaceTools.AddText( "Text", _title.transform, new Vector2( 0, 0 ), AnchorPresets.StretchAll, PivotPresets.MiddleCenter, Rect.zero, Color.grey, "Welcome" );

			Toggle _toggle = SystemTools.FindChildOfTypeByName<Toggle>( "ToggleRandomConnect", _panel.transform );
			if( _toggle == null || ! _toggle.transform.IsChildOf( _panel.transform ) )
				_toggle = InterfaceTools.AddToggle( "ToggleRandomConnect", _panel.transform, new Vector2( 0, 30 ), AnchorPresets.HorStretchTop, PivotPresets.TopCenter, new Rect( 0,-70,0,-40 ), Color.clear, "Random Connection" );

			Button _connect_button = SystemTools.FindChildOfTypeByName<Button>( "ButtonConnect", _panel.transform );
			if( _connect_button == null || ! _connect_button.transform.IsChildOf( _panel.transform ) )
				_connect_button = InterfaceTools.AddButton( "ButtonConnect", _panel.transform, new Vector2( 0, 32 ), AnchorPresets.HorStretchBottom, PivotPresets.BottomCenter, new Rect( 0,0,0,0 ), Color.clear );

				Text _connect_button_text = SystemTools.FindChildOfTypeByName<Text>( "Text", _connect_button.transform );
				if( _connect_button_text == null || ! _connect_button_text.transform.IsChildOf( _connect_button.transform ) )
					_connect_button_text = InterfaceTools.AddText( "Text", _connect_button.transform, new Vector2( 0, 0 ), AnchorPresets.StretchAll, PivotPresets.MiddleCenter, Rect.zero, Color.grey, "Connect" );
			
			/*
			_options.StatusPanel = _status_panel.rectTransform;
			_options.ImageStatus = _status_image;
			_options.ImageMaster = _status_image;
			_options.TextStatus = _status_text;
			_options.ButtonDisconnect = _status_button;
			*/



		}
		#endregion
	}
	#endif
}
