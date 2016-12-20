// ##############################################################################
//
// ICECreatureStatus.cs
// Version 1.1.21
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
	public class ICECreatureStatus : FsmStateAction
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

		public FsmFloat Age;
		public FsmFloat MaxAge;

		public FsmFloat Fitness;
		public FsmFloat Health;
		public FsmFloat Power;
		public FsmFloat Stamina;

		public FsmFloat Damage;
		public FsmFloat Stress;   
		public FsmFloat Debility;
		public FsmFloat Hunger;
		public FsmFloat Thirst;

		public FsmFloat Aggressivity;
		public FsmFloat Anxiety;
		public FsmFloat Experience;
		public FsmFloat Nosiness;

		public FsmBool IsDead;
		public FsmBool IsWounded;

		public FsmString Gender;
		public FsmInt GenderType;

		public FsmString TrophicLevel;
		public FsmInt TrophicLevelType;
		public FsmBool IsCannibal;

		public override void Reset(){
			/*
			Age.Value = 0;
			MaxAge.Value = 0;

			Fitness.Value = 0;

			Health.Value = 0;
			Power.Value = 0;
			Stamina.Value = 0;

			Aggressivity.Value = 0;
			Anxiety.Value = 0;
			Experience.Value = 0;
			Nosiness.Value = 0;

			Damage.Value = 0;
			Debility.Value = 0;
			Stress.Value = 0;
			Hunger.Value = 0;
			Thirst.Value = 0;

			IsDead.Value = false;
			IsWounded.Value = false;

			Gender.Value = "";
			GenderType.Value = 0;

			TrophicLevel.Value = "";
			TrophicLevelType.Value = 0;
			IsCannibal.Value = false;*/
		}

		public override void OnEnter(){
			UpdateValues();
		}

		public override void OnUpdate()
		{
			UpdateValues();
		}

		void UpdateValues()
		{
			if( Controller == null )
				return;
			
			//behaviourMode.Value = Controller.Creature.Behaviour.ActiveBehaviourModeKey;
			Age.Value = Controller.Creature.Status.Age;
			MaxAge.Value = Controller.Creature.Status.MaxAge;

			Fitness.Value = Controller.Creature.Status.FitnessInPercent;

			Health.Value = Controller.Creature.Status.HealthInPercent;
			Power.Value = Controller.Creature.Status.PowerInPercent;
			Stamina.Value = Controller.Creature.Status.StaminaInPercent;

			Aggressivity.Value = Controller.Creature.Status.AggressivityInPercent;
			Anxiety.Value = Controller.Creature.Status.AnxietyInPercent;
			Experience.Value = Controller.Creature.Status.ExperienceInPercent;
			Nosiness.Value = Controller.Creature.Status.NosinessInPercent;

			Damage.Value = Controller.Creature.Status.DamageInPercent;
			Debility.Value = Controller.Creature.Status.DebilityInPercent;
			Stress.Value = Controller.Creature.Status.StressInPercent;
			Hunger.Value = Controller.Creature.Status.HungerInPercent;
			Thirst.Value = Controller.Creature.Status.ThirstInPercent;

			IsDead = Controller.Creature.Status.IsDead;
			IsWounded = Controller.Creature.Status.IsWounded;

			Gender = Controller.Creature.Status.GenderType.ToString();
			GenderType = (int)Controller.Creature.Status.GenderType;

			TrophicLevel = Controller.Creature.Status.TrophicLevel.ToString();
			TrophicLevelType = (int)Controller.Creature.Status.TrophicLevel;
			IsCannibal = Controller.Creature.Status.IsCannibal;

		}
	}
}
#endif