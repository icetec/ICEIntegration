﻿// ##############################################################################
//
// ice_editor_defines.cs
// Version 1.4.0
//
// Copyrights © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.icecreaturecontrol.com
// mailto:support@icecreaturecontrol.com
//
// Unity Asset Store End User License Agreement (EULA)
// http://unity3d.com/legal/as_terms
//
// This script based on the Custom Defines Manager script of Jon Kenkel (nonathaj) 
// The original source is available under Creative Commons Attribution Share Alike. 
// http://wiki.unity3d.com/index.php/Custom_Defines_Manager
// Author: Jon Kenkel (nonathaj)
// Created: 1/23/2016
//
// ##############################################################################

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ICE;
using ICE.World;

namespace ICE.Integration.Menus
{
	public class ICEIntegrationTools : AssetPostprocessor
	{
		/*
		static ICEIntegrationTools()
		{
			ValidateDefines();
		}*/

		/// <summary>
		/// Custom ICE asset defines to add based on the file and keyword to detect the asset and the desired platforms
		/// </summary>
		private static List<AssetDefine> m_CustomDefines = new List<AssetDefine>
		{
			/*new AssetDefine("ICEWorldBehaviour.cs", "", true, null, "ICE" ),*/
			new AssetDefine("ICECreatureControl.cs", "", true, null, "ICE_CC" ),

			// DAMAGE HANDLING
			new AssetDefine("Health.cs", "namespace Opsive.ThirdPersonController", true, null, "ICE_OPSIVE_TPC" ),
			new AssetDefine("vp_DamageHandler.cs", "", true, null, "ICE_UFPS" ),
			new AssetDefine("CharacterDamage.cs", "", true, null, "ICE_RFPSP" ),
			new AssetDefine("DamageManager.cs", "UnitZ", true, null, "ICE_UNITZ" ),
			new AssetDefine("Health.cs", "Author: MutantGopher", true, null, "ICE_EASY_WEAPONS" ),
			new AssetDefine("GenericVitals.cs", "namespace UltimateSurvival", true, null, "ICE_ULTIMATE_SURVIVAL" ),
			new AssetDefine("Entity.cs", "", true, null, "ICE_UMMORPG" ),
			new AssetDefine("vCharacter.cs", "namespace Invector", true, null, "ICE_INVECTOR_TPC" ),

			// NETWORKING
			new AssetDefine("PhotonNetwork.cs", "", true, null, "ICE_PUN" ),
			new AssetDefine("vp_MPMaster.cs", "", true, null, "ICE_UFPS_MP" ),

			// PATHFINDING
			new AssetDefine("AstarPath.cs", "", true, null, "ICE_ASTAR" ),
			new AssetDefine("ApexShared.dll", "", true, null, "ICE_APEX" ),

			// ENVIRONMENT
			new AssetDefine("ICEEnvironment.cs", "", true, null, "ICE_ENV" ),
			new AssetDefine("UniStormWeatherSystem_C.cs", "", true, null, "ICE_UNISTORM" ),
			new AssetDefine("TenkokuModule.cs", "", true, null, "ICE_TENKOKU" ),
			new AssetDefine("WeatherMakerScript.cs", "", true, null, "ICE_WEATHER_MAKER" ),
			new AssetDefine("EnviroSky.cs", "", true, null, "ICE_ENVIRO" ),

			// OTHER
			new AssetDefine("PlayMakerEditor.dll", "", true, null, "ICE_PLAYMAKER" )
		};

		private struct AssetDefine
		{
			public readonly string AssetDetectionFile;              //the file used to detect if the asset exists
			public readonly string[] AssetDefines;                  //series of defines for this asset
			public readonly BuildTargetGroup[] DefinePlatforms;     //platform this define will be used for (null is all platforms)
			public readonly string AssetDetectionKeywords; 
			public readonly bool AssetIsReady; 

			public AssetDefine(string _file, string _keywords, bool _ready, BuildTargetGroup[] _platforms, params string[] _defines)
			{
				AssetDetectionFile = _file;
				AssetDetectionKeywords = _keywords;
				DefinePlatforms = _platforms;
				AssetDefines = _defines;
				AssetIsReady = _ready;
			}

			public bool IsValid { get { return AssetDetectionFile != null && AssetDefines != null; } }
			public static AssetDefine Invalid = new AssetDefine(null, null, false, null, null);

			public void RemoveAllDefines()
			{
				foreach( string _define in AssetDefines )
					RemoveCompileDefine( _define, DefinePlatforms );
			}

			public void AddAllDefines()
			{
				foreach( string _define in AssetDefines )
					AddCompileDefine(_define, DefinePlatforms);
			}
		}


		public static void ValidateDefines()
		{
			foreach( AssetDefine _define in m_CustomDefines )
			{
				string[] _file_codes = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(_define.AssetDetectionFile));
				foreach( string _code in _file_codes )
				{
					string _path = AssetDatabase.GUIDToAssetPath( _code );
					string _filename = Path.GetFileName( _path );
					if( _filename == _define.AssetDetectionFile && _define.IsValid )
					{
						bool _is_valid = true;
						if( ! string.IsNullOrEmpty( _define.AssetDetectionKeywords ) )
						{
							string _text = File.ReadAllText( _path );
							if( ! _text.Contains( _define.AssetDetectionKeywords ) || ! _define.AssetIsReady )
								_is_valid = false;
						}
			
						if( _is_valid )
							_define.AddAllDefines();
					}
				}
			}		
		}

		private static void OnPostprocessAllAssets( string[] _imported_assets, string[] _deleted_assets, string[] _moved_assets, string[] _moved_from_asset_paths )
		{
			foreach( string _deleted_file in _deleted_assets )
			{
				AssetDefine _define = AssetDefine.Invalid;
				{
					string _file = Path.GetFileName( _deleted_file );
					foreach( AssetDefine _custom_define in m_CustomDefines )
					{
						if( _custom_define.AssetDetectionFile == _file )
						{
							_define = _custom_define;
							break;
						}
					}
				}

				if( _define.IsValid )
					_define.RemoveAllDefines();
			}
		}
			
		/// <summary>
		/// Attempts to add a new #define constant to the Player Settings
		/// </summary>
		/// <param name="newDefineCompileConstant">constant to attempt to define</param>
		/// <param name="targetGroups">platforms to add this for (null will add to all platforms)</param>
		public static void AddCompileDefine( string _new_define_compile_constant, BuildTargetGroup[] _target_groups = null )
		{
			ICEDebug.LogInfo( "Found '" + _new_define_compile_constant + "'" );

			if( _target_groups == null )
				_target_groups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));

			foreach( BuildTargetGroup _group in _target_groups )
			{
				//the unknown group does not have any constants location
				if( _group == BuildTargetGroup.Unknown )       
					continue;

				string _defines = PlayerSettings.GetScriptingDefineSymbolsForGroup( _group );

				if( ! _defines.Contains( _new_define_compile_constant ) )
				{
					ICEDebug.LogInfo( "Add new define '" + _new_define_compile_constant + "' for BuildTargetGroup '" + _group.ToString() + "'" );

					//if the list is empty, we don't need to append a semicolon first
					if( _defines.Length > 0 )         
						_defines += ";";


					_defines += _new_define_compile_constant;

					PlayerSettings.SetScriptingDefineSymbolsForGroup( _group, _defines );

				}
			}
		}

		/// <summary>
		/// Attempts to remove a #define constant from the Player Settings
		/// </summary>
		/// <param name="defineCompileConstant"></param>
		/// <param name="targetGroups"></param>
		public static void RemoveCompileDefine( string _define_compile_constant, BuildTargetGroup[] _target_groups = null )
		{
			if( _target_groups == null )
				_target_groups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));

			foreach( BuildTargetGroup _group in _target_groups )
			{
				string _defines = PlayerSettings.GetScriptingDefineSymbolsForGroup( _group );

				ICEDebug.LogInfo( "Remove custom define '" + _define_compile_constant + "' define for BuildTargetGroup '" + _group.ToString() + "'" );

				int _index = _defines.IndexOf( _define_compile_constant );
				if( _index < 0 )
					continue; //this target does not contain the define
				else if( _index > 0 )
					_index -= 1; //include the semicolon before the define
				//else we will remove the semicolon after the define

				//Remove the word and it's semicolon, or just the word (if listed last in defines)
				int _length_to_remove = Math.Min( _define_compile_constant.Length + 1, _defines.Length - _index );

				//remove the constant and it's associated semicolon (if necessary)
				_defines = _defines.Remove( _index, _length_to_remove );

				PlayerSettings.SetScriptingDefineSymbolsForGroup( _group, _defines );

			}
		}
	}
}
