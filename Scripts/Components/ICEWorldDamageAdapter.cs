// ##############################################################################
//
// ICEWorldDamageAdapter.cs
// Version 1.4.0
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
#elif ICE_INVECTOR_TPC
using Invector;
#elif ICE_ULTIMATE_SURVIVAL
using UltimateSurvival;
#endif

namespace ICE.Integration.Adapter
{
#if ICE_INVECTOR_TPC 
	public class ICEWorldDamageAdapter : vCharacter 
	{
		protected ICEWorldEntity m_Entity = null;
		public ICEWorldEntity Entity{
			get{ return m_Entity = ( m_Entity == null ? ICEWorldEntity.GetWorldEntity( this.gameObject ) : m_Entity ); }
		}

		// IMPORTANT: this overrides the EntityDamageConverter.DoHandleDamage method with the 
		// customized damage method and allows to use the original damage handler of the asset.
		private DamageConverter _dc = new DamageConverter();

		protected Rigidbody m_AttachedRigidbody = null;
		public Rigidbody AttachedRigidbody{
			get{ return m_AttachedRigidbody = ( m_AttachedRigidbody == null ? this.gameObject.GetComponent<Rigidbody>() : m_AttachedRigidbody ); }
		}

		void Start()
		{
			currentHealth = Entity.Durability;
		}
		public void Update()
		{
			if( Entity != null )
				currentHealth = Entity.Durability;
		}

		public override void TakeDamage( Damage _damage, bool _hit_reaction = true )
		{
			if( _damage == null )
				return;

			Transform _target = ( _damage.receiver != null ? _damage.receiver : transform );
			Transform _attacker = _damage.sender;

			Vector3 _hit_point = _damage.hitPosition;
			Vector3 _relative_point = transform.position;
			_relative_point.y = _hit_point.y;
			Vector3 _force = _damage.value * ( _relative_point - _hit_point );

			Vector3 _direction = ( _attacker != null && _target != null ? _attacker.position - _target.position : _force );
			_direction.Normalize();

			Entity.AddDamage( _damage.value , _direction, _hit_point, _attacker.transform, _force.magnitude );
			
			currentHealth = Entity.Durability;

			if( _damage.activeRagdoll )
				EnableRagdoll();

			if( AttachedRigidbody != null )
				AttachedRigidbody.AddForce( _force, ForceMode.Impulse );
		}
	}

#elif ICE_OPSIVE_TPC 
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

		public bool UseUFPSDamageHandling = false;

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

			if( UseUFPSDamageHandling )
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

			if( UseUFPSDamageHandling && ! Entity.Status.IsDestroyed )
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
			if( Entity == null )
				return;

			if( UseUFPSDamageHandling && Entity.Status.IsDestroyed )
				base.Die();	
		}

		public override void DieBySources( Transform[] sourceAndOriginalSource )
		{
			if( Entity == null )
				return;
			
			if( sourceAndOriginalSource.Length != 2 )
			{
				ICEDebug.LogWarning( "Warning (" + this + ") 'DieBySources' argument must contain 2 transforms." );
				return;
			}

			Transform _source = sourceAndOriginalSource[0];
			Transform _original_source = ( sourceAndOriginalSource[1] != null ? sourceAndOriginalSource[1] : _source );

			Vector3 _position = ( _source != null ? _source.position : transform.position ); 
			Vector3 _direction = ( _original_source != null ? _original_source.position - transform.position : ( _source != null ? _source.forward : Vector3.zero ) );

			Entity.AddDamage( 100, _direction, _position, _original_source , 0 );
			/*
			 // ToDo: could check for an entity to get the damage value ...
			Source = sourceAndOriginalSource[0];
			OriginalSource = sourceAndOriginalSource[1];
			*/

			if( UseUFPSDamageHandling )
				base.DieBySources( sourceAndOriginalSource );	
		}
	}
#elif ICE_RFPSP
	public class ICEWorldDamageAdapter : ICEWorldBehaviour
	{
		// IMPORTANT: this overrides the EntityDamageConverter.DoHandleDamage method with the 
		// customized damage method and allows to use the original damage handler of the asset.
		private DamageConverter _dc = new DamageConverter();
	}
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
#elif ICE_ULTIMATE_SURVIVAL

	[RequireComponent(typeof(ICEWorldEntity))]
	public class ICEWorldDamageAdapter : EntityBehaviour, IDamageable 
	{
		protected ICEWorldEntity m_AttachedEntity = null;
		public ICEWorldEntity AttachedEntity{
			get{ return m_AttachedEntity = ( m_AttachedEntity == null ? ICEWorldEntity.GetWorldEntity( this.gameObject ) : m_AttachedEntity ); }
		}

		private EntityEventHandler m_ParentEntity;

		// IMPORTANT: this overrides the EntityDamageConverter.DoHandleDamage method with the 
		// customized damage method and allows to use the original damage handler of the asset.
		private DamageConverter _dc = new DamageConverter();


		private void Awake()
		{
			/*
			m_ParentEntity = GetComponentInParent<EntityEventHandler>();
			if(!m_ParentEntity)
			{
				Debug.LogErrorFormat(this, "[This HitBox is not under an entity, like a player, animal, etc, it has no purpose.", name);
				enabled = false;
				return;
			}*/
		}

		private void Start() 
		{
			//Entity.ChangeHealth.SetTryer( TryChangeHealth ); 
		}

		public void ReceiveDamage( HealthEventData _damage_data )
		{
			if( ! enabled )
				return;
			
			Transform _attacker = null;
			if( _damage_data.Damager != null )
				_attacker = _damage_data.Damager.transform;

			AttachedEntity.AddDamage( _damage_data.Delta * (-1) , _damage_data.HitDirection , _damage_data.HitPoint , _attacker , _damage_data.HitImpulse );

		}
	}
	/*
	[RequireComponent(typeof(ICEWorldEntity))]
	public class ICEWorldDamageAdapter : GenericVitals {

		protected ICEWorldEntity m_AttachedEntity = null;
		public ICEWorldEntity AttachedEntity{
			get{ return m_AttachedEntity = ( m_AttachedEntity == null ? ICEWorldEntity.GetWorldEntity( this.gameObject ) : m_AttachedEntity ); }
		}

		// IMPORTANT: this overrides the EntityDamageConverter.DoHandleDamage method with the 
		// customized damage method and allows to use the original damage handler of the asset.
		private DamageConverter _dc = new DamageConverter();

	
		private void Awake()
		{
			if( Entity == null )
			{
				Debug.Log( "UltimateSurvival.EntityEventHandler is missing!" );
				return;
			}
				
			Entity.ChangeHealth.SetTryer( Try_ChangeHealth );
			Entity.Land.AddListener(On_Landed);
			Entity.Health.AddChangeListener(OnChanged_Health);
		}

		private void OnChanged_Health()
		{
			Debug.Log( "OnChanged_Health" );

			float delta = Entity.Health.Get() - Entity.Health.GetLastValue();
			if(delta < 0f)
			{
				
			}
		}

		private void On_Landed(float landSpeed)
		{
			Debug.Log( "On_Landed" );
		}


		protected override bool Try_ChangeHealth( HealthEventData _health_event_data )
		{
			if( _health_event_data == null || Entity == null || AttachedEntity == null )
				return false;

			Transform _attacker = null;
			if( _health_event_data.Damager != null )
				_attacker = _health_event_data.Damager.transform;

			AttachedEntity.AddDamage( _health_event_data.Delta * (-1) , _health_event_data.HitDirection , _health_event_data.HitPoint , null , 0 );

			return true;
		}
	}*/
#elif ICE_UMMORPG

	using UnityEngine.Networking;

	[RequireComponent(typeof(ICEWorldEntity))]
	public class ICEWorldDamageAdapter : Entity {

		protected ICEWorldEntity m_AttachedEntity = null;
		public ICEWorldEntity AttachedEntity{
			get{ return m_AttachedEntity = ( m_AttachedEntity == null ? ICEWorldEntity.GetWorldEntity( this.gameObject ) : m_AttachedEntity ); }
		}

		public override int healthMax { get { return (int)AttachedEntity.Status.InitialDurabilityMax; } }

		[SerializeField] int _manaMax = 1;
		public override int manaMax { get { return _manaMax; } }

		[SerializeField] int _damage = 2;
		public override int damage { get { return _damage; } }

		[SerializeField] int _defense = 1;
		public override int defense { get { return _defense; } }

		[SerializeField, Range(0, 1)] float _blockChance = 0;
		public override float blockChance { get { return _blockChance; } }

		[SerializeField, Range(0, 1)] float _criticalChance = 0;
		public override float criticalChance { get { return _criticalChance; } }

		[SerializeField, Range(0, 1)] float moveProbability = 0.1f; // chance per second
		[SerializeField] float moveDistance = 10;


		[Server]
		protected override string UpdateServer() {
			return "IDLE";
		}

		// finite state machine - client ///////////////////////////////////////////
		[Client]
		protected override void UpdateClient() {

		}

		// aggro ///////////////////////////////////////////////////////////////////
		// this function is called by the AggroArea (if any) on clients and server
		[ServerCallback]
		public override void OnAggro(Entity entity) {

			Debug.Log( "AGGRO");
		}

		// skills //////////////////////////////////////////////////////////////////
		// monsters always have a weapon
		public override bool HasCastWeapon() { return true; }

		// monsters can only attack players
		public override bool CanAttackType(System.Type type) {
			return type == typeof(Player);
		}
	}
#else
	public class ICEWorldDamageAdapter : ICEWorldBehaviour{}
#endif
}
