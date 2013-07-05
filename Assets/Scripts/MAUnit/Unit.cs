using System;
using UnityEngine;
using MACore;

namespace MAUnit
{
	[RequireComponent(typeof (CapsuleCollider))]
	public abstract class Unit : MonoBehaviour
	{
		
		public GameObject DefaultTarget;
		public GameObject weaponParent;
		public Texture icon;
		public new string name;
		public string desc;		

		protected float hp;
		public float CurrentHealth { get { return hp; } set { hp = value; } }
		public int health;
		public int armor;
		public float speed;
		
		public Weapon weapon;
		public ParticleSystem hurt;
		
		protected bool bDead;
		protected bool bReady;
		protected Unit target;
		private float radius;
		
		
		// Stateful variables
		private bool spinningUp;
		[HideInInspector]
		public bool Feared;
		[HideInInspector]
		public bool powerTargeting;
		
		public bool SpinningUp { get { return spinningUp; } set { spinningUp = value; } }


		public virtual void Awake()
		{
			Feared			= false;
			powerTargeting 	= false;
			spinningUp 		= false;
			if( DefaultTarget == null )
				DefaultTarget = gameObject;
			
			CurrentHealth = health;
			bDead = false;
			bReady= false;
			radius= -1.0f;			
		}
		
		public float GetCurrentHealth()
		{
			return hp;
		}
		
		/// <summary>
		/// Gets the radius.
		/// </summary>
		/// <returns>
		/// The radius.
		/// </returns>
		public float GetRadius()
		{
			if( radius < 0 )
			{
				radius = GetComponent<CapsuleCollider>().radius;
			}
			return radius;
		}

		public void Begin(Vector3 spawn)
		{
			if( weaponParent == null )
				weaponParent = gameObject;
			
			// Initialize its location
			transform.position = spawn;
			weapon = Instantiate(weapon) as Weapon;
			weapon.transform.parent 	= weaponParent.transform;
			weapon.transform.localPosition = Vector3.zero;
			bReady = true;
		}		
		
		public void Kill(Vector3 force)
		{
			
			bDead = true;
			//gameObject.AddComponent<Rigidbody>();			

			if( rigidbody != null )
			{
				//Debug.LogWarning("FORCE: " + force );
				//Animator anim 	= GetComponent<Animator>();
				//anim.enabled 	= false;				
				rigidbody.AddForce( force, ForceMode.Impulse );
			}

			//Destroy(this);
		}
		
		public bool IsDead()
		{
			return bDead;
		}
		
		/// <summary>
		/// Raises the attack event.
		/// </summary>
		/// <param name='target'>
		/// Target.
		/// </param>
		public void OnHurt(Unit source)
		{
			PlayHurt();
		}		
		

		/// <summary>
		/// Picks the target.
		/// </summary>
		protected abstract void PickTarget();
		
		public virtual void Update()
		{
			// QUick sanity check
			if( !bReady ) return;
			if( bDead ) return;
			
			
			if( spinningUp || powerTargeting ) 
				return;
			
			// Pick a target
			PickTarget ();
			
			if( target == null )
				return;
			
			// Check to see if I can attack my target!
			if( weapon.IsInRange(this,target) && !Feared)
			{
				AttackTarget();
				return;			
			}
			
			// Move towards that target
			MoveTowards();
		}
		
		/// <summary>
		/// Moves the towards.
		/// </summary>
		public virtual void MoveTowards()
		{
			float s = Feared?speed/2:speed;
			transform.position = Vector3.MoveTowards(transform.position, GetTargetPosition(), Time.deltaTime*s);			

		}
		
		/// <summary>
		/// Attacks the target.
		/// </summary>
		public virtual void AttackTarget()
		{
			if( weapon.CanHit(this, target) )
			{				
				UnitAnimationController anim = GetComponent<UnitAnimationController>();
				if( anim != null )
					anim.Attacking = true;
				
				// Execute the attack in a delayed way if needed
				if( weapon.spinup > 0 )
				{
					spinningUp = true;
					Invoke("ExecuteAttack", weapon.spinup);
				}
				else
					ExecuteAttack();
			}
		}
		
		/// <summary>
		/// Spinups the attack.
		/// </summary>
		private void ExecuteAttack()
		{
			if( !IsDead() && !powerTargeting )
			{
				weapon.Attack(target);
				Game.Instance.DoDamage(this, weapon, target);	
			}
			spinningUp = false;
		}
		
		/// <summary>
		/// Gets the target position.
		/// </summary>
		/// <returns>
		/// The target position.
		/// </returns>
		public virtual Vector3 GetTargetPosition()
		{
			if( target == null )
				return transform.position;			
			
			if( !Feared )
			{
				return target.transform.position;
			}
			else
			{
				Vector3 dir = (transform.position-target.transform.position).normalized;	
				dir.Scale(new Vector3(100, 100, 100));
				return transform.position + dir;
			}
		}
		
		
		protected void PlayHurt()
		{
			if( hurt == null )
				return;
			 
			ParticleSystem effect = Instantiate(hurt, transform.position, Quaternion.identity) as ParticleSystem;
			Destroy(effect.gameObject, hurt.duration);
		}
		
		public bool IsReady()
		{
			return bReady;
		}
	}
}

