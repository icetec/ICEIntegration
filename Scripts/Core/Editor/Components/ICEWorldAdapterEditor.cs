// ##############################################################################
//
// ICEWorldTPCAdapterEditor.cs
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
	[CustomEditor(typeof(ICEWorldAdapter))]
	public class ICEWorldTPCAdapterEditor : ICEWorldBehaviourEditor
	{
		public override void OnInspectorGUI()
		{
			ICEWorldAdapter _target = DrawMonoHeader<ICEWorldAdapter>();
			DrawWorldTPCAdapter( _target );
			DrawMonoFooter( _target );
		}

		public void DrawWorldTPCAdapter( ICEWorldAdapter _adapter )
		{
			#if TPC
			#elif UFPS
			#elif RFPS
			#elif UNITZ
			#else
			#endif
		}
	}
}
