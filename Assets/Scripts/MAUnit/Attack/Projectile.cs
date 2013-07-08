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
		
		[HideInInspector]
		public bool armed = false;
		public float speed = 1;
		
		public void Fire(Weapon source, Vector3 target)
		{
			transform.LookAt(target);
			
			armed = true;
			this.source 		= source;
			Vector3 dir 		= (target - transform.position).normalized;	
			rigidbody.velocity 	= dir * speed;
			Destroy(gameObject, cooldown);
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
			
			/*
	        Rigidbody body = hit.collider.attachedRigidbody;
	        if (body == null )
	            return;
			
			if( body.isKinematic  ) 
				Explode (hit);
			
			Unit unit = hit.collider.gameObject.GetComponent<Unit>();
			if(  unit != null && unit != source.owner )
			{
				Debug.LogError("I HIT A UNIT!!!!!!" );
				Explode(hit);
			}
			*/
			
			Explode( collision.contacts[0] );
	    }
		
		public void Explode(ContactPoint hit)
		{			
			// Get all the units in the radius
			List<Monster> monsters = new List<Monster>(Game.Instance.GetMonstersInRange(hit.point, radius));
			foreach( Monster monster in monsters )
				ProjectileHit(monster);
			
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

