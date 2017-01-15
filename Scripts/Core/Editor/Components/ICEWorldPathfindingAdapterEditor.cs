// ##############################################################################
//
// ICEWorldPathfindingAdapter.cs
// Version 1.3.7
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

#if ICE_ASTAR
using Pathfinding;
using Pathfinding.RVO;
#endif

namespace ICE.Integration.Adapter
{

	[CustomEditor(typeof(ICEWorldPathfindingAdapter))]
	public class ICEWorldPathfindingAdapterEditor : ICEWorldBehaviourEditor {

		private enum _modifier_types{
			AdvancedSmoothModifier,
			AlternativePathModifier,
			FunnelModifier,
			RadiusModifier,
			RaycastModifier,
			SimpleSmoothModifier,
			StartEndModifier
		}

		private static _modifier_types _modifier_type;

		public override void OnInspectorGUI()
		{
			ICEWorldPathfindingAdapter _target = DrawMonoHeader<ICEWorldPathfindingAdapter>();
			DrawAdapterContent( _target );
			DrawMonoFooter( _target );
		}

		public void DrawAdapterContent( ICEWorldPathfindingAdapter _adapter )
		{
#if ICE_ASTAR && ICE_CC

			EditorGUILayout.Separator();	

			ICEEditorLayout.BeginHorizontal();
				EditorGUI.BeginDisabledGroup( _adapter.AutoRepath == false );
					_adapter.AutoRepathRate = ICEEditorLayout.DefaultSlider( "Auto Repath Rate", "", _adapter.AutoRepathRate, 0.05f, 0, 10, 0.5f );
				EditorGUI.EndDisabledGroup();
				_adapter.AutoRepath = ICEEditorLayout.EnableButton( _adapter.AutoRepath );
			ICEEditorLayout.EndHorizontal();

			_adapter.SlowdownDistance = ICEEditorLayout.DefaultSlider( "Slowdown Distance", "", _adapter.SlowdownDistance, 0.05f, 0, 10, 0.6f );
			_adapter.PickNextWaypointDist = ICEEditorLayout.DefaultSlider( "PickNextWaypointDist", "", _adapter.PickNextWaypointDist, 0.05f, 0, 10, 2 );
			_adapter.ForwardLook = ICEEditorLayout.DefaultSlider( "ForwardLook", "", _adapter.ForwardLook, 0.05f, 0, 10, 1 );
			_adapter.EndReachedDistance = ICEEditorLayout.DefaultSlider( "EndReachedDistance", "", _adapter.EndReachedDistance, 0.05f, 0, 10, 0.2F );

			_adapter.CanMove = ICEEditorLayout.Toggle( "Can Move", "Enables or disables movement", _adapter.CanMove, "" );
			_adapter.ClosestOnPathCheck = ICEEditorLayout.Toggle( "ClosestOnPathCheck", "Enables or disables searching for paths", _adapter.ClosestOnPathCheck, "" );

			EditorGUILayout.Separator();	
	
			ICEEditorLayout.BeginHorizontal();
			ICEEditorLayout.Label( "RVO Controller" );

			RVOController _rvo = _adapter.gameObject.GetComponent<RVOController>();

			if( _rvo != null && _rvo.enabled )
			{
				if( ! ICEEditorLayout.CheckButtonMiddle( "ENABLED", "", true ) )
					_rvo.enabled = false;
			}
			else 
			{
				if( ICEEditorLayout.CheckButtonMiddle( "ENABLED", "", false ) )
				{
					if( _rvo == null )
						_rvo = _adapter.gameObject.AddComponent<RVOController>();

					if( _rvo != null && ! _rvo.enabled )
						_rvo.enabled = true;
				}
			}
			ICEEditorLayout.EndHorizontal();
	
			ICEEditorLayout.BeginHorizontal();

					_modifier_type = (_modifier_types)EditorGUILayout.EnumPopup( "Path Modifier", _modifier_type );

				MonoModifier _modifier = null;

				switch( _modifier_type )
				{
					case _modifier_types.AdvancedSmoothModifier:
						_modifier = _adapter.gameObject.GetComponent<AdvancedSmooth>();
						break;
					case _modifier_types.AlternativePathModifier:
						_modifier = _adapter.gameObject.GetComponent<AlternativePath>();
						break;
					case _modifier_types.FunnelModifier:
						_modifier = _adapter.gameObject.GetComponent<FunnelModifier>();
						break;
					case _modifier_types.RadiusModifier:
						_modifier = _adapter.gameObject.GetComponent<RadiusModifier>();
						break;
					case _modifier_types.RaycastModifier:
						_modifier = _adapter.gameObject.GetComponent<RaycastModifier>();
						break;
					case _modifier_types.SimpleSmoothModifier:
						_modifier = _adapter.gameObject.GetComponent<SimpleSmoothModifier>();
						break;
					/*case _modifier_types.StartEndModifier:
						_modifier = _adapter.gameObject.GetComponent<StartEndModifier>() as MonoModifier;
						break;*/
				}

			if( _modifier != null && _modifier.enabled )
			{
				if( ! ICEEditorLayout.CheckButtonMiddle( "ENABLED", "", true ) )
					_modifier.enabled = false;
			}
			else 
			{
				if( ICEEditorLayout.CheckButtonMiddle( "ENABLED", "", false ) )
				{
					if( _modifier == null )
					{
						switch( _modifier_type )
						{
							case _modifier_types.AdvancedSmoothModifier:
								_modifier = _adapter.gameObject.AddComponent<AdvancedSmooth>();
								break;
							case _modifier_types.AlternativePathModifier:
								_modifier = _adapter.gameObject.AddComponent<AlternativePath>();
								break;
							case _modifier_types.FunnelModifier:
								_modifier = _adapter.gameObject.AddComponent<FunnelModifier>();
								break;
							case _modifier_types.RadiusModifier:
								_modifier = _adapter.gameObject.AddComponent<RadiusModifier>();
								break;
							case _modifier_types.RaycastModifier:
								_modifier = _adapter.gameObject.AddComponent<RaycastModifier>();
								break;
							case _modifier_types.SimpleSmoothModifier:
								_modifier = _adapter.gameObject.AddComponent<SimpleSmoothModifier>();
								break;
							/*case _modifier_types.StartEndModifier:
								_modifier = _adapter.gameObject.AddComponent<StartEndModifier>() as MonoModifier;
								break;*/
						}
					}

					if( _modifier != null && ! _modifier.enabled )
						_modifier.enabled = true;
				}
			}
			ICEEditorLayout.EndHorizontal();

#else

			string _info = "This Adapter requires the A* Pathfinding Project and ICECreatureControl packages. " +
				"If both assets are correct installed please add 'ASTAR' and 'ICECC' to your custom defines.";
			EditorGUILayout.HelpBox( _info, MessageType.Info );
#endif

			EditorGUILayout.Separator();

		}
	}

}
