using System;
using MACore;
using System.Collections.Generic;
using UnityEngine;

namespace MAUnit
{
	public class Projectile : Attack
	{
		public ParticleSystem explosion;
		public Weapon source;
		public float radius = 0;
		public float arc = 0;
		
		[HideInInspector]
		public bool armed = false;
		public float speed = 1;
		
		public void Fire(Weapon source, Vector3 target)
		{
			transform.LookAt(target);
			
			armed = true;
			this.source 		= source;
			
			
			
			Vector3 dir;
			
			// Missile
			if( arc == 0 )
				dir	= (target - transform.position).normalized;	
			
			// Grenade
			else
				dir	= BallisticVel(target, arc);
			
			rigidbody.velocity 	= dir * speed;
			
			Destroy(gameObject, cooldown);
		}
		
		public Vector3 BallisticVel(Vector3 target, float angle)
		{
			Vector3 dir = target - transform.position;  // get target direction
			
			float h 	= dir.y;  			// get height difference
			dir.y 		= 0;  				// retain only the horizontal direction
			float dist 	= dir.magnitude;  	// get horizontal distance
			
			float a 	= angle * Mathf.Deg2Rad;// convert angle to radians
			dir.y 		= dist * Mathf.Tan(a);  // set dir to the elevation angle
			dist 		+= h / Mathf.Tan(a);  	// correct for small height differences
	
			// calculate the velocity magnitude
			float vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
			return vel * dir.normalized;
		}
		
		/// <summary>
		/// Raises the controller collider hit event.
		/// </summary>
		/// <param name='hit'>
		/// Hit.
		/// </param>
	    //void OnControllerColliderHit(ControllerColliderHit hit)
		void OnCollisionEnter(Collision collision)
		{
			if( !armed )
				return;
			
			Explode( collision.contacts[0] );
	    }
		
		public void Explode(ContactPoint hit)
		{			
			
			//Debug.LogError("SOURCE IS " + source.owner + " IS monster? " + (source.owner.GetType() == typeof(Monster)) );
			//Debug.LogError("SOURCE IS MONSTER?" + source.owner is Monster + " source is " + source );
			
			// Get all the units in the radius
			if( source.owner.GetType() == typeof(Defender) )
			{
				List<Monster> monsters = new List<Monster>(Game.Instance.GetMonstersInRange(hit.point, radius));
				foreach( Monster monster in monsters )
					ProjectileHit(monster);
			}
			else if( source.owner.GetType() == typeof(Monster) )
			{
				List<Defender> defenders = new List<Defender>(Game.Instance.GetDefendersInRange(hit.point, radius));
				
				Debug.Log("DEFENDERS ARE: " + defenders.Count );
				foreach( Defender defender in defenders )
					ProjectileHit(defender);				
			}
			
			Destroy(gameObject);
			
			
			PlaySound();
			if( explosion != null )
			{
				ParticleSystem effect = Instantiate(explosion, transform.position, Quaternion.identity) as ParticleSystem;
				Destroy(effect.gameObject, explosion.duration);			
			}
		}
		
		private void ProjectileHit(Unit target)
		{
			Game.Instance.DoDamage(source.owner, this, target);
		}
	}
}

