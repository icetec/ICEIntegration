// ##############################################################################
//
// ICECreatureSetStatus.cs
// Version 1.3.5
//
// © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.icecreaturecontrol.com
// mailto:support@icecreaturecontrol.com
// 
// Unity Asset Store End User License Agreement (EULA)
// http://unity3d.com/legal/as_terms
//
// ##############################################################################

using UnityEngine;
using ICE;
using ICE.Creatures;

#if PLAYMAKER && ICE_CC 
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategoryAttribute("ICE")]
	[Tooltip("Gets the status values of an ICECreatureControl character.")]
	public class ICECreatureSetStatus : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		protected GameObject m_ControllerOwner = null;
		protected GameObject ControllerOwner{
			get{ return m_ControllerOwner = ( m_ControllerOwner == null ? Fsm.GetOwnerDefaultTarget(gameObject) : m_ControllerOwner ); }
		}

		protected ICECreatureControl m_Controller = null;
		protected ICECreatureControl Controller{
			get{ return m_Controller = ( m_Controller == null && ControllerOwner != null ? ControllerOwner.GetComponent<ICECreatureControl>() : m_Controller ); }
		}

		public enum ValueType { Damage, Stress, Debility, Hunger, Thirst, Aggressivity, Experience, Anxiety, Nosiness };


		public ValueType DesiredValue;
		[HasFloatSlider(-100, 100)]
		public FsmFloat DesiredAmount;
		public FsmBool OverwriteAmount;
		[HasFloatSlider(0, 1)]
		public FsmFloat UpdateInterval;
		public FsmInt UpdateLimit;

		public FsmFloat StatusValue;





		private int m_UpdateLimitCounter;
		private float m_UpdateTime;
		private float m_UpdateTimer;
		private bool m_Finish;


		public override void Reset(){

			m_Finish = false;
		}

		public override void OnEnter(){

			m_UpdateTime = Time.realtimeSinceStartup;
			m_UpdateTimer = 0;
			m_UpdateLimitCounter = 0;
			m_Finish = false;
		}

		public override void OnUpdate()
		{
			UpdateValues();
		}

		void UpdateValues()
		{
			if( Controller == null )
				return;

			if( UpdateLimit.Value == 0 || m_UpdateLimitCounter < UpdateLimit.Value )
			{
				if( UpdateLimit.Value > 0 )
					m_UpdateLimitCounter++;
					
				if( UpdateInterval.Value > 0 )
				{
					m_UpdateTimer = Time.realtimeSinceStartup - m_UpdateTime;
					if( m_UpdateTimer >= UpdateInterval.Value )
					{
						StatusValue.Value = UpdateStatus( DesiredAmount.Value );
						m_UpdateTime = Time.realtimeSinceStartup;
					}
				}
				else
				{
					StatusValue.Value = UpdateStatus( DesiredAmount.Value );
				}
			}
			else
				m_Finish = true;
			       
			if( m_Finish )
				Finish();
		}

		private float UpdateStatus( float _amount )
		{
			if( Controller == null )
				return 0;

			switch( DesiredValue )
			{
				case ValueType.Damage:
					if( OverwriteAmount.Value ) Controller.Creature.Status.DamageInPercent = _amount; else Controller.Creature.Status.AddDamage(_amount);
					return Controller.Creature.Status.DamageInPercent;
				case ValueType.Stress:
					if( OverwriteAmount.Value ) Controller.Creature.Status.StressInPercent = _amount; else Controller.Creature.Status.AddStress(_amount);
					return Controller.Creature.Status.StressInPercent;
				case ValueType.Debility:
					if( OverwriteAmount.Value ) Controller.Creature.Status.DebilityInPercent = _amount; else Controller.Creature.Status.AddDebility(_amount);
					return Controller.Creature.Status.DebilityInPercent;
				case ValueType.Hunger:
					if( OverwriteAmount.Value ) Controller.Creature.Status.HungerInPercent = _amount; else Controller.Creature.Status.AddHunger(_amount);
					return Controller.Creature.Status.HungerInPercent;
				case ValueType.Thirst:
					if( OverwriteAmount.Value ) Controller.Creature.Status.ThirstInPercent = _amount; else Controller.Creature.Status.AddThirst(_amount);
					return Controller.Creature.Status.ThirstInPercent;

				case ValueType.Aggressivity:
					if( OverwriteAmount.Value ) Controller.Creature.Status.Aggressivity = _amount; else Controller.Creature.Status.AddAggressivity(_amount);
					return Controller.Creature.Status.AggressivityInPercent;
				case ValueType.Anxiety:
					if( OverwriteAmount.Value ) Controller.Creature.Status.Anxiety = _amount; else Controller.Creature.Status.AddAnxiety(_amount);
					return Controller.Creature.Status.AnxietyInPercent;
				case ValueType.Experience:
					if( OverwriteAmount.Value ) Controller.Creature.Status.Experience = _amount; else Controller.Creature.Status.AddExperience(_amount);
					return Controller.Creature.Status.ExperienceInPercent;
				case ValueType.Nosiness:
					if( OverwriteAmount.Value ) Controller.Creature.Status.Nosiness = _amount; else Controller.Creature.Status.AddNosiness(_amount);
					return Controller.Creature.Status.NosinessInPercent;
				default:	
					return 0;

			}
		}

	}
}
#endif