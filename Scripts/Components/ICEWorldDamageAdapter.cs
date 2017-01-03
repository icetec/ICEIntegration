// ##############################################################################
//
// ICEWorldDamageAdapter.cs
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

using ICE.Integration;
using ICE.Integration.Objects;

#if ICE_OPSIVE_TPC 
using Opsive.ThirdPersonController;
#endif

namespace ICE.Integration.Adapter
{
#if ICE_OPSIVE_TPC 
	public class ICEWorldDamageAdapter : Health 
	{
		protected ICEWorldEntity m_Entity = null;
		public ICEWorldEntity Entity{
			get{ return m_Entity = ( m_Entity == null ? ICEWorldEntity.GetWorldEntity( this.gameObject ) : m_Entity ); }
		}

		// IMPORTANT: this overrides the EntityDamageConverter.DoHandleDamage method with the 
		// customized damage method and allows to use the original damage handler of the asset.
		private DamageConverter _dc = new DamageConverter();

		/**/
		protected override void Awake()
		{
			base.Awake();

			if( Entity != null )
			{
				MaxHealth = Entity.Status.InitialDurability;
				SetHealthAmount( Entity.Status.Durability );
			}
		}


		public override void Damage( float _amount, Vector3 _position, Vector3 _force, float _radius, GameObject _attacker, GameObject _target )
		{
			if( Entity == null )
				return;

			_target = ( _target != null ? _target : gameObject );
			_attacker = ( _attacker != null ? _attacker : gameObject );

			Vector3 _direction = ( _attacker != null && _target != null ? _attacker.transform.position - _target.transform.position : _force );
			_direction.Normalize();

			Entity.AddDamage( _amount, _direction, _position, _attacker.transform, _force.magnitude );

			if( ! Entity.Status.IsDestroyed )
				base.Damage( _amount, _position, _force, _radius, _attacker, _target );
			else
				base.Die( _position, _force, _attacker );
		}
	}
#elif ICE_UFPS
	public class ICEWorldDamageAdapter : vp_DamageHandler {

		protected ICEWorldEntity m_Entity = null;
		public ICEWorldEntity Entity{
			get{ return m_Entity = ( m_Entity == null ? ICEWorldEntity.GetWorldEntity( this.gameObject ) : m_Entity ); }
		}

		// IMPORTANT: this overrides the EntityDamageConverter.DoHandleDamage method with the 
		// customized damage method and allows to use the original damage handler of the asset.
		private DamageConverter _dc = new DamageConverter();

		protected override void Awake()
		{
			if( Entity != null )
			{
				MaxHealth = Entity.Status.InitialDurability;
				CurrentHealth = Entity.Status.Durability;
			}

			base.Awake();
		}

		public void Update()
		{
			if( Entity == null )
				return;

			CurrentHealth = Entity.Status.Durability;

			if( Entity.Status.IsDestroyed )
				Die();
		}

		public override void Damage( float _damage ){
			Damage( new vp_DamageInfo( _damage, null ) );
		}

		public override void Damage( vp_DamageInfo damageInfo )
		{
			if( Entity == null )
				return;

			Transform _source = damageInfo.Source;
			Transform _original_source = ( damageInfo.OriginalSource != null ? damageInfo.OriginalSource : damageInfo.Source );

			Vector3 _position = ( _source != null ? _source.position : transform.position ); 
			Vector3 _direction = ( _original_source != null ? _original_source.position - transform.position : ( _source != null ? _source.forward : Vector3.zero ) );

			Entity.AddDamage( damageInfo.Damage, _direction, _position, _original_source , 0 );

			if( ! Entity.Status.IsDestroyed )
				base.Damage( damageInfo );
		}

		protected override void Reset()
		{
			if( Entity == null )
				return;

			Entity.Reset();

			base.Reset();

		}

		public override void Die()
		{
			if( Entity != null )
			{
				if( Entity.Status.IsDestroyed )
					base.Die();				
			}
		}
	}
#elif ICE_RFPSP
	public class ICEWorldDamageAdapter : ICEWorldBehaviour{}
#elif ICE_UNITZ
	[RequireComponent(typeof(ICEWorldEntity))]
	public class ICEWorldDamageAdapter : DamageManager {

		protected ICEWorldEntity m_Entity = null;
		public ICEWorldEntity Entity{
			get{ return m_Entity = ( m_Entity == null ? ICEWorldEntity.GetWorldEntity( this.gameObject ) : m_Entity ); }
		}

		// IMPORTANT: this overrides the EntityDamageConverter.DoHandleDamage method with the 
		// customized damage method and allows to use the original damage handler of the asset.
		private DamageConverter _dc = new DamageConverter();

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
#elif ICE_EASY_WEAPONS
	[RequireComponent(typeof(ICEWorldEntity))]
	public class ICEWorldDamageAdapter : ICEWorldBehaviour {

		protected ICEWorldEntity m_Entity = null;
		public ICEWorldEntity Entity{
			get{ return m_Entity = ( m_Entity == null ? ICEWorldEntity.GetWorldEntity( this.gameObject ) : m_Entity ); }
		}

		// IMPORTANT: this overrides the EntityDamageConverter.DoHandleDamage method with the 
		// customized damage method and allows to use the original damage handler of the asset.
		private DamageConverter _dc = new DamageConverter();

		public void ChangeHealth( float _damage )
		{
			if( Entity == null )
				return;

			Entity.ApplyDamage( _damage * (-1) );
		}
	}

#else
	public class ICEWorldDamageAdapter : ICEWorldBehaviour{}
#endif
}
