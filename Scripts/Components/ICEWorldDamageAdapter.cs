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
using System.Collections.Generic;

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
#elif ICE_TPMC
using com.ootii.Actors.AnimationControllers;
using com.ootii.Actors.Attributes;
using com.ootii.Actors.Combat;
using com.ootii.Data.Serializers;
using com.ootii.Helpers;
using com.ootii.Messages;
using com.ootii.Utilities;
using com.ootii.Actors.LifeCores;
#endif

namespace ICE.Integration.Adapter
{
#if ICE_INVECTOR_TPC 
	#region INVECTOR TPC ADAPTER

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

	#endregion
#elif ICE_OPSIVE_TPC 
	#region OPSIVE TPC ADAPTER

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

	#endregion
#elif ICE_UFPS
	#region UFPS ADAPTER

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

			//Entity.Reset();

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

	#endregion
#elif ICE_RFPSP
	#region RFPSP ADAPTER

	public class ICEWorldDamageAdapter : ICEWorldBehaviour
	{
		// IMPORTANT: this overrides the EntityDamageConverter.DoHandleDamage method with the 
		// customized damage method and allows to use the original damage handler of the asset.
		private DamageConverter _dc = new DamageConverter();
	}

	#endregion
#elif ICE_UNITZ
	#region UNITZ ADAPTER

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

	#endregion
#elif ICE_EASY_WEAPONS
	#region EASY WEAPONS ADAPTER

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

	#endregion
#elif ICE_ULTIMATE_SURVIVAL
	#region ULTIMATE SURVIVAL ADAPTER
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
	#endregion
#elif ICE_UMMORPG
	#region UMMORPG ADAPTER

	using UnityEngine.Networking;

	[RequireComponent(typeof(ICEWorldEntity))]
	public class ICEWorldDamageAdapter : Entity {

		protected ICEWorldEntity m_AttachedEntity = null;
		public ICEWorldEntity AttachedEntity{
			get{ return m_AttachedEntity = ( m_AttachedEntity == null ? ICEWorldEntity.GetWorldEntity( this.gameObject ) : m_AttachedEntity ); }
		}

		public ICE.Creatures.ICECreatureControl AttachedCreature{
			get{ return AttachedEntity as ICE.Creatures.ICECreatureControl; }
		}
			
		public GameObject DamagePopupPrefab{
			get{ return base.damagePopupPrefab; }
			set{ base.damagePopupPrefab = value; }
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


		// IMPORTANT: this overrides the EntityDamageConverter.DoHandleDamage method with the 
		// customized damage method and allows to use the original damage handler of the asset.
		private DamageConverter _dc = new DamageConverter();

		void Start()
		{
		}

		void OnEnable()
		{
			base.health = (int)AttachedEntity.Status.Durability;
		}

		void OnDisable()
		{
			base.health = (int)AttachedEntity.Status.Durability;
		}

		void Update()
		{
			if( (int)AttachedEntity.Status.Durability > base.health )
			{
				float _damage = Mathf.Floor( AttachedEntity.Status.Durability - (float)base.health );
				AttachedEntity.ApplyDamage( _damage );
			}
			else
				base.health = (int)AttachedEntity.Status.Durability;

			if( AttachedCreature != null && AttachedCreature.ActiveTargetGameObject != null )
				target = AttachedCreature.ActiveTargetGameObject.GetComponentInParent<Entity>();
		}

		public override void OnStartServer() {
			// call Entity's OnStartServer
			base.OnStartServer();

			// all monsters should spawn with full health and mana
			health = healthMax;
			mana = manaMax;

		}

		[Server]
		protected override string UpdateServer() 
		{
			if( AttachedCreature != null )
				return AttachedCreature.BehaviourModeKey;
			else
				return "IDLE";
		}
			
		[Client]
		protected override void UpdateClient() 
		{
			if( AttachedCreature != null )
				AttachedCreature.BehaviourModeKey = state;
		}

		// aggro ///////////////////////////////////////////////////////////////////
		// this function is called by the AggroArea (if any) on clients and server
		[ServerCallback]
		public override void OnAggro(Entity entity) {

			//Debug.Log( "AGGRO");
		}

		[ClientCallback] 
		void LateUpdate() {


		}
			
		public override bool HasCastWeapon() { return true; }
		public override bool CanAttackType(System.Type type) { return true; }
	}


	#endregion
#elif ICE_TPMC
	#region TPMC ADAPTER
	public class ICEWorldDamageAdapter : ICEWorldBehaviour, IDamageable 
	{
		protected ICEWorldEntity m_AttachedEntity = null;
		public ICEWorldEntity AttachedEntity{
			get{ return m_AttachedEntity = ( m_AttachedEntity == null ? ICEWorldEntity.GetWorldEntity( this.gameObject ) : m_AttachedEntity ); }
		}

		// IMPORTANT: this overrides the EntityDamageConverter.DoHandleDamage method with the 
		// customized damage method and allows to use the original damage handler of the asset.
		private DamageConverter _dc = new DamageConverter();

		public virtual bool OnDamaged( DamageMessage _msg )
		{
			if( AttachedEntity == null || AttachedEntity.Status.IsDestroyed )
				return true;
			
			/*
			if (!IsAlive) { return true; }

			float lRemainingHealth = 0f;
			if (AttributeSource != null)
			{
				if (rMessage is DamageMessage)
				{
					lRemainingHealth = AttributeSource.GetAttributeValue<float>(HealthID) - ((DamageMessage)rMessage).Damage;
					AttributeSource.SetAttributeValue(HealthID, lRemainingHealth);
				}
			}

			if (lRemainingHealth <= 0f)
			{
				OnKilled(rMessage);
			}
			else if (rMessage != null)
			{
				bool lPlayAnimation = true;
				if (rMessage is DamageMessage) { lPlayAnimation = ((DamageMessage)rMessage).AnimationEnabled; }

				if (lPlayAnimation)
				{
					MotionController lMotionController = gameObject.GetComponent<MotionController>();
					if (lMotionController != null)
					{
						// Send the message to the MC to let it activate
						rMessage.ID = CombatMessage.MSG_DEFENDER_DAMAGED;
						lMotionController.SendMessage(rMessage);
					}

					if (!rMessage.IsHandled && DamagedMotion.Length > 0)
					{
						MotionControllerMotion lMotion = null;
						if (lMotionController != null) { lMotion = lMotionController.GetMotion(DamagedMotion); }

						if (lMotion != null)
						{
							lMotionController.ActivateMotion(lMotion);
						}
						else
						{
							int lID = Animator.StringToHash(DeathMotion);
							if (lID != 0)
							{
								Animator lAnimator = gameObject.GetComponent<Animator>();
								if (lAnimator != null) { lAnimator.CrossFade(DamagedMotion, 0.25f, 0); }
							}
						}
					}
				}
			}
*/
			return true;
		}

	}

	#endregion
#else
	public class ICEWorldDamageAdapter : ICEWorldBehaviour{}
#endif
}
