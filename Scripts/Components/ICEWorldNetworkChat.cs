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

namespace ICE.Integration.Adapter
{
	#if ICE_PUN
	[RequireComponent(typeof(PhotonView))]
	public class ICEWorldNetworkChat : ICEPhotonNetworkChat{}
	#else
	public class ICEWorldNetworkChat : MonoBehaviour
	{
		protected static ICEWorldNetworkChat m_Instance = null;
		public static ICEWorldNetworkChat Instance{
			get{ return m_Instance = ( m_Instance == null?GameObject.FindObjectOfType<ICEWorldNetworkChat>():m_Instance ); }
		}

		public void SendSystemMessage( string _line ){}
	}

	public class NetworkChat 
	{
		public static void SendSystemMessage( string _line )
		{
			if( ICEWorldNetworkChat.Instance != null )
				ICEWorldNetworkChat.Instance.SendSystemMessage( _line );
		}
	}

	#endif
}
