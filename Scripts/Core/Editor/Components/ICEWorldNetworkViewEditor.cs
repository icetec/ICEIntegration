// ##############################################################################
//
// ICEWorldNetworkViewEditor.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;

using ICE;
using ICE.World;
using ICE.World.EditorUtilities;

using ICE.Integration;
using ICE.Integration.EditorInfos;

#if ICE_CC
using ICE.Creatures;
using ICE.Creatures.Objects;
using ICE.Creatures.EnumTypes;
#endif

namespace ICE.Integration.Adapter
{
	[CustomEditor(typeof(ICEWorldNetworkView))]
	#if ICE_PUN
		public class ICEWorldNetworkViewEditor : ICEPhotonNetworkViewEditor
	#else
		public class ICEWorldNetworkViewEditor : ICEUnityNetworkViewEditor 
	#endif
	{
		public override void OnInspectorGUI()
		{
			ICEWorldNetworkView _target = DrawMonoHeader<ICEWorldNetworkView>();
			DrawAdapterContent( _target );
			DrawMonoFooter( _target );
		}

		public void DrawAdapterContent( ICEWorldNetworkView _adapter )
		{
			#if ICE_PUN
				DrawPhotonAdapterContent( _adapter );
			#else
				DrawUnityAdapterContent( _adapter );
			#endif

			EditorGUILayout.Separator();

		}
	}

}
