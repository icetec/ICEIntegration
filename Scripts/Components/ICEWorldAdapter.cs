// ##############################################################################
//
// ICEWorldAdapter.cs
// Version 1.3.5
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
using System.Collections;

using ICE;
using ICE.World;
using ICE.World.Objects;
using ICE.World.Utilities;
using ICE.World.EnumTypes;

namespace ICE.Integration.Adapter
{

#if UNITZ
	[RequireComponent(typeof(ICEWorldEntity))]
	public class ICEWorldAdapter : DamageManager {

		protected ICEWorldEntity m_Entity = null;
		public ICEWorldEntity Entity{
			get{ return m_Entity = ( m_Entity == null ? ICEWorldEntity.GetWorldEntity( this.gameObject ) : m_Entity ); }
		}

		public override void ApplyDamage( int _damage, Vector3 _direction, string _attacker_id, string _team )
		{
			if( Entity == null )
				return;

			Vector3 _position = transform.position;

			Entity.AddDamage( _damage, _direction, _position, null , 0 );

			if( ! Entity.Status.IsDestroyed )
				base.ApplyDamage( _damage, _direction, _attacker_id, _team );
		}

	}
#else
		public class ICEWorldAdapter : MonoBehaviour{}
#endif
}
