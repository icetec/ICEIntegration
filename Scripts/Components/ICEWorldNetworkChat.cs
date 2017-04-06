// ##############################################################################
//
// ICEWorldNetworkChat.cs
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
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;


using ICE;
using ICE.World;

using ICE.Integration;


#if ICE_PUN

#endif

namespace ICE.Integration.Adapter
{
	#if ICE_PUN
	[RequireComponent(typeof(PhotonView))]
	public class ICEWorldNetworkChat : Photon.MonoBehaviour 
	{

		protected static ICEWorldNetworkChat m_Instance = null;
		public static ICEWorldNetworkChat Instance{
			get{ return m_Instance = ( m_Instance == null?GameObject.FindObjectOfType<ICEWorldNetworkChat>():m_Instance ); }
		}

		public GameObject ChatPanel = null;
		public InputField ChatInputField = null;
		public Text ChatMessages = null;

		public KeyCode ActivationKey = KeyCode.Tab;

		public int MaxMessages = 20;
		public bool DeactivateOnSend = false;

		public bool IsVisible = true;

		public List<string> m_Messages = new List<string>();

		public static readonly string ChatRPC = "Chat";
		public static readonly string SysMsgRPC = "SystemMessage";

		public void Start()
		{
			if( ChatInputField != null )
				ChatInputField.DeactivateInputField();
				
			if( ChatMessages != null )
			{
				ChatMessages.raycastTarget = false;
				ChatMessages.verticalOverflow = VerticalWrapMode.Overflow;
				ChatMessages.horizontalOverflow = HorizontalWrapMode.Overflow;
			}
		}

		public void Update()
		{
			if( ! this.IsVisible || ! PhotonNetwork.inRoom )
				return;

			if( ChatInputField != null )
			{
				if( Input.GetKeyDown( ActivationKey ) )
				{
					if( ChatInputField.isFocused )
						ChatInputField.DeactivateInputField();
					else
						ChatInputField.ActivateInputField();		
				}

				if( ! ChatInputField.isFocused )
					ChatInputField.text = "";
			}
		}

		public void OnGUI()
		{
			if( ! this.IsVisible || ! PhotonNetwork.inRoom )
				return;

			if( m_Messages.Count >= MaxMessages )
				m_Messages.RemoveAt(0);

			if( ChatInputField != null && ChatInputField.isFocused )
			{
				if( Event.current.type == EventType.KeyDown && ( Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return ) )
				{
					if( ! string.IsNullOrEmpty( ChatInputField.text ) )
					{
						this.photonView.RPC( "Chat", PhotonTargets.All, ChatInputField.text );
						ChatInputField.text = "";

						if( DeactivateOnSend )
							ChatInputField.DeactivateInputField();
							
						return;
					}
				}
			}

			if( ChatMessages != null )
			{
				ChatMessages.text = "";
				for( int i = 0 ; i < m_Messages.Count; i++ )
					ChatMessages.text += "\n" + m_Messages[i] ;
			}
		}

		[PunRPC]
		public void Chat( string _line, PhotonMessageInfo _info )
		{
			string _sender = "anonymous";

			if( _info.sender != null )
			{
				if( ! string.IsNullOrEmpty( _info.sender.NickName ) )
					_sender = _info.sender.NickName;
				else
					_sender = "player " + _info.sender.ID;
			}

			string _msg = ( PhotonNetwork.player.NickName == _sender ? "<color=yellow>" : "<color=white>" ); 
			_msg += _sender + ": " + _line;
			_msg += "</color>";

			this.m_Messages.Add( _msg );
		}

		[PunRPC]
		public void SystemMessage( string _line, PhotonMessageInfo _info )
		{
			string _sender = "System";

			this.m_Messages.Add( "<color=red>" + _sender + ": " + _line + "</color>" );
		}

		public void AddMessage( string _line )
		{
			//this.m_Messages.Add( _line );
		}

		public void SendSystemMessage( string _line )
		{
			this.photonView.RPC( "SystemMessage", PhotonTargets.All, _line );
		}
	}
	#else
		public class ICEWorldNetworkChat : MonoBehaviour{

			protected static ICEWorldNetworkChat m_Instance = null;
			public static ICEWorldNetworkChat Instance{
			get{ return m_Instance = ( m_Instance == null?GameObject.FindObjectOfType<ICEWorldNetworkChat>():m_Instance ); }
			}

			public void SendSystemMessage( string _line ){}
		}
	#endif


	public class NetworkChat 
	{
		public static void SendSystemMessage( string _line )
		{
			if( ICEWorldNetworkChat.Instance != null )
				ICEWorldNetworkChat.Instance.SendSystemMessage( _line );
		}
	}
}
