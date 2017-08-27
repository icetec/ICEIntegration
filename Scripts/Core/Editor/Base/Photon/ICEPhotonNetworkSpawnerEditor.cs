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


#if ICE_PUN

#endif

namespace ICE.Integration.Adapter
{
#if ICE_PUN
	[CustomEditor(typeof(ICEPhotonNetworkSpawner))]
	public class ICEPhotonNetworkSpawnerEditor : ICEWorldBehaviourEditor {

		public void DrawPhotonAdapterContent( ICEWorldNetworkSpawner _adapter )
		{
			PhotonView m_PhotonView = _adapter.GetComponent<PhotonView>(); 
			EditorGUI.BeginDisabledGroup( m_PhotonView != null );
			GUI.backgroundColor = ( m_PhotonView == null ? Color.yellow : Color.green );			
			if( ICEEditorLayout.Button( "Photon View", "", ICEEditorStyle.ButtonExtraLarge ) )
				m_PhotonView = _adapter.gameObject.AddComponent<PhotonView>();
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			EditorGUI.EndDisabledGroup();

			_adapter.AddViewIDToName = ICEEditorLayout.Toggle( "Add ViewID to name", "", _adapter.AddViewIDToName, "" );

		#if ICE_UFPS_MP
			_adapter.WaitForUFPSMP = ICEEditorLayout.Toggle( "Wait For UFPS vp_MPMaster", "", _adapter.WaitForUFPSMP, "" );
		#endif
			EditorGUILayout.Separator();

		}
	}
#endif

}
