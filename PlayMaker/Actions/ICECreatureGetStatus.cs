// ##############################################################################
//
// ICECreatureGetStatus.cs
// Version 1.4.0
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

#if ICE_PLAYMAKER && ICE_CC 
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategoryAttribute("ICE")]
	[Tooltip("Gets the status values of an ICECreatureControl character.")]
	public class ICECreatureGetStatus : FsmStateAction
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

		public enum ValueType { Age, Damage, Stress, Debility, Hunger, Thirst, Aggressivity, Experience, Anxiety, Nosiness };

		public ValueType DesiredValue;
		public FsmBool InPercent;
		[HasFloatSlider(0, 1)]
		public FsmFloat UpdateInterval;

		public FsmFloat StatusValue;


		private float m_UpdateTime;
		private float m_UpdateTimer;
		private bool m_Finish;


		public override void Reset(){

			m_Finish = false;
		}

		public override void OnEnter(){

			m_UpdateTime = Time.realtimeSinceStartup;
			m_UpdateTimer = 0;
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

			if( UpdateInterval.Value > 0 )
			{
				m_UpdateTimer = Time.realtimeSinceStartup - m_UpdateTime;
				if( m_UpdateTimer >= UpdateInterval.Value )
				{
					StatusValue.Value = GetStatusValue();
					m_UpdateTime = Time.realtimeSinceStartup;
				}
			}
			else
			{
				StatusValue.Value = GetStatusValue();
				m_Finish = true;
			}
			       
			if( m_Finish )
				Finish();
		}

		private float GetStatusValue()
		{
			if( Controller == null )
				return 0;

			switch( DesiredValue )
			{
				case ValueType.Age:
					return ( InPercent.Value ? Controller.Creature.Status.LifespanInPercent : Controller.Creature.Status.Age );

				case ValueType.Damage:
					return ( InPercent.Value ? Controller.Creature.Status.DamageInPercent : Controller.Creature.Status.Damage );
				case ValueType.Stress:
					return ( InPercent.Value ? Controller.Creature.Status.StressInPercent : Controller.Creature.Status.Stress );
				case ValueType.Debility:
					return ( InPercent.Value ? Controller.Creature.Status.DebilityInPercent : Controller.Creature.Status.Debility );
				case ValueType.Hunger:
					return ( InPercent.Value ? Controller.Creature.Status.HungerInPercent : Controller.Creature.Status.Hunger );
				case ValueType.Thirst:
					return ( InPercent.Value ? Controller.Creature.Status.ThirstInPercent : Controller.Creature.Status.Thirst );

				case ValueType.Aggressivity:
					return ( InPercent.Value ? Controller.Creature.Status.AggressivityInPercent : Controller.Creature.Status.Aggressivity );
				case ValueType.Anxiety:
					return ( InPercent.Value ? Controller.Creature.Status.AnxietyInPercent : Controller.Creature.Status.Anxiety );
				case ValueType.Experience:
					return ( InPercent.Value ? Controller.Creature.Status.ExperienceInPercent : Controller.Creature.Status.Experience );
				case ValueType.Nosiness:
					return ( InPercent.Value ? Controller.Creature.Status.NosinessInPercent : Controller.Creature.Status.Nosiness );
				default:	
					return 0;
			}
		}

	}
}
#endif