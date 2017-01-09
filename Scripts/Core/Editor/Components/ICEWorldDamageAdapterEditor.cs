// ##############################################################################
//
// ICEWorldDamageAdapterEditor.cs
// Version 1.3.5
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
			#if ICE_OPSIVE_TPC
			#elif ICE_UFPS
			_adapter.UseUFPSDamageHandling = ICEEditorLayout.Toggle( "Use UFPS Damage Handling", "", _adapter.UseUFPSDamageHandling );
			#elif RFPS
			#elif ICE_UNITZ
			#else
			#endif
		}
	}
}
