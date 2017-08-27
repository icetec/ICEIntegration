// ##############################################################################
//
// ICEWorldNetworkManager.cs
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
using ICE.World.Objects;
using ICE.World.Utilities;

using ICE.Integration;
using ICE.Integration.Objects;



namespace ICE.Integration.Adapter
{

#if ICE_PUN
	public class ICEWorldNetworkManager : ICEPhotonNetworkManager{}
#else
	public class ICEWorldNetworkManager : MonoBehaviour{

		protected static ICEWorldNetworkManager m_Instance = null;
		public static ICEWorldNetworkManager Instance{
			get{ return m_Instance = ( m_Instance == null?GameObject.FindObjectOfType<ICEWorldNetworkManager>():m_Instance ); }
		}
	}
#endif
}