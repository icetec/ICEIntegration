// ##############################################################################
//
// ICEIntegrationNetworkingMenu.cs
// Version 1.3.7
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
	public class ICEIntegrationNetworkingMenu : MonoBehaviour {

		[MenuItem ( "ICE/ICE Integration/Networking/Create Network Manager", false, 8022 )]
		static void AddNetworkManager() 
		{
			ICEWorldNetworkManager _manager = ICEWorldNetworkManager.Instance as ICEWorldNetworkManager;

			if( _manager == null )
			{
				GameObject _object = new GameObject();
				_manager = _object.AddComponent<ICEWorldNetworkManager>();
				_object.name = "NetworkManager";
			}
		}
			
		[MenuItem ( "ICE/ICE Integration/Networking/Create Network Manager", true)]
		static bool ValidateAddNetworkManager() {
			if( ICEWorldNetworkManager.Instance == null )
				return true;
			else
				return false;
		}

		// ADD NETWORKING ADAPTER
		[MenuItem ("ICE/ICE Integration/Networking/Add Networking Adapters", false, 8033 )]
		static void AddNetworkingAdapter(){

			// add the ICEWorldNetworkManager to an existing register object
			ICEWorldRegister _register = GameObject.FindObjectOfType<ICEWorldRegister>();
			if( _register != null && _register.GetComponent<ICEWorldNetworkSpawner>() == null )
				_register.gameObject.AddComponent<ICEWorldNetworkSpawner>();
			
			// add the ICEWorldNetworkingAdapter to all entities
			ICEWorldEntity[] _entities = GameObject.FindObjectsOfType<ICEWorldEntity>();
			if( _entities != null )
			{
				foreach( ICEWorldEntity _entity in _entities )
				{
					if( ( _entity.GetComponent<ICEWorldNetworkView>() == null ) &&
						( _entity.EntityType != ICE.World.EnumTypes.EntityClassType.BodyPart ) &&
						( _entity.EntityType != ICE.World.EnumTypes.EntityClassType.Location ) &&
						( _entity.EntityType != ICE.World.EnumTypes.EntityClassType.Object ) )
					{
						_entity.gameObject.AddComponent<ICEWorldNetworkView>();
					}
				}
			}
		}

		[MenuItem ( "ICE/ICE Integration/Networking/Add Networking Adapters", true)]
		static bool ValidateAddNetworkingAdapter(){
			#if ICE_PUN && ICE_CC
			return true;
			#else
			return false;
			#endif
		}

		// REMOVE NETWORKING ADAPTER
		[MenuItem ("ICE/ICE Integration/Networking/Remove Networking Adapters", false, 8033 )]
		static void RemoveNetworkingAdapter(){

			// remove the ICEWorldNetworkManager from an existing register object
			ICEWorldRegister _register = GameObject.FindObjectOfType<ICEWorldRegister>();
			if( _register != null && _register.GetComponent<ICEWorldNetworkSpawner>() != null )
				GameObject.DestroyImmediate( _register.GetComponent<ICEWorldNetworkSpawner>() );

			// remove the ICEWorldNetworkingAdapter from all entities
			ICEWorldEntity[] _entities = GameObject.FindObjectsOfType<ICEWorldEntity>();
			if( _entities != null )
			{
				foreach( ICEWorldEntity _entity in _entities )
				{
					if( _entity.GetComponent<ICEWorldNetworkView>() != null )
						GameObject.DestroyImmediate( _entity.GetComponent<ICEWorldNetworkView>() );
				}
			}
		}

		[MenuItem ( "ICE/ICE Integration/Networking/Remove Networking Adapters", true)]
		static bool ValidateRemoveNetworkingAdapter(){
			#if ICE_PUN && ICE_CC
			return true;
			#else
			return false;
			#endif
		}

	}
}