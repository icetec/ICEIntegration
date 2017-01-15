// ##############################################################################
//
// ICECreatureTarget.cs
// Version 1.3.7
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
	public class ICECreatureTarget : FsmStateAction
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



		public override void Reset(){

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
			


		}
	}
}
#endif