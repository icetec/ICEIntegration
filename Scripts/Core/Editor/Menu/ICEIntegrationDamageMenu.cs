// ##############################################################################
//
// ICEIntegrationDamageMenu.cs
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
	public class ICEIntegrationDamageMenu : MonoBehaviour {


		// ADD DAMAGE ADAPTERS
		[MenuItem ("ICE/ICE Integration/Damage/Add Damage Adapters", false, 8022 )]
		static void AddDamageAdapters(){

			ICEWorldEntity[] _entities = GameObject.FindObjectsOfType<ICEWorldEntity>();
			if( _entities != null )
			{
				foreach( ICEWorldEntity _entity in _entities )
				{
					if( _entity.EntityType != ICE.World.EnumTypes.EntityClassType.Player && _entity.GetComponent<ICEWorldDamageAdapter>() == null )
						_entity.gameObject.AddComponent<ICEWorldDamageAdapter>();						
				}
			}

		}

		[MenuItem ( "ICE/ICE Integration/Damage/Add Damage Adapters", true)]
		static bool ValidateAddDamageAdapters(){
			#if ICE_UNITZ || ICE_UFPS || ICE_OPSIVE_TPC || ICE_EASY_WEAPONS 
			return true;
			#else
			return false;
			#endif
		}

		// REMOVE DAMAGE ADAPTERS
		[MenuItem ("ICE/ICE Integration/Damage/Remove Damage Adapters", false, 8022 )]
		static void RemoveDamageAdapters(){

			ICEWorldEntity[] _entities = GameObject.FindObjectsOfType<ICEWorldEntity>();
			if( _entities != null )
			{
				foreach( ICEWorldEntity _entity in _entities )
				{
					if( _entity.GetComponent<ICEWorldDamageAdapter>() != null )
						GameObject.DestroyImmediate( _entity.GetComponent<ICEWorldDamageAdapter>() );						
				}
			}

		}

		[MenuItem ( "ICE/ICE Integration/Damage/Remove Damage Adapters", true)]
		static bool ValidateRemoveDamageAdapters(){
			#if ICE_UNITZ || ICE_UFPS || ICE_OPSIVE_TPC || ICE_EASY_WEAPONS  
			return true;
			#else
			return false;
			#endif
		}

		// UNITZ
		#if ICE_UNITZ
		[MenuItem ( "ICE/ICE Integration/Damage/UnitZ/Adapt UnitZ Scripts", false, 8033 )]
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
		/*
		[MenuItem ( "ICE/ICE Integration/UnitZ/Adapt UnitZ Scripts", true)]
		static bool ValidateUNITZ(){
			#if ICE_UNITZ
			return true;
			#else
			return false;
			#endif
		}*/
		#endif
	
		// RFPSP
		#if ICE_RFPSP
		[MenuItem ( "ICE/ICE Integration/Damage/RFPSP/Adapt RFPSP Scripts", false, 8033 )]
		static void AdaptRFPSPScripts() 
		{
			{
				string _text = File.ReadAllText("Assets/RFPSP/Scripts/Weapons/WeaponBehavior.cs");
				if( ! _text.Contains( "BEGIN ICE DAMAGE - MODIFIED BY ICE" ) )
				{
					int _start_pos = _text.IndexOf( "case 13://hit object is an NPC" );
					if( _start_pos > 0 )
					{
						int _end_pos = _text.IndexOf( "break;", _start_pos );
						if( _end_pos > _start_pos )
						{
							int _insert_pos = _end_pos;

							string _insert_str = "" +
								"\n\t\t\t// BEGIN ICE DAMAGE - MODIFIED BY ICE" +
								"\n\t\t\tif( hit.collider.gameObject.GetComponentInParent<ICE.World.ICEWorldEntity>() ){" +
								"\n\t\t\t\thit.collider.gameObject.GetComponentInParent<ICE.World.ICEWorldEntity>().AddDamage( damageAmt, directionArg, mainCamTransform.position, myTransform, 0 );" +
								"\n\t\t\t\tFPSPlayerComponent.UpdateHitTime();//used for hitmarker" +
								"\n\t\t\t}" +								
								"\n\t\t\t// END ICE DAMAGE - MODIFIED BY ICE" +
								"\n\t\t\t";
							_text = _text.Insert( _insert_pos, _insert_str );

							Debug.Log( "Insert ICE damage handling for layer 13 (NPCs) in Assets/RFPSP/Scripts/Weapons/WeaponBehavior.cs"  ); 
							File.WriteAllText( "Assets/RFPSP/Scripts/Weapons/WeaponBehavior.cs", _text );
						}
					}
				}
				else
					Debug.Log( "ICE damage handling for layer 13 (NPCs) already exists in Assets/RFPSP/Scripts/Weapons/WeaponBehavior.cs"  );
			}

			{
				string _text = File.ReadAllText("Assets/RFPSP/Scripts/Weapons/ArrowObject.cs");
				if( ! _text.Contains( "BEGIN ICE DAMAGE - MODIFIED BY ICE" ) )
				{
					int _start_pos = _text.IndexOf( "case 13://hit object is an NPC" );
					if( _start_pos > 0 )
					{
						int _end_pos = _text.IndexOf( "break;", _start_pos );
						if( _end_pos > _start_pos )
						{
							int _insert_pos = _end_pos;

							string _insert_str = "" +
								"\n\t\t\t\t// BEGIN ICE DAMAGE - MODIFIED BY ICE" +
								"\n\t\t\t\tif( hitCol.gameObject.GetComponentInParent<ICE.World.ICEWorldEntity>() ){" +
								"\n\t\t\t\t\thitCol.gameObject.GetComponentInParent<ICE.World.ICEWorldEntity>().AddDamage( damage + damageAddAmt, transform.forward,  Camera.main.transform.position, transform, 0 );" +
								"\n\t\t\t\t\tFPSPlayerComponent.UpdateHitTime();//used for hitmarker" +
								"\n\t\t\t\t}" +								
								"\n\t\t\t\t// END ICE DAMAGE - MODIFIED BY ICE" +
								"\n\t\t\t\t";
							_text = _text.Insert( _insert_pos, _insert_str );

							Debug.Log( "Insert ICE damage handling for layer 13 (NPCs) in Assets/RFPSP/Scripts/Weapons/ArrowObject.cs"  ); 
							File.WriteAllText( "Assets/RFPSP/Scripts/Weapons/ArrowObject.cs", _text );
						}
					}
				}
				else
					Debug.Log( "ICE damage handling for layer 13 (NPCs) already exists in Assets/RFPSP/Scripts/Weapons/ArrowObject.cs"  );
			}

			{
				string _text = File.ReadAllText("Assets/RFPSP/Scripts/Objects/!Destructibles/ExplosiveObject.cs");
				if( ! _text.Contains( "BEGIN ICE DAMAGE - MODIFIED BY ICE" ) )
				{
					int _start_pos = _text.IndexOf( "case 13://hit object is an NPC" );
					if( _start_pos > 0 )
					{
						int _end_pos = _text.IndexOf( "break;", _start_pos );
						if( _end_pos > _start_pos )
						{
							int _insert_pos = _end_pos;

							string _insert_str = "" +
								"\n\t\t\t\t\t\t\t// BEGIN ICE DAMAGE - MODIFIED BY ICE" +
								"\n\t\t\t\t\t\t\tif( hitCollider.gameObject.GetComponentInParent<ICE.World.ICEWorldEntity>() ){" +
								"\n\t\t\t\t\t\t\t\thitCollider.gameObject.GetComponentInParent<ICE.World.ICEWorldEntity>().AddDamage( explosionDamageAmt, Vector3.zero, myTransform.position, myTransform, 0 );" +
								"\n\t\t\t\t\t\t\t}" +								
								"\n\t\t\t\t\t\t\t// END ICE DAMAGE - MODIFIED BY ICE" +
								"\n\t\t\t\t\t\t\t";
							_text = _text.Insert( _insert_pos, _insert_str );

							Debug.Log( "Insert ICE damage handling for layer 13 (NPCs) in Assets/RFPSP/Scripts/Objects/!Destructibles/ExplosiveObject.cs"  ); 
							File.WriteAllText( "Assets/RFPSP/Scripts/Objects/!Destructibles/ExplosiveObject.cs", _text );
						}
					}
				}
				else
					Debug.Log( "ICE damage handling for layer 13 (NPCs) already exists in Assets/RFPSP/Scripts/Objects/!Destructibles/ExplosiveObject.cs"  );
			}

			{
				string _text = File.ReadAllText("Assets/RFPSP/Scripts/Objects/!Destructibles/MineExplosion.cs");
				if( ! _text.Contains( "BEGIN ICE DAMAGE - MODIFIED BY ICE" ) )
				{
					int _start_pos = _text.IndexOf( "case 13://hit object is an NPC" );
					if( _start_pos > 0 )
					{
						int _end_pos = _text.IndexOf( "break;", _start_pos );
						if( _end_pos > _start_pos )
						{
							int _insert_pos = _end_pos;

							string _insert_str = "" +
								"\n\t\t\t\t\t\t// BEGIN ICE DAMAGE - MODIFIED BY ICE" +
								"\n\t\t\t\t\t\tif( hitColliders[i].GetComponent<Collider>().GetComponentInParent<ICE.World.ICEWorldEntity>() ){" +
								"\n\t\t\t\t\t\t\thitColliders[i].GetComponent<Collider>().GetComponentInParent<ICE.World.ICEWorldEntity>().AddDamage( explosionDamage, Vector3.zero, myTransform.position, null, 0 );" +
								"\n\t\t\t\t\t\t}" +								
								"\n\t\t\t\t\t\t// END ICE DAMAGE - MODIFIED BY ICE" +
								"\n\t\t\t\t\t\t";
							_text = _text.Insert( _insert_pos, _insert_str );

							Debug.Log( "Insert ICE damage handling for layer 13 (NPCs) in Assets/RFPSP/Scripts/Objects/!Destructibles/MineExplosion.cs"  ); 
							File.WriteAllText( "Assets/RFPSP/Scripts/Objects/!Destructibles/MineExplosion.cs", _text );
						}
					}
				}
				else
					Debug.Log( "ICE damage handling for layer 13 (NPCs) already exists in Assets/RFPSP/Scripts/Objects/!Destructibles/MineExplosion.cs"  );
			}
		}

		[MenuItem ( "ICE/ICE Integration/RFPSP/Adapt RFPSP Scripts", true)]
		static bool ValidateAdaptRFPSPScripts() {
			#if ICE_RFPSP
			string _text = File.ReadAllText("Assets/RFPSP/Scripts/Weapons/WeaponBehavior.cs");
			if( ! _text.Contains( "BEGIN ICE DAMAGE - MODIFIED BY ICE" ) )
				return true;

			_text = File.ReadAllText("Assets/RFPSP/Scripts/Weapons/ArrowObject.cs");
			if( ! _text.Contains( "BEGIN ICE DAMAGE - MODIFIED BY ICE" ) )
				return true;

			_text = File.ReadAllText("Assets/RFPSP/Scripts/Objects/!Destructibles/ExplosiveObject.cs");
			if( ! _text.Contains( "BEGIN ICE DAMAGE - MODIFIED BY ICE" ) )
				return true;

			_text = File.ReadAllText("Assets/RFPSP/Scripts/Objects/!Destructibles/MineExplosion.cs");
			if( ! _text.Contains( "BEGIN ICE DAMAGE - MODIFIED BY ICE" ) )
				return true;

			return false;
			#else
			return false;
			#endif
		}
		#endif
	}
}