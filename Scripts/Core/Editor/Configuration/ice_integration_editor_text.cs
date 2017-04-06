// ##############################################################################
//
// ice_integration_editor_text.cs | Info
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;

using ICE;
using ICE.World;
using ICE.World.EditorUtilities;

namespace ICE.Integration.EditorInfos
{
	public class Info : ICE.World.EditorInfos.Info
	{
		public static readonly string NETWORK_MANAGER_DISPLAY_OPTIONS = "";
		public static readonly string NETWORK_MANAGER_SCENE_OPTIONS = "";

		public static readonly string SYNC_MOVEMENTS = "";
		public static readonly string SYNC_MOVEMENTS_SMOOTHING = "";

		public static readonly string SYNC_STATUS = "";
	}
}

