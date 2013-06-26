using System;
using UnityEngine;
using MACore;

namespace MAUnit
{
	public abstract class Unit : MonoBehaviour
	{
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

		public Unit ()
		{
		}
		
		public void Awake()
		{
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
			// Initialize its location
			transform.position = spawn;
			weapon = Instantiate(weapon) as Weapon;
			weapon.transform.parent = transform;
			weapon.transform.position = transform.position;
			bReady = true;
		}		
		
		public void Kill(Vector3 force)
		{
			bDead = true;
			gameObject.AddComponent<Rigidbody>();			
			rigidbody.AddForce( force, ForceMode.Impulse );

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
			
			// Pick a target
			PickTarget ();
			
			if( target == null )
				return;
			
			// Check to see if I can attack my target!
			if( weapon.IsInRange(this,target) )
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
			transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime*speed);			
		}
		
		public virtual void AttackTarget()
		{
			if( weapon.CanHit(this, target) )
			{				
				weapon.Attack(target);
				Game.Instance.DoDamage(this, weapon, target);
			}
		}
		
		
		protected void PlayHurt()
		{
			if( hurt == null )
				return;
			 
			ParticleSystem effect = Instantiate(hurt, transform.position, Quaternion.identity) as ParticleSystem;
			Debug.Log("OBJ: " + effect );
			Destroy(effect.gameObject, hurt.duration);
		}
		
		public bool IsReady()
		{
			return bReady;
		}
	}
}

