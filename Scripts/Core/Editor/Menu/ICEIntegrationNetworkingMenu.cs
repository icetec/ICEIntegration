// ##############################################################################
//
// ICEIntegrationNetworkingMenu.cs
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
	public class ICEIntegrationNetworkingMenu : MonoBehaviour {


		// ADD NETWORKING ADAPTER
		[MenuItem ("ICE/ICE Integration/Networking/Add Networking Adapters", false, 8022 )]
		static void AddNetworkingAdapter(){

			// add the ICEWorldNetworkManager to an existing register object
			ICEWorldRegister _register = GameObject.FindObjectOfType<ICEWorldRegister>();
			if( _register != null && _register.GetComponent<ICEWorldNetworkManager>() == null )
				_register.gameObject.AddComponent<ICEWorldNetworkManager>();
			
			// add the ICEWorldNetworkingAdapter to all entities
			ICEWorldEntity[] _entities = GameObject.FindObjectsOfType<ICEWorldEntity>();
			if( _entities != null )
			{
				foreach( ICEWorldEntity _entity in _entities )
				{
					if( _entity.GetComponent<ICEWorldNetworkingAdapter>() == null )
						_entity.gameObject.AddComponent<ICEWorldNetworkingAdapter>();
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
		[MenuItem ("ICE/ICE Integration/Networking/Remove Networking Adapters", false, 8022 )]
		static void RemoveNetworkingAdapter(){

			// remove the ICEWorldNetworkManager from an existing register object
			ICEWorldRegister _register = GameObject.FindObjectOfType<ICEWorldRegister>();
			if( _register != null && _register.GetComponent<ICEWorldNetworkManager>() != null )
				GameObject.DestroyImmediate( _register.GetComponent<ICEWorldNetworkManager>() );

			// remove the ICEWorldNetworkingAdapter from all entities
			ICEWorldEntity[] _entities = GameObject.FindObjectsOfType<ICEWorldEntity>();
			if( _entities != null )
			{
				foreach( ICEWorldEntity _entity in _entities )
				{
					if( _entity.GetComponent<ICEWorldNetworkingAdapter>() != null )
						GameObject.DestroyImmediate( _entity.GetComponent<ICEWorldNetworkingAdapter>() );
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