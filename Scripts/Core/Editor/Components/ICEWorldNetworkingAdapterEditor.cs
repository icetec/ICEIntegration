// ##############################################################################
//
// ICEWorldNetworkingAdapterEditor.cs
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

	[CustomEditor(typeof(ICEWorldNetworkingAdapter))]
	public class ICEWorldNetworkingAdapterEditor : ICEWorldBehaviourEditor {

		public override void OnInspectorGUI()
		{
			ICEWorldNetworkingAdapter _target = DrawMonoHeader<ICEWorldNetworkingAdapter>();
			DrawAdapterContent( _target );
			DrawMonoFooter( _target );
		}

		public void DrawAdapterContent( ICEWorldNetworkingAdapter _adapter )
		{
#if ICE_PUN && ICE_CC

			PhotonView m_PhotonView = _adapter.GetComponent<PhotonView>(); 
			PhotonTransformView m_PhotonTransformView = _adapter.GetComponent<PhotonTransformView>(); 

			ICECreatureControl m_Creature = _adapter.GetComponent<ICECreatureControl>(); 
			ICECreaturePlayer m_Player = _adapter.GetComponent<ICECreaturePlayer>(); 
			ICECreatureEntity m_Entity = _adapter.GetComponent<ICECreatureEntity>(); 

			EditorGUI.BeginDisabledGroup( m_PhotonView != null );
				GUI.backgroundColor = ( m_PhotonView == null ? Color.yellow : Color.green );			
				if( ICEEditorLayout.Button( "Photon View", "", ICEEditorStyle.ButtonExtraLarge ) )
					m_PhotonView = _adapter.gameObject.AddComponent<PhotonView>();
				GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			EditorGUI.EndDisabledGroup();

			if( m_PhotonView != null && m_PhotonView.ObservedComponents != null )
			{
				// CREATURE VIEW BEGIN
				if( m_Creature != null )
				{
					int _index = m_PhotonView.ObservedComponents.FindIndex( _comp => _comp != null &&_comp.GetType() == typeof(ICEWorldNetworkingAdapter) );
					GUI.backgroundColor = ( _index == -1 ? Color.yellow : Color.green );
					EditorGUI.BeginDisabledGroup( _index >= 0 );
					if( ICEEditorLayout.Button( "Observe Photon Creature View", "", ICEEditorStyle.ButtonExtraLarge ) )
						m_PhotonView.ObservedComponents.Add( _adapter );
					EditorGUI.EndDisabledGroup();
					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;					
				}
				// CREATURE VIEW END

				// PLAYER VIEW BEGIN
				else if( m_Player != null )
				{
					int _index = m_PhotonView.ObservedComponents.FindIndex( _comp => _comp != null &&_comp.GetType() == typeof(ICEWorldNetworkingAdapter) );
					GUI.backgroundColor = ( _index == -1 ? Color.yellow : Color.green );
					EditorGUI.BeginDisabledGroup( _index >= 0 );
					if( ICEEditorLayout.Button( "Observe Photon Player View", "", ICEEditorStyle.ButtonExtraLarge ) )
						m_PhotonView.ObservedComponents.Add( _adapter );
					EditorGUI.EndDisabledGroup();
					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;					
				}
				// PLAYER VIEW END

				// ENTITY VIEW BEGIN
				else if( m_Entity != null )
				{
					int _index = m_PhotonView.ObservedComponents.FindIndex( _comp => _comp != null &&_comp.GetType() == typeof(ICEWorldNetworkingAdapter) );
					GUI.backgroundColor = ( _index == -1 ? Color.yellow : Color.green );
					EditorGUI.BeginDisabledGroup( _index >= 0 );
					if( ICEEditorLayout.Button( "Observe Photon Entity View", "", ICEEditorStyle.ButtonExtraLarge ) )
						m_PhotonView.ObservedComponents.Add( _adapter );
					EditorGUI.EndDisabledGroup();
					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;					
				}
				// ENTITY VIEW END

				// TRANSFORM VIEW BEGIN
				if( m_PhotonTransformView == null )
				{
					GUI.backgroundColor = Color.yellow;
					if( ICEEditorLayout.Button( "Observe Photon Transform View", "", ICEEditorStyle.ButtonExtraLarge ) )
					{
						m_PhotonTransformView = _adapter.gameObject.AddComponent<PhotonTransformView>();
						m_PhotonView.ObservedComponents.Add( m_PhotonTransformView );
					}
					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
				}
				else
				{
					int _index = m_PhotonView.ObservedComponents.FindIndex( _comp => _comp != null &&_comp.GetType() == typeof(PhotonTransformView) );
					GUI.backgroundColor = ( _index == -1 ? Color.yellow : Color.green );
					EditorGUI.BeginDisabledGroup( _index >= 0 );
					if( ICEEditorLayout.Button( "Observe Photon Transform View", "", ICEEditorStyle.ButtonExtraLarge ) )
						m_PhotonView.ObservedComponents.Add( m_PhotonTransformView );
					EditorGUI.EndDisabledGroup();
					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
				}
				// TRANSFORM VIEW END

				if( m_PhotonView.ObservedComponents.Count > 1 && m_PhotonView.ObservedComponents[0] == null )
					m_PhotonView.ObservedComponents.RemoveAt(0);
			}
			else
			{
				EditorGUI.BeginDisabledGroup( m_PhotonView == null );
					ICEEditorLayout.Button( "Observe Photon Creature View", "", ICEEditorStyle.ButtonExtraLarge );
					ICEEditorLayout.Button( "Observe Photon Transform View", "", ICEEditorStyle.ButtonExtraLarge );
				EditorGUI.EndDisabledGroup();
			}

#else

			string _info = "This Adapter requires the Photon Unity Network and ICECreatureControl packages. " +
			"If both assets are correct installed please add 'ICE_PUN' and 'ICE_CC' to your custom defines.";
			EditorGUILayout.HelpBox( _info, MessageType.Info );
#endif

			EditorGUILayout.Separator();

		}
	}

}
