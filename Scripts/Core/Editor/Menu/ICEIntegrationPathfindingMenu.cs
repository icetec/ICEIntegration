// ##############################################################################
//
// ICEIntegrationPathfindingMenu.cs
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
	public class ICEIntegrationPathfindingMenu : MonoBehaviour {


		// ADD PATHFINDING ADAPTER
		[MenuItem ("ICE/ICE Integration/Pathfinding/Add Pathfinding Adapters", false, 8022 )]
		static void AddAstarAdapter(){

			ICE.Creatures.ICECreatureControl[] _creatures = GameObject.FindObjectsOfType<ICE.Creatures.ICECreatureControl>();
			if( _creatures != null )
			{
				foreach( ICE.Creatures.ICECreatureControl _creature in _creatures )
				{
					if( _creature.GetComponent<ICEWorldPathfindingAdapter>() == null )
					{
						_creature.gameObject.AddComponent<ICEWorldPathfindingAdapter>();
						_creature.Creature.Move.MotionControl = ICE.Creatures.EnumTypes.MotionControlType.CUSTOM;
					}
				}
			}

		}

		[MenuItem ( "ICE/ICE Integration/Pathfinding/Add Pathfinding Adapters", true)]
		static bool ValidateAddAstarAdapter(){
			#if ICE_ASTAR && ICE_CC
			return true;
			#else
			return false;
			#endif
		}

		// REMOVE PATHFINDING ADAPTER
		[MenuItem ("ICE/ICE Integration/Pathfinding/Remove Pathfinding Adapters", false, 8022 )]
		static void RemoveAstarAdapter(){

			ICE.Creatures.ICECreatureControl[] _creatures = GameObject.FindObjectsOfType<ICE.Creatures.ICECreatureControl>();
			if( _creatures != null )
			{
				foreach( ICE.Creatures.ICECreatureControl _creature in _creatures )
				{
					if( _creature.GetComponent<ICEWorldPathfindingAdapter>() != null )
					{
						GameObject.DestroyImmediate( _creature.GetComponent<ICEWorldPathfindingAdapter>() );	
						_creature.Creature.Move.MotionControl = ICE.Creatures.EnumTypes.MotionControlType.INTERNAL;
					}
				}
			}
		}

		[MenuItem ( "ICE/ICE Integration/Pathfinding/Remove Pathfinding Adapters", true)]
		static bool ValidateRemoveAstarAdapter(){
			#if ICE_ASTAR && ICE_CC
			return true;
			#else
			return false;
			#endif
		}

	}
}