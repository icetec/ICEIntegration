// ##############################################################################
//
// ICEIntegrationEnvironmentMenu.cs
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


namespace ICE.Integration.Menus
{
	public class ICEIntegrationEnvironmentMenu : MonoBehaviour {


		[MenuItem ("ICE/ICE Integration/Environment/Create Environment Adapter", false, 8022 )]
		static void AddEnvironmentAdapter(){

			if( ICEWorldEnvironmentAdapter.Instance == null )
			{
				GameObject _object = new GameObject();
				_object.name = "Environment";
				_object.AddComponent<ICEWorldEnvironmentAdapter>();
			}
		}

		[MenuItem ( "ICE/ICE Integration/Environment/Create Environment Adapter", true)]
		static bool ValidateEnvironmentAdapter(){
			#if ICE_UNISTORM || ICE_TENKOKU || ICE_WEATHER_MAKER || ICE_ENVIRO
			return true;
			#else
			return false;
			#endif
		}

		[MenuItem ("ICE/ICE Integration/Environment/Remove Environment Adapter", false, 8022 )]
		static void RemoveEnvironmentAdapter(){
			
			if( ICEWorldEnvironmentAdapter.Instance != null )
			{
				if( ICEWorldEnvironmentAdapter.Instance.gameObject.name == "Environment" )
					DestroyImmediate( ICEWorldEnvironmentAdapter.Instance.gameObject );
				else
					DestroyImmediate( ICEWorldEnvironmentAdapter.Instance );
			}
		}

		[MenuItem ( "ICE/ICE Integration/Environment/Remove Environment Adapter", true)]
		static bool ValidateRemoveEnvironmentAdapter(){
			#if ICE_UNISTORM || ICE_TENKOKU || ICE_WEATHER_MAKER || ICE_ENVIRO
			return true;
			#else
			return false;
			#endif
		}

	}
}