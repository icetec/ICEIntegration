// ##############################################################################
//
// ICEWorldNetworkSpawnerEditor.cs
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

	[CustomEditor(typeof(ICEWorldNetworkSpawner))]
	public class ICEWorldNetworkSpawnerEditor : ICEWorldBehaviourEditor {

		public override void OnInspectorGUI()
		{
			ICEWorldNetworkSpawner _target = DrawMonoHeader<ICEWorldNetworkSpawner>();
			DrawAdapterContent( _target );
			DrawMonoFooter( _target );
		}

		public void DrawAdapterContent( ICEWorldNetworkSpawner _adapter )
		{
#if ICE_PUN && ICE_CC

			PhotonView m_PhotonView = _adapter.GetComponent<PhotonView>(); 

			EditorGUI.BeginDisabledGroup( m_PhotonView != null );
			GUI.backgroundColor = ( m_PhotonView == null ? Color.yellow : Color.green );			
			if( ICEEditorLayout.Button( "Photon View", "", ICEEditorStyle.ButtonExtraLarge ) )
				m_PhotonView = _adapter.gameObject.AddComponent<PhotonView>();
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			EditorGUI.EndDisabledGroup();

			//_adapter.AutoConnect = ICEEditorLayout.CheckButton( "Auto Connect", "", _adapter.AutoConnect, ICEEditorStyle.ButtonExtraLarge );


#else

			string _info = "This Adapter requires the Photon Unity Network and ICECreatureControl packages. " +
				"If both assets are correct installed please add 'ICE_PUN' and 'ICE_CC' to your custom defines.";
			EditorGUILayout.HelpBox( _info, MessageType.Info );
#endif

			EditorGUILayout.Separator();

		}
	}

}
