// ##############################################################################
//
// ICEIntegrationAstarMenu.cs
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
	public class ICEIntegrationAstarMenu : MonoBehaviour {


		// A* PATHFINDING ADAPTER
		[MenuItem ("ICE/ICE Integration/Add A* Pathfinding Adapters", false, 8033 )]
		static void AddAstarAdapter(){

			ICE.Creatures.ICECreatureControl[] _creatures = GameObject.FindObjectsOfType<ICE.Creatures.ICECreatureControl>();
			if( _creatures != null )
			{
				foreach( ICE.Creatures.ICECreatureControl _creature in _creatures )
				{
					if( _creature.GetComponent<ICECreatureAstarAdapter>() == null )
						_creature.gameObject.AddComponent<ICECreatureAstarAdapter>();						
				}
			}

		}

		[MenuItem ( "ICE/ICE Integration/Add A* Pathfinding Adapters", true)]
		static bool ValidateAddAstarAdapter(){
			#if ICE_ASTAR && ICE_CC
			return true;
			#else
			return false;
			#endif
		}

		[MenuItem ("ICE/ICE Integration/Remove A* Pathfinding Adapters", false, 8033 )]
		static void RemoveAstarAdapter(){

			ICE.Creatures.ICECreatureControl[] _creatures = GameObject.FindObjectsOfType<ICE.Creatures.ICECreatureControl>();
			if( _creatures != null )
			{
				foreach( ICE.Creatures.ICECreatureControl _creature in _creatures )
				{
					if( _creature.GetComponent<ICECreatureAstarAdapter>() != null )
						GameObject.DestroyImmediate( _creature.GetComponent<ICECreatureAstarAdapter>() );						
				}
			}

		}

		[MenuItem ( "ICE/ICE Integration/Remove A* Pathfinding Adapters", true)]
		static bool ValidateRemoveAstarAdapter(){
			#if ICE_ASTAR && ICE_CC
			return true;
			#else
			return false;
			#endif
		}

	}
}