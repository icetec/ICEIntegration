// ##############################################################################
//
// ICEWorldNetworkViewEditor.cs
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

using ICE.Integration;
using ICE.Integration.EditorInfos;

#if ICE_CC
using ICE.Creatures;
using ICE.Creatures.Objects;
using ICE.Creatures.EnumTypes;
#endif

#if ICE_PUN

#endif

namespace ICE.Integration.Adapter
{

	[CustomEditor(typeof(ICEWorldNetworkView))]
	public class ICEWorldNetworkViewEditor : ICEWorldBehaviourEditor {

		public override void OnInspectorGUI()
		{
			ICEWorldNetworkView _target = DrawMonoHeader<ICEWorldNetworkView>();
			DrawAdapterContent( _target );
			DrawMonoFooter( _target );
		}

#if ICE_PUN
		private T DrawObservedComponent<T>( ICEWorldNetworkView _adapter, PhotonView _view, T _component, string _title ) where T : Component
		{
			if( _component == null )
			{
				GUI.backgroundColor = Color.yellow;
				if( ICEEditorLayout.Button( "Observe " + _title, "", ICEEditorStyle.ButtonExtraLarge ) )
				{
					_component = _adapter.gameObject.AddComponent<T>();
					_view.ObservedComponents.Add( _component );
				}
				GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			}
			else
			{
				int _index = _view.ObservedComponents.FindIndex( _comp => _comp != null &&_comp.GetType() == typeof(T) );
				GUI.backgroundColor = ( _index == -1 ? Color.yellow : Color.green );

				if( _index == -1 )
				{
					if( ICEEditorLayout.Button( "Observe " + _title, "", ICEEditorStyle.ButtonExtraLarge ) )
						_view.ObservedComponents.Add( _component );
				}
				else
				{
					if( ICEEditorLayout.Button( "Remove " + _title, "", ICEEditorStyle.ButtonExtraLarge ) )
					{
						_view.ObservedComponents.Remove( _component );

						if( _component != _adapter )
							DestroyImmediate( _component );
					}
				}
				GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			}

			return _component;
		}
#endif

		public void DrawAdapterContent( ICEWorldNetworkView _adapter )
		{
#if ICE_PUN && ICE_CC

			PhotonView m_PhotonView = _adapter.GetComponent<PhotonView>(); 
			PhotonTransformView m_PhotonTransformView = _adapter.GetComponent<PhotonTransformView>(); 
			PhotonRigidbodyView m_PhotonRigidbodyView = _adapter.GetComponent<PhotonRigidbodyView>(); 
			PhotonAnimatorView m_PhotonAnimatorView = _adapter.GetComponent<PhotonAnimatorView>(); 

			ICECreatureEntity m_Entity = _adapter.GetComponent<ICECreatureEntity>(); 

			EditorGUI.BeginDisabledGroup( m_PhotonView != null );
				GUI.backgroundColor = ( m_PhotonView == null ? Color.yellow : Color.green );			
				if( ICEEditorLayout.Button( "Photon View", "", ICEEditorStyle.ButtonExtraLarge ) )
					m_PhotonView = _adapter.gameObject.AddComponent<PhotonView>();
				GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			EditorGUI.EndDisabledGroup();

			if( m_PhotonView != null && m_PhotonView.ObservedComponents != null )
			{
				m_PhotonTransformView = DrawObservedComponent<PhotonTransformView>( _adapter, m_PhotonView, m_PhotonTransformView, "Photon Transform View" );
				m_PhotonRigidbodyView = DrawObservedComponent<PhotonRigidbodyView>( _adapter, m_PhotonView, m_PhotonRigidbodyView, "Photon Rigidbody View" );
				m_PhotonAnimatorView = DrawObservedComponent<PhotonAnimatorView>( _adapter, m_PhotonView, m_PhotonAnimatorView, "Photon Animator View" );

				string _title = ( m_Entity != null ? m_Entity.EntityType.ToString() : "Network" );		
				DrawObservedComponent<ICEWorldNetworkView>( _adapter, m_PhotonView, _adapter, "ICE " + _title + " View" );

				bool _can_move = false;
				if( m_Entity == null || 
					( m_Entity.EntityType == ICE.World.EnumTypes.EntityClassType.Player ) || 
					( m_Entity.EntityType == ICE.World.EnumTypes.EntityClassType.Creature ) || 
					( m_Entity.EntityType == ICE.World.EnumTypes.EntityClassType.Item ) ||
					( m_Entity.EntityType == ICE.World.EnumTypes.EntityClassType.Projectile ) )
					_can_move = true;
				
				// SYNC OPTIONS BEGIN
				EditorGUI.indentLevel++;		
					EditorGUI.BeginDisabledGroup( _can_move == false );
						_adapter.SynchronizeMovement = ICEEditorLayout.Toggle( "Synchronize Movement", "", _adapter.SynchronizeMovement, Info.SYNC_MOVEMENTS );
						EditorGUI.BeginDisabledGroup( _adapter.SynchronizeMovement == false );
							EditorGUI.indentLevel++;
								_adapter.SycnMovementSmoothingDelay = ICEEditorLayout.DefaultSlider( "Smoothing Delay", "", _adapter.SycnMovementSmoothingDelay, Init.DECIMAL_PRECISION_TIMER, 0, 20, 5, Info.SYNC_MOVEMENTS_SMOOTHING );
							EditorGUI.indentLevel--;
						EditorGUI.EndDisabledGroup();
					EditorGUI.EndDisabledGroup();

					_adapter.SynchronizeStatus = ICEEditorLayout.Toggle( "Synchronize Status", "", _adapter.SynchronizeStatus, Info.SYNC_STATUS );
				EditorGUI.indentLevel--;
				// SYNC OPTIONS END

				// Clean-up empty components
				if( m_PhotonView.ObservedComponents.Count > 1 )
				{
					for( int i = 0 ; i < m_PhotonView.ObservedComponents.Count ; i++ )
					{
						if( m_PhotonView.ObservedComponents[i] == null )
						{
							m_PhotonView.ObservedComponents.RemoveAt(i);
							--i;
						}
					}
				}
					
			}
			else
			{
				EditorGUI.BeginDisabledGroup( m_PhotonView == null );
					ICEEditorLayout.Button( "Observe Photon Transform View", "", ICEEditorStyle.ButtonExtraLarge );
					ICEEditorLayout.Button( "Observe Photon Rigidbody View", "", ICEEditorStyle.ButtonExtraLarge );
					ICEEditorLayout.Button( "Observe Photon Animator View", "", ICEEditorStyle.ButtonExtraLarge );
					ICEEditorLayout.Button( "Observe Photon Creature View", "", ICEEditorStyle.ButtonExtraLarge );
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
