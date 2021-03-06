using System;
using UnityEngine;
using MACore;

namespace MAUnit
{
	[RequireComponent(typeof (Animator))]
	[RequireComponent(typeof (Rigidbody))]
	public class UnitAnimationController  : AbstractUnitAnimationController
	{
		//static int IdleState 	= Animator.StringToHash("Base Layer.Idle");
		static int AttackState 	= Animator.StringToHash("Base Layer.Attack");
		//static int RunState 	= Animator.StringToHash("Base Layer.Run");
		//static int DeathState 	= Animator.StringToHash("Base Layer.Death");
		
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
			
			Vector3 delta 	= unit.GetTargetPosition()-unit.transform.position;
			delta.y 		= 0;
			Vector3 dir 	= delta.normalized;
			
			if( delta.magnitude < 0.5 )
			{
				unit.rigidbody.velocity = Vector3.zero;
				dir = Vector3.zero;
			}
			
			if( anim == null || unit.IsInactive() ) 
				return;
			
			if( unit.IsDead() )
			{
				anim.SetBool("Dead", true );
				return;
			}
			
			// Set the running state
			if( unit.powerTargeting  )
			{
				anim.SetBool("Running", false);
			}
			else
			{				
				if( dir == Vector3.zero )
					FaceTarget( Game.Instance==null?Vector3.forward:Vector3.back );
				else
					FaceTarget(dir);
				
				
				bool isRunning = unit.rigidbody.velocity.magnitude > 0.1f  && delta.sqrMagnitude > 1f;
				anim.SetBool("Running", isRunning );
			}			
			
			// If I am already attacking, I cant be attacking!
			AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
			if( state.nameHash ==  AttackState )
				_attacking = false;
			
			// Set the attacking state
			anim.SetBool("Attacking", _attacking);
		}
		
		private void FaceTarget(Vector3 dir)
		{			
			// Get the dest target
			// Vector3 target 		= GetComponent<Unit>().GetTargetPosition();
			//yaw.transform.rotation = Quaternion.LookRotation(dir);
			float targetAngle	= Quaternion.LookRotation(dir).eulerAngles.y;// + (GetComponent<Unit>().Feared?180.0f:0.0f);
			float currentAngle	= transform.rotation.eulerAngles.y;
			
			transform.RotateAround(Vector3.up, (targetAngle-currentAngle) * 0.2f * Time.deltaTime);			
		}
	}	
}


