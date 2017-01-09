// ##############################################################################
//
// ICEIntegrationEnvironmentMenu.cs
// Version 1.3.6
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
	public class ICEIntegrationEnvironmentMenu : MonoBehaviour {


		// A* PATHFINDING ADAPTER
		[MenuItem ("ICE/ICE Integration/Environment/Add Environment Adapter", false, 8022 )]
		static void AddAstarAdapter(){

			ICE.Creatures.ICECreatureControl[] _creatures = GameObject.FindObjectsOfType<ICE.Creatures.ICECreatureControl>();
			if( _creatures != null )
			{
				foreach( ICE.Creatures.ICECreatureControl _creature in _creatures )
				{
					if( _creature.GetComponent<ICEWorldPathfindingAdapter>() == null )
						_creature.gameObject.AddComponent<ICEWorldPathfindingAdapter>();						
				}
			}

		}

		[MenuItem ( "ICE/ICE Integration/Environment/Add Environment Adapter", true)]
		static bool ValidateAddAstarAdapter(){
			#if ICE_UNISTORM
			return true;
			#else
			return false;
			#endif
		}

		[MenuItem ("ICE/ICE Integration/Environment/Remove Environment Adapter", false, 8022 )]
		static void RemoveUniStormAdapter(){

			ICE.Creatures.ICECreatureControl[] _creatures = GameObject.FindObjectsOfType<ICE.Creatures.ICECreatureControl>();
			if( _creatures != null )
			{
				foreach( ICE.Creatures.ICECreatureControl _creature in _creatures )
				{
					if( _creature.GetComponent<ICEWorldPathfindingAdapter>() != null )
						GameObject.DestroyImmediate( _creature.GetComponent<ICEWorldPathfindingAdapter>() );						
				}
			}

		}

		[MenuItem ( "ICE/ICE Integration/Environment/Remove Environment Adapter", true)]
		static bool ValidateRemoveUniStormAdapter(){
			#if ICE_UNISTORM
			return true;
			#else
			return false;
			#endif
		}

	}
}