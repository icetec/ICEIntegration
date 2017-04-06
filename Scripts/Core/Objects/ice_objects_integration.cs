// ##############################################################################
//
// ICE.World.Objects.ice_objects_integration.cs | DamageConverter : EntityDamageConverter
// Version 1.4.0
//
// Copyrights © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.icecreaturecontrol.com
// mailto:support@icecreaturecontrol.com
//
// Unity Asset Store End User License Agreement (EULA)
// http://unity3d.com/legal/as_terms
//
// ##############################################################################

using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;

using ICE;
using ICE.World;
using ICE.World.Objects;
using ICE.World.Utilities;
using ICE.World.EnumTypes;

#if ICE_INVECTOR_TPC
using Invector.EventSystems;
#endif

namespace ICE.Integration.Objects
{
	public class DamageConverter : EntityDamageConverter{

		public DamageConverter(){
			HandleDamage = DoHandleDamage;
		}

		public new static bool DoHandleDamage( GameObject _sender, GameObject _target, DamageTransferType _impact_type, float _damage, string _damage_method, Vector3 _damage_point, DamageForceType _force_type, float _force )
		{
			if( _target == null || _sender == null || _target == _sender )
				return false;

			bool _handled = false;

			#if ICE_UFPS
				_handled = TryUFPSDamage( _sender, _target, _impact_type, _damage, _damage_method, _damage_point, _force_type, _force );
			#elif ICE_RFPSP
				_handled = TryRFPSPDamage( _sender, _target, _impact_type, _damage, _damage_method, _damage_point, _force_type, _force );
			#elif ICE_OPSIVE_TPC
				_handled = TryTPCDamage( _sender, _target, _impact_type, _damage, _damage_method, _damage_point, _force_type, _force );
			#elif ICE_INVECTOR_TPC
				_handled = TryInvectorTPCDamage( _sender, _target, _impact_type, _damage, _damage_method, _damage_point, _force_type, _force );
			#elif ICE_UNITZ
				_handled = TryUnitZDamage( _sender, _target, _impact_type, _damage, _damage_method, _damage_point, _force_type, _force );
			#elif ICE_EASY_WEAPONS
				_handled = TryEasyWeaponsDamage( _sender, _target, _impact_type, _damage, _damage_method, _damage_point, _force_type, _force );
			#elif ICE_ULTIMATE_SURVIVAL
				_handled = TryUltimateSurvivalDamage( _sender, _target, _impact_type, _damage, _damage_method, _damage_point, _force_type, _force );
			#endif
		
			return _handled;
		}


		/// <summary>
		/// Tries to handle RFPSP damage.
		/// </summary>
		/// <returns><c>true</c>, if RFPSP damage was successful, <c>false</c> otherwise.</returns>
		/// <param name="_sender">Sender.</param>
		/// <param name="_target">Target.</param>
		/// <param name="_impact_type">Impact type.</param>
		/// <param name="_damage">Damage.</param>
		/// <param name="_damage_method">Damage method.</param>
		/// <param name="_damage_point">Damage point.</param>
		/// <param name="_force_type">Force type.</param>
		/// <param name="_force">Force.</param>
		private static bool TryRFPSPDamage( GameObject _sender, GameObject _target, DamageTransferType _impact_type, float _damage, string _damage_method, Vector3 _damage_point, DamageForceType _force_type, float _force )
		{
			if( _target == null || _sender == null || _target == _sender )
				return false;

			bool _handled = false;

			#if ICE_RFPSP

			switch( _target.gameObject.layer )
			{
				case 0://hit object
					if(_target.GetComponent<AppleFall>()){
						_target.GetComponent<AppleFall>().ApplyDamage( _damage );
						_handled = true;
					}else if(_target.GetComponent<BreakableObject>()){
						_target.GetComponent<BreakableObject>().ApplyDamage( _damage );
						_handled = true;
					}/*else if(_target.GetComponent<ExplosiveObject>()){
					_target.GetComponent<ExplosiveObject>().ApplyDamage( _damage );
					_handled = true;
					}*/else if(_target.GetComponent<MineExplosion>()){
						_target.GetComponent<MineExplosion>().ApplyDamage( _damage );
						_handled = true;
					}
					break;
				case 1://hit object is an object with transparent effects like a window
					if(_target.GetComponent<BreakableObject>()){
						_target.GetComponent<BreakableObject>().ApplyDamage( _damage );
						_handled = true;
					}	
					break;
				case 13://hit object is an NPC

					Vector3 _position = ( _damage_point == Vector3.zero ? _target.transform.position : _damage_point ); 
					Vector3 _direction = _sender.transform.position - _target.transform.position;

					if(_target.GetComponent<CharacterDamage>() && _target.GetComponent<AI>().enabled)
					{
						_target.gameObject.GetComponent<CharacterDamage>().ApplyDamage( _damage, _direction, _position, _target.transform, true, false);
						_handled = true;
					}

					if(_target.GetComponent<LocationDamage>() && _target.GetComponent<LocationDamage>().AIComponent.enabled)
					{
						_target.gameObject.GetComponent<LocationDamage>().ApplyDamage( _damage, _direction, _position, _target.transform, true, false);
						_handled = true;
					}

					break;
				case 11://hit object is player or ...
				case 20://hit object is player lean collider
					if(_target.GetComponent<FPSPlayer>()){
						_target.GetComponent<FPSPlayer>().ApplyDamage( _damage, _target.transform, false );
						_handled = true;
					}	
					if(_target.GetComponent<LeanColliderDamage>()){
						_target.GetComponent<LeanColliderDamage>().ApplyDamage( _damage );
						_handled = true;
					}	
					break;
				default:
					break;	
			}

				//if( _handled == true )
				//FPSPlayerComponent.UpdateHitTime();//used for hitmarker

			#endif

			return _handled;
		}

		/// <summary>
		/// Tries to handle UFPS damage.
		/// </summary>
		/// <returns><c>true</c>, if UFPS damage was successful, <c>false</c> otherwise.</returns>
		/// <param name="_sender">Sender.</param>
		/// <param name="_target">Target.</param>
		/// <param name="_impact_type">Impact type.</param>
		/// <param name="_damage">Damage.</param>
		/// <param name="_damage_method">Damage method.</param>
		/// <param name="_damage_point">Damage point.</param>
		/// <param name="_force_type">Force type.</param>
		/// <param name="_force">Force.</param>
		private static bool TryUFPSDamage( GameObject _sender, GameObject _target, DamageTransferType _impact_type, float _damage, string _damage_method, Vector3 _damage_point, DamageForceType _force_type, float _force )
		{
			if( _target == null || _sender == null || _target == _sender )
				return false;

			bool _handled = false;

			#if ICE_UFPS

			vp_DamageTransfer _damage_transfer = _target.GetComponent<vp_DamageTransfer>();				
			if( _damage_transfer != null )
			{
				_damage_transfer.Damage( new vp_DamageInfo( _damage, _sender.transform, ( _force_type == DamageForceType.Explosion ? vp_DamageInfo.DamageType.Explosion : vp_DamageInfo.DamageType.Impact ) ) );
				_handled = true;
			}
			else
			{
				vp_DamageHandler _damage_handler = _target.GetComponent<vp_DamageHandler>();				
				if( _damage_handler != null )
				{
					_damage_handler.Damage( new vp_DamageInfo( _damage, _sender.transform, ( _force_type == DamageForceType.Explosion ? vp_DamageInfo.DamageType.Explosion : vp_DamageInfo.DamageType.Impact ) ) );
					_handled = true;
				}
			}

			#endif

			return _handled;
		}

		/// <summary>
		/// Tries to handle TPC damage.
		/// </summary>
		/// <returns><c>true</c>, if TPC damage was tryed, <c>false</c> otherwise.</returns>
		/// <param name="_sender">Sender.</param>
		/// <param name="_target">Target.</param>
		/// <param name="_impact_type">Impact type.</param>
		/// <param name="_damage">Damage.</param>
		/// <param name="_damage_method">Damage method.</param>
		/// <param name="_damage_point">Damage point.</param>
		/// <param name="_force_type">Force type.</param>
		/// <param name="_force">Force.</param>
		private static bool TryTPCDamage( GameObject _sender, GameObject _target, DamageTransferType _impact_type, float _damage, string _damage_method, Vector3 _damage_point, DamageForceType _force_type, float _force )
		{
			if( _target == null || _sender == null || _target == _sender )
				return false;

			bool _handled = false;

			#if ICE_OPSIVE_TPC

			Vector3 _position = ( _damage_point == Vector3.zero ? _target.transform.position : _damage_point ); 
			Vector3 _direction = _sender.transform.position - _position;
			_direction.Normalize();

			Opsive.ThirdPersonController.Health _health = _target.GetComponentInParent<Opsive.ThirdPersonController.Health>();
			if( _health != null )
			{
				_health.Damage( _damage, _position, ( _force_type != DamageForceType.None ? _direction.normalized * _force : Vector3.zero ) , _sender, _target );
			_handled = true;
			}

			#endif

			return _handled;
		}

		/// <summary>
		/// Tries to handle Invector TPC damage.
		/// </summary>
		/// <returns><c>true</c>, if invector TPC damage was tryed, <c>false</c> otherwise.</returns>
		/// <param name="_sender">Sender.</param>
		/// <param name="_target">Target.</param>
		/// <param name="_impact_type">Impact type.</param>
		/// <param name="_damage">Damage.</param>
		/// <param name="_damage_method">Damage method.</param>
		/// <param name="_damage_point">Damage point.</param>
		/// <param name="_force_type">Force type.</param>
		/// <param name="_force">Force.</param>
		private static bool TryInvectorTPCDamage( GameObject _sender, GameObject _target, DamageTransferType _impact_type, float _damage, string _damage_method, Vector3 _damage_point, DamageForceType _force_type, float _force )
		{
			if( _target == null || _sender == null || _target == _sender )
				return false;

			bool _handled = false;

			#if ICE_INVECTOR_TPC

			Damage _damage_obj = new Damage( (int)Mathf.RoundToInt( _damage ) );

			_damage_obj.sender = _sender.transform;
			_damage_obj.receiver = _target.transform;
			_damage_obj.hitPosition = _damage_point;

			if( _target.IsAMeleeFighter() )
				_target.GetMeleeFighter().OnReceiveAttack( _damage_obj, _target.GetMeleeFighter() );
			else if ( _target.CanReceiveDamage())
				_target.ApplyDamage( _damage_obj );

			_handled = true;

			#endif

			return _handled;
		}

		/// <summary>
		/// Tries to handle UnitZ damage.
		/// </summary>
		/// <returns><c>true</c>, if unit Z damage was tryed, <c>false</c> otherwise.</returns>
		/// <param name="_sender">Sender.</param>
		/// <param name="_target">Target.</param>
		/// <param name="_impact_type">Impact type.</param>
		/// <param name="_damage">Damage.</param>
		/// <param name="_damage_method">Damage method.</param>
		/// <param name="_damage_point">Damage point.</param>
		/// <param name="_force_type">Force type.</param>
		/// <param name="_force">Force.</param>
		private static bool TryUnitZDamage( GameObject _sender, GameObject _target, DamageTransferType _impact_type, float _damage, string _damage_method, Vector3 _damage_point, DamageForceType _force_type, float _force )
		{
			if( _target == null || _sender == null || _target == _sender )
				return false;

			bool _handled = false;

			#if ICE_UNITZ

			DamageManager _damage_manager = _target.GetComponent<DamageManager>(); 
			if( _damage_manager != null )
				_damage_manager = _target.GetComponentInParent<DamageManager> ();

			if( _damage_manager != null )
			{
				Vector3 _direction = _sender.transform.position - _target.transform.position;
				_direction.Normalize();

				_damage_manager.ApplyDamage( Mathf.RoundToInt( _damage ), _direction, "", "" );
				_handled = true;
			}

			#endif

			return _handled;
		}

		/// <summary>
		/// Tries to handle EasyWeapons damage.
		/// </summary>
		/// <returns><c>true</c>, if easy weapons damage was tryed, <c>false</c> otherwise.</returns>
		/// <param name="_sender">Sender.</param>
		/// <param name="_target">Target.</param>
		/// <param name="_impact_type">Impact type.</param>
		/// <param name="_damage">Damage.</param>
		/// <param name="_damage_method">Damage method.</param>
		/// <param name="_damage_point">Damage point.</param>
		/// <param name="_force_type">Force type.</param>
		/// <param name="_force">Force.</param>
		private static bool TryEasyWeaponsDamage( GameObject _sender, GameObject _target, DamageTransferType _impact_type, float _damage, string _damage_method, Vector3 _damage_point, DamageForceType _force_type, float _force )
		{
			if( _target == null || _sender == null || _target == _sender )
				return false;

			bool _handled = false;

			#if ICE_EASY_WEAPONS

			Health _health = _target.GetComponent<Health>(); 
			if( _health != null )
				_health = _target.GetComponentInParent<Health> ();

			if( _health != null )
			{
				_health.ChangeHealth( - _damage );
				_handled = true;
			}
			else
			{
				_target.SendMessageUpwards( "ChangeHealth", - _damage, SendMessageOptions.DontRequireReceiver);
				_handled = true;
			}

			#endif

			return _handled;
		}

		/// <summary>
		/// Tries to handle Ultimate Survival damage.
		/// </summary>
		/// <returns><c>true</c>, if ultimate survival damage was tryed, <c>false</c> otherwise.</returns>
		/// <param name="_sender">Sender.</param>
		/// <param name="_target">Target.</param>
		/// <param name="_impact_type">Impact type.</param>
		/// <param name="_damage">Damage.</param>
		/// <param name="_damage_method">Damage method.</param>
		/// <param name="_damage_point">Damage point.</param>
		/// <param name="_force_type">Force type.</param>
		/// <param name="_force">Force.</param>
		private static bool TryUltimateSurvivalDamage( GameObject _sender, GameObject _target, DamageTransferType _impact_type, float _damage, string _damage_method, Vector3 _damage_point, DamageForceType _force_type, float _force )
		{
			if( _target == null || _sender == null || _target == _sender )
				return false;

			bool _handled = false;

			#if ICE_ULTIMATE_SURVIVAL

			UltimateSurvival.EntityEventHandler _attacker = _sender.GetComponent<UltimateSurvival.EntityEventHandler>();
			UltimateSurvival.EntityEventHandler _victim = _target.GetComponent<UltimateSurvival.EntityEventHandler>();
			UltimateSurvival.IDamageable _damageable = _sender.GetComponent<UltimateSurvival.IDamageable>();

			UltimateSurvival.HealthEventData _health_event_data = new UltimateSurvival.HealthEventData( - _damage, _attacker, _damage_point, _sender.transform.position - _target.transform.position, _force );

			if( _damageable != null)
				_damageable.ReceiveDamage( _health_event_data );
			else if( _victim != null )
				_victim.ChangeHealth.Try( _health_event_data );

			#endif

			return _handled;
		}

	}
}
