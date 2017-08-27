// ##############################################################################
//
// ICEUnityNetworkSpawnerEditor.cs
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
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;

using ICE;
using ICE.World;
using ICE.World.EditorUtilities;

namespace ICE.Integration.Adapter
{
	[CustomEditor(typeof(ICEUnityNetworkSpawner))]
	public class ICEUnityNetworkSpawnerEditor : ICEWorldBehaviourEditor {

		public void DrawUnityAdapterContent( ICEWorldNetworkSpawner _adapter )
		{
			NetworkIdentity m_NetworkIdentity = _adapter.GetComponent<NetworkIdentity>(); 
			EditorGUI.BeginDisabledGroup( m_NetworkIdentity != null );
			GUI.backgroundColor = ( m_NetworkIdentity == null ? Color.yellow : Color.green );			
			if( ICEEditorLayout.Button( "Network Identity", "", ICEEditorStyle.ButtonExtraLarge ) )
				m_NetworkIdentity = _adapter.gameObject.AddComponent<NetworkIdentity>();
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			EditorGUI.EndDisabledGroup();

			_adapter.AddViewIDToName = ICEEditorLayout.Toggle( "Add ViewID to name", "", _adapter.AddViewIDToName, "" );


			EditorGUILayout.Separator();

		}
	}
}
