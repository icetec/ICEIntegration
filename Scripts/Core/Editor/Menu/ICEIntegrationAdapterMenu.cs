// ##############################################################################
//
// ICECreatureControlMenu.cs
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

using ICE;
using ICE.World.EditorUtilities;

using ICE.Creatures;
using ICE.Creatures.Utilities;
using ICE.Creatures.Objects;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Attributes;
using ICE.Creatures.EditorInfos;
using ICE.Creatures.Windows;

namespace ICE.Integration.Menus
{
	public class ICEIntegrationAdapterMenu : MonoBehaviour {


		[MenuItem ( "ICE/ICE Integration/Adapter/Import Energy Bar Toolkit Adapter", false, 8091 )]
		static void ImportAdapterEBT() 
		{
			string _path = Application.dataPath + "/ICE/ICEIntegration/Adapter/";
			string _file = "ICECreatureEBTAdapter.unitypackage";

			AssetDatabase.ImportPackage( _path + _file , true);
			AssetDatabase.Refresh();
		}

		[MenuItem ( "ICE/ICE Integration/Adapter/Import ProTips Adapter", false, 8091 )]
		static void ImportAdapterProTips() 
		{
			string _path = Application.dataPath + "/ICE/ICEIntegration/Adapter/";
			string _file = "ICECreatureProTipsAdapter.unitypackage";

			AssetDatabase.ImportPackage( _path + _file , true);
			AssetDatabase.Refresh();
		}

		[MenuItem ( "ICE/ICE Integration/Adapter/Import GameFlow Adapter", false, 8091 )]
		static void ImportAdapterGameFlow() 
		{
			string _path = Application.dataPath + "/ICE/ICEIntegration/Adapter/";
			string _file = "ICEIntegrationGameFlowAdapter.unitypackage";

			AssetDatabase.ImportPackage( _path + _file , true);
			AssetDatabase.Refresh();
		}
	}
}