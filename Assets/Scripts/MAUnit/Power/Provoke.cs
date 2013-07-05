using System;
using System.Collections.Generic;
using MACore;
using UnityEngine;

namespace MAUnit
{
	public class Provoke : RadialPower
	{
		public ParticleSystem fearedEffect;
		public ParticleSystem healEffect;
		public float slowPercent = 0.8f;
		public float fearDuration = 6;
		public float heal = 10;
		
		private List<Monster> targets;	
		
		// Use this for initialization
		public override void OnActivateBegin ()
		{
			// Construct the area
			// ConstructArea();
			
			PlaySound();
			CooldownBegin();

			// Get all the monsters in range
			targets = GetMonstersInRange();
			
			
			// get ME!
			Defender me 	 = GetDefender();
			me.CurrentHealth = Mathf.Min(me.CurrentHealth+heal, me.health);
			
			foreach( Monster monster in targets )
			{
				// Taunt them
				monster.SetTarget( me );
				
				// Fear them!
				monster.Feared = true;
				
			}
			
			if( targets.Count > 0)
				Invoke("Unfear", fearDuration );
			
			//GameObject.Destroy(instance);
			ParticleSystem system 	= Play(healEffect, transform.position, transform.rotation);
			system.transform.parent = transform;
		}
		
		// Update is called once per frame
		public override void OnActivateUpdate ()
		{
		}
		
		// Use this for execution
		public override void OnActivateEnd ()
		{
		}
		
		private void Unfear()
		{
			foreach( Monster monster in targets )
				monster.Feared = false;
		}

		
		/// <summary>
		/// Plaies the effect.
		/// </summary>
		/// <param name='effect'>
		/// Effect.
		/// </param>
		protected override ParticleSystem Play(ParticleSystem effect, Vector3 offset, Quaternion rotation)
		{
			return base.Play(effect, offset, rotation);
		}			
		
		public override void OnAvailable(){}
	}
}

