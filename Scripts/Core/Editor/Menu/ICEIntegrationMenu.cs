// ##############################################################################
//
// ICEIntegrationMenu.cs
// Version 1.4.0
//
// Copyrights © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.icecreaturecontrol.com
// mailto:support@icecreaturecontrol.com
// 
// Unity Asset Store End User License Agreement (EULA)
// http://unity3d.com/legal/as_terms
//
// ##############################################################################

using UnityEditor;
using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.Collections;

using ICE;
using ICE.World;
using ICE.World.EditorUtilities;
using ICE.World.Windows;

using ICE.Integration.Adapter;
using ICE.Integration.Windows;

namespace ICE.Integration.Menus
{
	public class ICEIntegrationMenu : MonoBehaviour {

		// GENERAL
		[MenuItem ("ICE/ICE Integration/Identify Supported Assets", false, 8001 )]
		static void IdentifySupportedAssets (){
			ICEIntegrationTools.ValidateDefines();
		}

		// INFOS
		[MenuItem ("ICE/ICE Integration/Repository", false, 9000 )]
		static void Repository (){
			Application.OpenURL("https://github.com/icetec/ICEIntegration");
		}

		[MenuItem ("ICE/ICE Integration/Wiki", false, 9000 )]
		static void Wiki (){
			Application.OpenURL("https://github.com/icetec/ICEIntegration/wiki");
		}

		[MenuItem ("ICE/ICE Integration/About", false, 9000 )]
		static void ShowAbout(){
			ICEIntegrationAbout.Create();
		} 
	
	}
}