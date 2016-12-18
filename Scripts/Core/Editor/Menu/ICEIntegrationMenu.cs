// ##############################################################################
//
// ICEIntegrationMenu.cs
// Version 1.3.5
//
// Copyrights © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.icecreaturecontrol.com
// mailto:support@icecreaturecontrol.com
// 
// Unity Asset Store End User License Agreement (EULA)
// http://unity3d.com/legal/as_terms
//
// ##############################################################################

using UnityEditor;
using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.Collections;

using ICE;
using ICE.World;
using ICE.World.EditorUtilities;
using ICE.World.Windows;

using ICE.Integration.Adapter;


namespace ICE.Integration.Menus
{
	public class ICEIntegrationMenu : MonoBehaviour {

		// GENERAL
		[MenuItem ("ICE/ICE Integration/Identify Supported Assets", false, 8001 )]
		static void IdentifySupportedAssets (){
			ICEIntegrationTools.ValidateDefines();
		}

		[MenuItem ("ICE/ICE Integration/Add Integration Adapters", false, 8001 )]
		static void AddIntegrationAdapters(){

			ICEWorldEntity[] _entities = GameObject.FindObjectsOfType<ICEWorldEntity>();
			if( _entities != null )
			{
				foreach( ICEWorldEntity _entity in _entities )
				{
					if( _entity.GetComponent<ICEWorldAdapter>() == null )
						_entity.gameObject.AddComponent<ICEWorldAdapter>();						
				}
			}

		}

		[MenuItem ("ICE/ICE Integration/Remove Integration Adapters", false, 8001 )]
		static void RemoveIntegrationAdapters(){

			ICEWorldEntity[] _entities = GameObject.FindObjectsOfType<ICEWorldEntity>();
			if( _entities != null )
			{
				foreach( ICEWorldEntity _entity in _entities )
				{
					if( _entity.GetComponent<ICEWorldAdapter>() != null )
						GameObject.DestroyImmediate( _entity.GetComponent<ICEWorldAdapter>() );						
				}
			}

		}

		// UNITZ
		[MenuItem ( "ICE/ICE Integration/UnitZ/Adapt UnitZ Scripts", false, 8112 )]
		static void AdaptUNITZScripts() 
		{
			{
				string _file = "Assets/UnitZ/Scripts/Damage/DamageManager.cs";
				string _text = File.ReadAllText( _file );
				if( ! _text.Contains( "MODIFIED BY ICE" ) )
				{
					string _search_str = "public void ApplyDamage";
					string _replace_str = "// MODIFIED BY ICE - virtual modifier was added\n" +
						"\tpublic virtual void ApplyDamage";

					_text = _text.Replace( _search_str, _replace_str );

					Debug.Log( "Insert virtual modifier for ApplyDamage in '" + _file + "'"  ); 
					File.WriteAllText( _file, _text );
				}
				else
					Debug.Log( "Virtual modifier for ApplyDamage already exists in '" + _file + "'"  );
			}
		}

		[MenuItem ( "ICE/ICE Integration/UnitZ/Adapt UnitZ Scripts", true)]
		static bool ValidateUNITZ(){
			#if UNITZ
			return true;
			#else
			return false;
			#endif
		}
	}
}