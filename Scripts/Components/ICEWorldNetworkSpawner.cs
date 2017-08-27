// ##############################################################################
//
// ICEWorldNetworkSpawner.cs
// Version 1.4.0
//
// © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.ice-technologies.com
// mailto:support@ice-technologies.com
// 
// Unity Asset Store End User License Agreement (EULA)
// http://unity3d.com/legal/as_terms
//
// The ICECreatureAstarAdapter Script based on the default movement script AIPath.cs
// which comes with the A* Pathfinding Project and provides ICECreatureControl to 
// use the powerful navigation of the A* Pathfinding System.
//
// ##############################################################################

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ICE;
using ICE.World;

namespace ICE.Integration.Adapter
{
#if ICE_PUN
	public class ICEWorldNetworkSpawner : ICEPhotonNetworkSpawner
#else
	public class ICEWorldNetworkSpawner : ICEUnityNetworkSpawner  
#endif
	{
		private ICEWorldRegister m_Register = null;
		protected ICEWorldRegister Register{
			get{ return m_Register = ( m_Register == null ? ICEWorldRegister.Instance : m_Register ); }
		}

		public bool UseDeactivateSceneCreatures = true;
		public bool UseDeactivateScenePlayer = true;

		protected virtual void Awake()
		{
			DeactivateSceneCreatures();

			ICEWorldInfo.IsMultiplayer = true;
		}


		/// <summary>
		/// Raises the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			Register.OnRemoveObject += OnRemoveObject;
			Register.OnDestroyObject += OnDestroyObject;
			Register.OnInstantiateObject += OnInstantiateObject; 
		}

		/// <summary>
		/// Raises the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			Register.OnRemoveObject -= OnRemoveObject;
			Register.OnDestroyObject -= OnDestroyObject;
			Register.OnInstantiateObject -= OnInstantiateObject; 
		}


		/// <summary>
		/// deactivates any creature (local or remote) that are present
		/// in the scene upon Awake, because Creature objects may be placed in 
		/// the scene to facilitate updating their prefabs, however once 
		/// a multiplayer session starts only instantiated creature 
		/// are allowed
		/// </summary>
		protected virtual void DeactivateSceneCreatures()
		{
			#if ICE_CC
			if( UseDeactivateSceneCreatures )
			{
				ICE.Creatures.ICECreatureControl[] _creatures = FindObjectsOfType<ICE.Creatures.ICECreatureControl>();
				foreach( ICE.Creatures.ICECreatureControl _creature in _creatures )
					_creature.gameObject.SetActive( false );
			}

			if( UseDeactivateScenePlayer )
			{
				ICE.Creatures.ICECreaturePlayer[] _player = FindObjectsOfType<ICE.Creatures.ICECreaturePlayer>();
				foreach( ICE.Creatures.ICECreaturePlayer _creature in _player )
					_creature.gameObject.SetActive( false );
			}
			#endif
		}

	}
}
