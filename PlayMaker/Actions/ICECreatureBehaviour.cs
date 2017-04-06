// ##############################################################################
//
// ICECreatureBehaviour.cs
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
	public class ICECreatureBehaviour : FsmStateAction
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

		public FsmString ActiveBehaviourModeKey;
		public FsmInt ActiveRuleIndex;
		public FsmBool ActiveRuleAnimationIsEnabled;
		public FsmBool ActiveRuleAudioIsEnabled;
		public FsmBool ActiveRuleMoveIsEnabled;
		public FsmBool ActiveRuleEffectIsEnabled;
		public FsmBool ActiveRuleInfluencesIsEnabled;
		public FsmBool ActiveRuleInventoryIsEnabled;
		public FsmBool ActiveRuleEventsIsEnabled;
		public FsmBool ActiveRuleLinkIsEnabled;

		public FsmString ActiveRuleAnimationInterface;
		public FsmInt ActiveRuleAnimationInterfaceType;

		public FsmString LastBehaviourModeKey;



		public override void Reset(){
			/*
			LastBehaviourModeKey.Value = "";

			ActiveBehaviourModeKey.Value = "";
			ActiveRuleIndex.Value = 0;
			ActiveRuleAudioIsEnabled.Value = false;
			ActiveRuleAnimationIsEnabled.Value = false;
			ActiveRuleMoveIsEnabled.Value = false;
			ActiveRuleAudioIsEnabled.Value = false;
			ActiveRuleEffectIsEnabled.Value = false;
			ActiveRuleInfluencesIsEnabled.Value = false;
			ActiveRuleInventoryIsEnabled.Value = false;
			ActiveRuleEventsIsEnabled.Value = false;
			ActiveRuleLinkIsEnabled.Value = false;

			ActiveRuleAnimationInterface.Value = "";
			ActiveRuleAnimationInterfaceType.Value = 0;*/
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
			
			ActiveBehaviourModeKey.Value = Controller.Creature.Behaviour.ActiveBehaviourModeKey;
			if( Controller.Creature.Behaviour.ActiveBehaviourMode != null )
			{
				ActiveRuleIndex.Value = Controller.Creature.Behaviour.ActiveBehaviourMode.ActiveRuleIndex;

				if( Controller.Creature.Behaviour.ActiveBehaviourMode.Rule != null )
				{
					ActiveRuleAnimationInterface.Value = Controller.Creature.Behaviour.ActiveBehaviourMode.Rule.Animation.InterfaceType.ToString();
					ActiveRuleAnimationInterfaceType.Value = (int)Controller.Creature.Behaviour.ActiveBehaviourMode.Rule.Animation.InterfaceType;

					ActiveRuleAnimationIsEnabled.Value = Controller.Creature.Behaviour.ActiveBehaviourMode.Rule.Animation.Enabled;
					ActiveRuleMoveIsEnabled.Value = Controller.Creature.Behaviour.ActiveBehaviourMode.Rule.Move.Enabled;
					ActiveRuleAudioIsEnabled.Value = Controller.Creature.Behaviour.ActiveBehaviourMode.Rule.Audio.Enabled;
					ActiveRuleEffectIsEnabled.Value = Controller.Creature.Behaviour.ActiveBehaviourMode.Rule.Effect.Enabled;
					ActiveRuleInfluencesIsEnabled.Value = Controller.Creature.Behaviour.ActiveBehaviourMode.Rule.Influences.Enabled;
					ActiveRuleInventoryIsEnabled.Value = Controller.Creature.Behaviour.ActiveBehaviourMode.Rule.Inventory.Enabled;
					ActiveRuleEventsIsEnabled.Value = Controller.Creature.Behaviour.ActiveBehaviourMode.Rule.Events.Enabled;
					ActiveRuleLinkIsEnabled.Value = Controller.Creature.Behaviour.ActiveBehaviourMode.Rule.Link.Enabled;
				}
			}

			LastBehaviourModeKey.Value = Controller.Creature.Behaviour.LastBehaviourModeKey;
		}
	}
}
#endif