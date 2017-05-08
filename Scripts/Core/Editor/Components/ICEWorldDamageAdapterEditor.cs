// ##############################################################################
//
// ICEWorldDamageAdapterEditor.cs
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
using ICE.World.Utilities;
using ICE.World.EditorUtilities;

namespace ICE.Integration.Adapter
{
	[CustomEditor(typeof(ICEWorldDamageAdapter))]
	public class ICEWorldDamageAdapterEditor : ICEWorldBehaviourEditor
	{
		public override void OnInspectorGUI()
		{
			ICEWorldDamageAdapter _target = DrawMonoHeader<ICEWorldDamageAdapter>();
			DrawAdapterContent( _target );
			DrawMonoFooter( _target );
		}

		public void DrawAdapterContent( ICEWorldDamageAdapter _adapter )
		{
			EditorGUILayout.HelpBox( "The ICEWorldDamageAdapter handles the damage impacts of " +
				"supported 3rd Party Assets. Add this script to all ICE entities that can be " +
				"damaged (e.g. creatures and their body parts, objects, items etc.).", MessageType.None );

			#if ICE_OPSIVE_TPC
			#elif ICE_OPSIVE_TPC
			#elif ICE_UFPS
			_adapter.UseUFPSDamageHandling = ICEEditorLayout.Toggle( "Use UFPS Damage Handling", "", _adapter.UseUFPSDamageHandling );
			#elif ICE_RFPS
			#elif ICE_UNITZ
			#elif ICE_EASY_WEAPONS
			#elif ICE_ULTIMATE_SURVIVAL
			/*
			UltimateSurvival.EntityEventHandler _entity_event_handler = _adapter.gameObject.GetComponentInParent< UltimateSurvival.EntityEventHandler >();

			EditorGUI.BeginDisabledGroup( _entity_event_handler != null );
			GUI.backgroundColor = ( _entity_event_handler == null ? Color.yellow : Color.green );			
			if( ICEEditorLayout.Button( "Entity Event Handler", "", ICEEditorStyle.ButtonExtraLarge ) )
				_entity_event_handler = _adapter.gameObject.AddComponent<UltimateSurvival.EntityEventHandler>();
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			EditorGUI.EndDisabledGroup();*/
			#elif ICE_UMMORPG
			#else
			#endif
		}
	}
}
