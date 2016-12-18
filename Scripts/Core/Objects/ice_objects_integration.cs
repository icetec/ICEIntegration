// ##############################################################################
//
// ICE.World.Objects.ice_objects_integration.cs
// Version 1.3.5
//
// Copyrights © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.icecreaturecontrol.com
// mailto:support@icecreaturecontrol.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is furnished 
// to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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

namespace ICE.World.Objects
{
	public class CodeAdapter
	{
		public static bool TryPreparingUnitZ(){

			#if UNITY_EDITOR
				string _file = "Assets/UnitZ/Scripts/Damage/DamageManager.cs";
				if( File.Exists( _file ) )
				{
				string _text = "";//File.ReadAllText( _file );
					if( ! _text.Contains( "MODIFIED BY ICE" ) )
					{
						string _search_str = "public void ApplyDamage";
						string _replace_str = "// MODIFIED BY ICE - virtual modifier was added\n" +
							"\tpublic virtual void ApplyDamage";

						_text = _text.Replace( _search_str, _replace_str );

						Debug.Log( "Insert virtual modifier for ApplyDamage in '" + _file + "'" ); 
						//File.WriteAllText( _file, _text );
					}

					return true;
				}

				return false;
			#else
				return true;
			#endif
		}
	}

	public class DamageConverter{

		public static bool HandleDamage( GameObject _sender, GameObject _target, DamageTransferType _impact_type, float _damage, string _damage_method, Vector3 _damage_point, DamageForceType _force_type, float _force )
		{
			if( _target == null || _sender == null || _target == _sender )
				return false;

			bool _handled = false;

			#if UFPS
			_handled = TryUFPSDamage( _sender, _target, _impact_type, _damage, _damage_method, _damage_point, _force_type, _force );
			#elif RFPSP
			_handled = TryRFPSPDamage( _sender, _target, _impact_type, _damage, _damage_method, _damage_point, _force_type, _force );
			#elif TPC
			_handled = TryTPCDamage( _sender, _target, _impact_type, _damage, _damage_method, _damage_point, _force_type, _force );
			#elif UNITZ
			_handled = TryUnitZDamage( _sender, _target, _impact_type, _damage, _damage_method, _damage_point, _force_type, _force );
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

			#if RFPSP

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

			#if UFPS

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

			#if TPC

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

			#if UNITZ

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


	}
}
