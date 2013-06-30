using System;
using UnityEngine;

namespace MAUnit
{
	[RequireComponent(typeof (Animator))]
	[RequireComponent(typeof (Rigidbody))]
	public class UnitAnimationController  : AbstractUnitAnimationController
	{
		static int IdleState 	= Animator.StringToHash("Base Layer.Idle");
		static int AttackState 	= Animator.StringToHash("Base Layer.Attack");
		static int RunState 	= Animator.StringToHash("Base Layer.Run");
		static int DeathState 	= Animator.StringToHash("Base Layer.Death");
		
		private Animator anim;
		
		
		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start()
		{
			// Get the animator
			anim = GetComponent<Animator>();
		}
		
		/// <summary>
		/// Fixeds the update.
		/// </summary>
		public void FixedUpdate()
		{
			Unit unit = GetComponent<Unit>();
			
			// Find out if im moving
			Vector3 dir = (unit.GetTargetPosition()-unit.transform.position).normalized;
			
			if( anim == null ) 
				return;
			
			if( unit.IsDead() )
			{
				anim.SetBool("Dead", true );
				return;
			}
			
			// Set the running state
			if( dir == Vector3.zero )
			{
				transform.forward = Vector3.forward;
				anim.SetBool("Running", false);
			}
			else
			{
				transform.forward = dir;
				anim.SetBool("Running", true);
			}
			
			
			
			
			
			// If I am already attacking, I cant be attacking!
			AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
			if( state.nameHash ==  AttackState )
				_attacking = false;
			
			// Set the attacking state
			anim.SetBool("Attacking", _attacking);
		}
	}
}


