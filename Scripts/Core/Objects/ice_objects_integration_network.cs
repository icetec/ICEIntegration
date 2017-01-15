// ##############################################################################
//
// ICE.World.Objects.ice_objects_integration_network.cs 
// Version 1.3.7
//
// Copyrights © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.icecreaturecontrol.com
// mailto:support@icecreaturecontrol.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is furnished 
// to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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
using System.Text;
using System.IO;

using ICE;
using ICE.World;
using ICE.World.Objects;
using ICE.World.Utilities;
using ICE.World.EnumTypes;

namespace ICE.Integration.Objects
{
	public enum BasicNetworkStatus
	{
		Undefined,
		Disconnected,
		Connecting,
		Connected,
		LobbyJoined,
		RoomJoined,
		Warning,
		Error
	}

	[System.Serializable]
	public class NetworkManagerDisplayOptionsObject : ICEDataObject{

		public NetworkManagerDisplayOptionsObject(){}
		public NetworkManagerDisplayOptionsObject( NetworkManagerDisplayOptionsObject _object ) : base( _object ) { Copy( _object ); }

		public void Copy( NetworkManagerDisplayOptionsObject _object )
		{
			if( _object == null )
				return;

			Enabled = _object.Enabled;
			Foldout = _object.Foldout;
		}

		public string PlayerName{
			get{ return ( InputFieldPlayerName != null ? InputFieldPlayerName.text : "" ); } }
		public InputField InputFieldPlayerName = null;

		public int SelectedRoomIndex{
			get{ return ( DropdownRooms != null ? DropdownRooms.value : 0 ); } }

		public string SelectedRoomName{
			get{ return ( DropdownRooms != null && DropdownRooms.captionText != null ? DropdownRooms.captionText.text : "" ); } }

		public int SelectedQualityIndex{
			get{ return ( DropdownQuality != null ? DropdownQuality.value : 0 ); } }

		public int SelectedScreenIndex{
			get{ return ( DropdownScreen != null ? DropdownScreen.value : 0 ); } }

		[SerializeField]
		private Dropdown m_DropdownRooms = null;
		public Dropdown DropdownRooms{
			get{ return m_DropdownRooms; }
			set{ m_DropdownRooms = value; }
		}
		/*
		public List<string> Rooms{
			get{ return ( ICEWorldNetworkManager.Instance != null ? ICEWorldNetworkManager.Instance.Connection.Rooms : new List<string>() ); }
		}*/

		private Dropdown ValidateRooms()
		{/*
			if( m_DropdownRooms != null && m_DropdownRooms.options.Count != Rooms.Count && ! Application.isPlaying )
			{
				m_DropdownRooms.ClearOptions(); 
				m_DropdownRooms.AddOptions( Rooms );
			}*/

			return m_DropdownRooms;
		}




		[SerializeField]
		private Dropdown m_DropdownQuality = null;
		public Dropdown DropdownQuality{
			get{ return ValidateQuality(); }
			set{ m_DropdownQuality = value; ValidateQuality(); }
		}

		public string[] QualityOptions = new string[]{ "Fastest", "Fast", "Simple", "Good", "Beautiful", "Fantastic" };
		private Dropdown ValidateQuality()
		{
			List<string> _options = new List<string>( QualityOptions );
			if( m_DropdownQuality != null && m_DropdownQuality.options != null && m_DropdownQuality.options.Count != _options.Count )
			{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
				m_DropdownQuality.ClearOptions(); 
				m_DropdownQuality.AddOptions( _options );
#else
				
				List<Dropdown.OptionData> _dd_options = new List<Dropdown.OptionData>();

				foreach( string _text in _options )
					_dd_options.Add( new Dropdown.OptionData( _text, null ) );

				m_DropdownQuality.options = _dd_options;
#endif
			}

			return m_DropdownQuality;
		}



		[SerializeField]
		private Dropdown m_DropdownScreen = null;
		public Dropdown DropdownScreen{
			get{ return ValidateScreen(); }
			set{ m_DropdownScreen = value; ValidateScreen(); }
		}

		public string[] ScreenOptions = new string[]{ "Window", "Fullscreen" };
		private Dropdown ValidateScreen()
		{
			List<string> _options = new List<string>( ScreenOptions );
			if( m_DropdownScreen != null && m_DropdownScreen.options != null && m_DropdownScreen.options.Count != _options.Count )
			{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
				m_DropdownScreen.ClearOptions(); 
				m_DropdownScreen.AddOptions( _options );
#else
				List<Dropdown.OptionData> _new_options = new List<Dropdown.OptionData>();				
				foreach( string _text in _options )
					_new_options.Add( new Dropdown.OptionData( _text, null ) );				
				m_DropdownScreen.options = _new_options;
#endif
			}

			return m_DropdownScreen;
		}

		protected string m_DefaultStartButtonText = "";
		protected string m_StartButtonText = "Log On";

		protected string m_FullScreenButtonText = "Fullscreen";

		public bool FoldoutLobbyPanel = true;
		public bool FoldoutStatusPanel = true;
		public bool FoldoutConnectPanel = true;

		public RectTransform LobbyPanel = null;
		public RectTransform StatusPanel = null;
		public RectTransform ConnectionPanel = null;

		public Toggle ToggleJoinRandomRoom = null;
		public Toggle ToggleRandomConnect = null;

		public Button ButtonConnect = null;
		public Button ButtonDisconnect = null;
		public Button ButtonJoin = null;
		public Button ButtonQuit = null;

		public Text TextStatus = null;

		public Image ImageStatus = null;
		public Image ImageMaster = null;

		public bool UseStatusColor = true;

		public Color ColorDisconnected = Color.black;
		public Color ColorConnecting = Color.gray;
		public Color ColorConnected = Color.white;
		public Color ColorLobbyJoined = Color.blue;
		public Color ColorRoomJoined = Color.green;
		public Color ColorWarning = Color.yellow;
		public Color ColorError = Color.red;

		public bool UseStatusLog = true;
		public int StatusLogMaxLines = 10;
		public bool UseStatusLogTime = true;
		public bool UseDetailedLog = false;

		private List<string> m_StatusLog = new List<string>();

		public void Log( string _msg )
		{
			if( ! Enabled || ! UseStatusLog || TextStatus == null )
				return;

			if( m_StatusLog.Count >= StatusLogMaxLines )
				m_StatusLog.RemoveAt(0);

			m_StatusLog.Add( _msg );


			TextStatus.text = "";
			foreach( string _line in m_StatusLog )
				TextStatus.text += "\n " + ( UseStatusLogTime ? System.DateTime.Now.ToShortTimeString() : "" ) + " - " + _line;
		}

		public void SetColorByNetworkStatus( BasicNetworkStatus _state )
		{
			switch( _state )
			{
			case BasicNetworkStatus.Disconnected:
				m_TargetStatusColor = ColorDisconnected;
				break;
			case BasicNetworkStatus.Connecting:
				m_TargetStatusColor = ColorConnecting;
				break;
			case BasicNetworkStatus.Connected:
				m_TargetStatusColor = ColorConnected;
				break;
			case BasicNetworkStatus.LobbyJoined:
				m_TargetStatusColor = ColorLobbyJoined;
				break;
			case BasicNetworkStatus.Warning:
				m_TargetStatusColor = ColorWarning;
				break;
			case BasicNetworkStatus.Error:
				m_TargetStatusColor = ColorError;
				break;
			}
		}

		private Color m_TargetStatusColor;
		public void UpdateStatusColor()
		{
			if( ! Enabled || ! UseStatusColor || ImageMaster == null )
				return;

			Color _color = Color.Lerp( ImageMaster.color, m_TargetStatusColor, 0.1f );
			_color.a = ( ICEWorldInfo.IsMasterClient ? 1 : 0.25f );
			ImageMaster.color = _color;
		}
	}

	[System.Serializable]
	public class NetworkManagerSceneManagementObject : ICEDataObject{

		public NetworkManagerSceneManagementObject(){}
		public NetworkManagerSceneManagementObject( NetworkManagerSceneManagementObject _object ) : base( _object ) { Copy( _object ); }

		public void Copy( NetworkManagerSceneManagementObject _object )
		{
			if( _object == null )
				return;

			Enabled = _object.Enabled;
			Foldout = _object.Foldout;
		}

		public bool UseSceneManagement{
			get{ return Enabled; }			
		}

		public string SceneNameMenu = "ICEPhotonDemoMenu";
		public string SceneNameGame = "ICEPhotonDemoGame";
	}

	[System.Serializable]
	public class NetworkManagerConnectionManagementObject : ICEDataObject{

		public NetworkManagerConnectionManagementObject(){}
		public NetworkManagerConnectionManagementObject( NetworkManagerConnectionManagementObject _object ) : base( _object ) { Copy( _object ); }

		public void Copy( NetworkManagerConnectionManagementObject _object )
		{
			if( _object == null )
				return;

			Enabled = _object.Enabled;
			Foldout = _object.Foldout;
		}

		public bool HandleConnection{
			get{ return Enabled; }			
		}

		[SerializeField]
		private bool m_UseMultiplayer = true;
		public bool UseMultiplayer{
			get{ return ( Enabled ? true : m_UseMultiplayer ); }
			set{ m_UseMultiplayer = value; }				
		}

		[SerializeField]
		private bool m_AutoConnect = true;
		public bool AutoConnect{
			get{ return ( Enabled ? m_AutoConnect : false ); }
			set{ m_AutoConnect = value; }				
		}

		[SerializeField]
		private bool m_AutoJoinLobby = true;
		public bool AutoJoinLobby{
			get{ return ( Enabled ? m_AutoJoinLobby : false ); }
			set{ m_AutoJoinLobby = value; }				
		}

		[SerializeField]
		private bool m_JoinRandomRoom = true;
		public bool JoinRandomRoom{
			get{ return ( Enabled ? ( Rooms.Count == 0 ? true : m_JoinRandomRoom ) : false ); }
			set{ 				
				if( ! value && Rooms.Count == 0 )
					Rooms.Add( m_DefaultRoomName );

				m_JoinRandomRoom = value; 
			}				
		}

		[SerializeField]
		private List<string> m_Rooms = new List<string>();
		public List<string> Rooms{
			get{ return m_Rooms = ( m_Rooms == null ? new List<string>() : m_Rooms ); }
			set{ 
				Rooms.Clear();
				if( value == null ) return;
				foreach( string _room in value )
					Rooms.Add( _room );
			}				
		}
		public string DefaultPlayerName = "Player";

		[SerializeField]
		private string m_DefaultRoomName = "Room";
		public string DefaultRoomName{
			get{ return m_DefaultRoomName = ( m_DefaultRoomName.Trim() == string.Empty ? ( Rooms != null && Rooms.Count > 0 ? Rooms[0] : "Room" ) : m_DefaultRoomName ); }
			set{ m_DefaultRoomName = value; }
		}

		public bool RemoveIllegalCharacters = true;
		public bool UseRegularExpressions = true;
		public string IllegalCharactersSimple = "<>()[]{}!?\"'§$%&\\/*~+;:-=^°_";
		public string IllegalCharactersRegex = "[^a-zA-Z 0-9'.@]";

		public int MaxPlayersPerRoom = 16;
		public string Version = "1";

		public bool AutomaticallySyncScene = true;
		public bool AutomaticallyCleanUpPlayerObjects = false;
	}

}