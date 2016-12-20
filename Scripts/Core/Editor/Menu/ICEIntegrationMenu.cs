// ##############################################################################
//
// ICEIntegrationMenu.cs
// Version 1.3.5
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


namespace ICE.Integration.Menus
{
	public class ICEIntegrationMenu : MonoBehaviour {

		// GENERAL
		[MenuItem ("ICE/ICE Integration/Identify Supported Assets", false, 8001 )]
		static void IdentifySupportedAssets (){
			ICEIntegrationTools.ValidateDefines();
		}

	
	}
}