using System;
using UnityEngine;
using System.Collections.Generic;
using MACore;

namespace MAUnit
{
	[RequireComponent(typeof (CapsuleCollider))]
	public abstract class Unit : MonoBehaviour
	{		
		public GameObject SelectionParent;
		public GameObject DefaultTarget;
		public GameObject weaponParent;
		public Texture icon;
		public new string name;
		public string desc;		
		public bool showStatus;

		protected float hp;
		public float CurrentHealth { get { return hp; } set { hp = value; } }
		public int health;
		public float armor;
		public float speed;
		
		public Weapon weapon;
		public List<Weapon> alternates;
		private List<Weapon> weapons;
		public ParticleSystem hurt;
		
		protected bool bDead;
		protected bool bReady;
		protected Unit target;
		public Unit Target { get { return target; } set { target = value; } }
		private float radius;
		
		[HideInInspector]
		public bool inCombatArea;
		
		[HideInInspector]
		public bool selected;		
			
		
		// Stateful variables
		private bool spinningUp;
		[HideInInspector]
		private bool _feared;
		private ParticleSystem _fearedEffect = null;
		public bool Feared
		{
			get { return _feared; }
			set
			{
				_feared = value;

				if( _feared && _fearedEffect == null )
				{
					GameObject template = Resources.Load("Particles/Feared") as GameObject;
					_fearedEffect = (Instantiate( template ) as GameObject).GetComponent<ParticleSystem>();
					_fearedEffect.transform.parent = this.DefaultTarget.transform;
					_fearedEffect.transform.localPosition = Vector3.zero;
				}
				
				
				else if( !_feared && _fearedEffect != null )
				{
					Destroy(_fearedEffect);
					_fearedEffect = null;
				}
			}
		}
		
		
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
			inCombatArea = false;
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
			
			Vector3 testSource 	= new Vector3(spawn.x, spawn.y+5, spawn.z );
			RaycastHit hit 		= new RaycastHit();
			if( Physics.Raycast( testSource, Vector3.down, out hit, 500.0f) )
			{
				spawn = hit.point;
			}
				
			weapons = new List<Weapon>();
			
			// Initialize its location
			transform.position = spawn;
			weapon = Instantiate(weapon) as Weapon;
			weapon.transform.parent 		= weaponParent.transform;
			weapon.transform.localPosition 	= Vector3.zero;
			weapon.owner					= this;
			
			weapons.Add(weapon);

			for( int x=0;x<alternates.Count;x++ )
			{
				Weapon alt = alternates[x];
				alt = Instantiate(alt) as Weapon;
				alt.transform.parent 		= weaponParent.transform;
				alt.transform.localPosition = Vector3.zero;
				alt.owner					= this;
				alternates[x] 				= alt;
				
				weapons.Add( alt );
			}
			
			bReady 							= true;
		}		
		
		public void Kill()
		{
			// The dead have no fear...
			Feared = false;
			bDead = true;
		}
		
		/// <summary>
		/// Applies the force.
		/// </summary>
		/// <param name='force'>
		/// Force.
		/// </param>
		public void ApplyForce(Vector3 force)
		{
			if( rigidbody == null )
				return;
			
			Animator anim 	= GetComponent<Animator>();

			if( anim != null )
				anim.applyRootMotion = false;
			rigidbody.AddForce( force, ForceMode.Impulse );
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
		
		public abstract Unit GetRandomTarget();
		
		public virtual void Update()
		{
			bool attacked = false;
			// QUick sanity check
			if( !bReady ) return;
			if( bDead ) return;
			
			if( spinningUp || powerTargeting ) 
				return;
			
			// Check to see if im in the combat area
			inCombatArea = Game.Instance.IsWithinCombatArea(this);
			
			// Pick a target
			PickTarget ();
			
			// Check to see if I can attack my target!
			if( inCombatArea && !Feared  && target != null )
			{
				foreach( Weapon weap in weapons )
				{
					if( weap.IsInRange(this,target) )
					{
						AttackTarget(weap);
						
						if( !rigidbody.isKinematic )
							rigidbody.velocity = Vector3.zero;
						attacked = true;
					}
				}
			}
			
			// Move towards that target
			if( !attacked )
				MoveTowards();
		}
		
		/// <summary>
		/// Determines whether this instance is target dead.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance is target dead; otherwise, <c>false</c>.
		/// </returns>
		public bool IsTargetDead()
		{
			return target == null || target.IsDead();
		}
		
		/// <summary>
		/// Moves the towards.
		/// </summary>
		public virtual void MoveTowards()
		{
			//float s = Feared?speed/2:speed;
			//transform.position = Vector3.MoveTowards(transform.position, GetTargetPosition(), Time.deltaTime*s);	
			
			if( target != null && target.IsDead() )
			{
				rigidbody.velocity = Vector3.zero;
				return;
			}
			
			Vector3 dir = (GetTargetPosition()-transform.position).normalized;
			
			Vector3 v = rigidbody.velocity;
			
			float currentSpeed = Feared?speed/5:speed;
			
			//rigidbody.velocity = dir * speed;
			if( !rigidbody.isKinematic )
				rigidbody.velocity = Vector3.Lerp(v, dir*currentSpeed, Time.deltaTime*10);

		}
		
		/// <summary>
		/// Attacks the target.
		/// </summary>
		public virtual void AttackTarget(Weapon instance)
		{
			if( instance.CanHit(this, target) )
			{				
				UnitAnimationController anim = GetComponent<UnitAnimationController>();
				if( anim != null )
					anim.Attacking = true;
				
				// Execute the attack in a delayed way if needed
				if( instance.spinup > 0 && instance == weapon )
				{
					spinningUp = true;
					Invoke("ExecuteMainAttack", instance.spinup);
				}
				else
					ExecuteAttack(instance);
			}
		}
		
		private void ExecuteMainAttack()
		{
			ExecuteAttack(weapon);
		}
		
		/// <summary>
		/// Spinups the attack.
		/// </summary>
		private void ExecuteAttack(Weapon instance)
		{
			if( Feared ) return;
			
			if( !IsDead() && !powerTargeting )
			{
				instance.Attack(target);
				Game.Instance.DoDamage(this, instance, target);	
			}
			
			if( instance == weapon )
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
			if( Game.Instance == null )
				return transform.position;
			
			if( !inCombatArea || target == null )
				return Game.Instance.GetDefenderSpawnCenter();
				//return transform.position;			
			
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

