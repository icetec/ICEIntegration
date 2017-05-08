// ##############################################################################
//
// ICEWorldEnvironmentAdapterEditor.cs
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
using ICE.World.EnumTypes;
using ICE.World.Utilities;
using ICE.World.Objects;
using ICE.World.EditorUtilities;
using ICE.World.EditorInfos;

namespace ICE.Integration.Adapter
{
	[CustomEditor(typeof(ICEWorldEnvironmentAdapter))]
	public class ICEWorldEnvironmentAdapterEditor : ICEWorldBehaviourEditor
	{
		public override void OnInspectorGUI()
		{
			ICEWorldEnvironmentAdapter _target = DrawDefaultHeader<ICEWorldEnvironmentAdapter>();
			DrawAdapterContent( _target );
			DrawDefaultFooter( _target );

		}

		public void DrawAdapterContent( ICEWorldEnvironmentAdapter m_adapter )
		{
			#if ICE_WEATHER_MAKER
				ICEEditorLayout.BeginHorizontal();
				m_adapter.UpdateInterval = ICEEditorLayout.DefaultSlider( "Update Interval", "", m_adapter.UpdateInterval, 0.05f, 0, 360, 10 );	

				if( ICEEditorLayout.Button( "UPDATE", "Update Weather", ICEEditorStyle.ButtonMiddle ) )
					m_adapter.UpdateWeather();
				ICEEditorLayout.EndHorizontal();
			#elif ICE_TENKOKU
				ICEEditorLayout.BeginHorizontal();
				m_adapter.UpdateInterval = ICEEditorLayout.DefaultSlider( "Update Interval", "", m_adapter.UpdateInterval, 0.05f, 0, 360, 10 );	

				if( ICEEditorLayout.Button( "UPDATE", "Update Weather", ICEEditorStyle.ButtonMiddle ) )
					m_adapter.UpdateWeather();
				ICEEditorLayout.EndHorizontal();
			#elif ICE_UNISTORM
				ICEEditorLayout.BeginHorizontal();
						m_adapter.UpdateInterval = ICEEditorLayout.DefaultSlider( "Update Interval", "", m_adapter.UpdateInterval, 0.05f, 0, 360, 10 );	

					if( ICEEditorLayout.Button( "UPDATE", "Update Weather", ICEEditorStyle.ButtonMiddle ) )
						m_adapter.UpdateWeather();
				ICEEditorLayout.EndHorizontal();
			#else
			#endif
		}
	}
}