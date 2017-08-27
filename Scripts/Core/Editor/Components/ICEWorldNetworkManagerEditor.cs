// ##############################################################################
//
// ICEWorldNetworkManagerEditor.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;

using ICE;
using ICE.World;
using ICE.World.EditorUtilities;

using ICE.Integration;
using ICE.Integration.Objects;
using ICE.Integration.EditorInfos;

#if ICE_CC
using ICE.Creatures;
using ICE.Creatures.Objects;
using ICE.Creatures.EnumTypes;
#endif



namespace ICE.Integration.Adapter
{
	[CustomEditor(typeof(ICEWorldNetworkManager))]
	#if ICE_PUN && ICE_CC
	public class ICEWorldNetworkManagerEditor : ICEPhotonNetworkManagerEditor 
	#else
	public class ICEWorldNetworkManagerEditor : ICEWorldBehaviourEditor 
	#endif
	{
		public override void OnInspectorGUI()
		{
			ICEWorldNetworkManager _target = DrawMonoHeader<ICEWorldNetworkManager>();
			DrawAdapterContent( _target );
			DrawMonoFooter( _target );
		}
			
		public void DrawAdapterContent( ICEWorldNetworkManager _adapter )
		{
			#if ICE_PUN && ICE_CC
				DrawPhotonAdapterContent( _adapter );
			#else
				string _info = "This Adapter requires the Photon Unity Network and ICECreatureControl packages. " +
					"If both assets are correct installed please add 'ICE_PUN' and 'ICE_CC' to your custom defines.";
				EditorGUILayout.HelpBox( _info, MessageType.Info );
			#endif
		}
	}
}
