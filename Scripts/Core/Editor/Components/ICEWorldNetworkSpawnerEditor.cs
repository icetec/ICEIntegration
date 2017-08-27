// ##############################################################################
//
// ICEWorldNetworkSpawnerEditor.cs
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

#if ICE_CC
using ICE.Creatures;
using ICE.Creatures.Objects;
using ICE.Creatures.EnumTypes;
#endif

#if ICE_PUN

#endif

namespace ICE.Integration.Adapter
{

	[CustomEditor(typeof(ICEWorldNetworkSpawner))]
	#if ICE_PUN
	public class ICEWorldNetworkSpawnerEditor : ICEPhotonNetworkSpawnerEditor
	#else
	public class ICEWorldNetworkSpawnerEditor : ICEUnityNetworkSpawnerEditor  
	#endif
	{
		public override void OnInspectorGUI()
		{
			ICEWorldNetworkSpawner _target = DrawMonoHeader<ICEWorldNetworkSpawner>();
			DrawAdapterContent( _target );
			DrawMonoFooter( _target );
		}

		public void DrawAdapterContent( ICEWorldNetworkSpawner _adapter )
		{
			#if ICE_PUN
				DrawPhotonAdapterContent( _adapter );
			#else
				DrawUnityAdapterContent( _adapter );
			#endif

			EditorGUILayout.Separator();

			#if ICE_CC
			_adapter.UseDeactivateSceneCreatures = ICEEditorLayout.Toggle( "Deactivate Scene Creatures", "Deactivates existing scene creatures at awake", _adapter.UseDeactivateSceneCreatures, "" );
			_adapter.UseDeactivateScenePlayer = ICEEditorLayout.Toggle( "Deactivate Scene Player", "Deactivates existing scene player at awake", _adapter.UseDeactivateScenePlayer, "" );
			#endif

			EditorGUILayout.Separator();

		}
	}


}
