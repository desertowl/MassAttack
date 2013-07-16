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
		public float missRadius = 0.0f;
		
		[HideInInspector]
		public bool armed = false;
		public float speed = 1;
		
		/// <summary>
		/// Fire the specified source and target.
		/// </summary>
		/// <param name='source'>
		/// Source.
		/// </param>
		/// <param name='target'>
		/// Target.
		/// </param>
		public void Fire(Weapon source, Vector3 target)
		{
			transform.LookAt(target);
			
			armed 			= true;
			this.source 	= source;
			
			if( missRadius != 0 )
				target += GetFudge();
			
			
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
		/// Gets the fudge.
		/// </summary>
		/// <returns>
		/// The fudge.
		/// </returns>
		private Vector3 GetFudge()
		{
			return new Vector3(UnityEngine.Random.value * missRadius, 0, UnityEngine.Random.value * missRadius);
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
			
			
			Explode( collision.gameObject, collision.contacts[0] );
	    }
		
		public void Explode(GameObject vicitm, ContactPoint hit)
		{	
			// Get all the units in the radius
			if( source.owner.GetType() == typeof(Defender) )
			{	
				// If im a monster...
				Monster target = vicitm.GetComponent<Monster>();
				
				if( target!=null )
					ProjectileHit(target);
				if( radius == 0 ) return;
				
				List<Monster> monsters = new List<Monster>(Game.Instance.GetMonstersInRange(hit.point, radius));
				foreach( Monster monster in monsters )
					if( monster != target )
						ProjectileHit(monster);
			}
			else if( source.owner.GetType() == typeof(Monster) )
			{
				// If im a Defender...
				Defender target = vicitm.GetComponent<Defender>();
				
				if( target!=null )
					ProjectileHit(target);
				if( radius == 0 ) return;				
				
				List<Defender> defenders = new List<Defender>(Game.Instance.GetDefendersInRange(hit.point, radius));
				foreach( Defender defender in defenders )
					if( defender != target )
						ProjectileHit(defender);				
			}
			
			Destroy(gameObject);
			
			
			PlaySound();
			SpawnParticleSystem(explosion);
		}
		
		private void ProjectileHit(Unit target)
		{
			Game.Instance.DoDamage(source.owner, this, target);
		}
	}
}

