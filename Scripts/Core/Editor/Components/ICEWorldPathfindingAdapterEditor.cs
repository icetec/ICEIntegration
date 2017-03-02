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
#elif ICE_APEX
using Apex;
using Apex.Steering;
using Apex.Steering.Components;
using Apex.PathFinding;
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
#if ICE_APEX && ICE_CC

			EditorGUILayout.Separator();

			SteerableUnitComponent m_SteerableUnitComponent = _adapter.GetComponent<SteerableUnitComponent>(); 
			SteerForPathComponent m_SteerForPathComponent = _adapter.GetComponent<SteerForPathComponent>(); 
			SteerToAlignWithVelocity m_SteerToAlignWithVelocity = _adapter.GetComponent<SteerToAlignWithVelocity>(); 
			HumanoidSpeedComponent m_HumanoidSpeedComponent = _adapter.GetComponent<HumanoidSpeedComponent>(); 

			PathOptionsComponent m_PathOptionsComponent = _adapter.GetComponent<PathOptionsComponent>(); 

			EditorGUI.BeginDisabledGroup( m_SteerableUnitComponent != null );
			GUI.backgroundColor = ( m_SteerableUnitComponent == null ? Color.yellow : Color.green );			
			if( ICEEditorLayout.Button( "Steerable Unit", "", ICEEditorStyle.ButtonExtraLarge ) )
				m_SteerableUnitComponent = _adapter.gameObject.AddComponent<SteerableUnitComponent>();
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup( m_SteerForPathComponent != null );
			GUI.backgroundColor = ( m_SteerForPathComponent == null ? Color.yellow : Color.green );			
			if( ICEEditorLayout.Button( "Steer For Path", "", ICEEditorStyle.ButtonExtraLarge ) )
				m_SteerForPathComponent = _adapter.gameObject.AddComponent<SteerForPathComponent>();
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup( m_SteerToAlignWithVelocity != null );
			GUI.backgroundColor = ( m_SteerToAlignWithVelocity == null ? Color.yellow : Color.green );			
			if( ICEEditorLayout.Button( "Steer To Align With Velocity", "", ICEEditorStyle.ButtonExtraLarge ) )
				m_SteerToAlignWithVelocity = _adapter.gameObject.AddComponent<SteerToAlignWithVelocity>();
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup( m_HumanoidSpeedComponent != null );
			GUI.backgroundColor = ( m_HumanoidSpeedComponent == null ? Color.yellow : Color.green );			
			if( ICEEditorLayout.Button( "Humanoid Speed Component", "", ICEEditorStyle.ButtonExtraLarge ) )
				m_HumanoidSpeedComponent = _adapter.gameObject.AddComponent<HumanoidSpeedComponent>();
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup( m_PathOptionsComponent != null );
			GUI.backgroundColor = ( m_PathOptionsComponent == null ? Color.yellow : Color.green );			
			if( ICEEditorLayout.Button( "Path Options Component", "", ICEEditorStyle.ButtonExtraLarge ) )
				m_PathOptionsComponent = _adapter.gameObject.AddComponent<PathOptionsComponent>();
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			EditorGUI.EndDisabledGroup();

#elif ICE_ASTAR && ICE_CC

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

			string _info = "This Adapter requires the A* Pathfinding Project or APEX Path and ICECreatureControl packages. " +
				"If all required assets are correct installed please add  'ICE_CC' and 'ICE_ASTAR' or 'ICE_APEX' to the Scripting Define Symbols.";
			EditorGUILayout.HelpBox( _info, MessageType.Info );
#endif

			EditorGUILayout.Separator();

		}
	}

}
