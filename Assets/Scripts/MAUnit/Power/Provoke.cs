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
		
		[HideInInspector]
		public bool healAll = false;
		
		private List<Monster> targets;	
		
		/// <summary>
		/// Raises the activate begin event.
		/// </summary>
		public override void OnActivateBegin ()
		{
			// Construct the area
			// ConstructArea();
			
			PlaySound();
			CooldownBegin();

			// Get all the monsters in range
			targets = GetMonstersInRange();
			
			
			// Heal Everyone!
			if( healAll )
			{
				foreach( Defender def in Game.Instance.Defenders )
				{
					if( def.IsDead() )
						continue;
					
					Heal ( def );
				}
			}
			else
			{
				Heal ( GetDefender() );				
			}
			
			Defender me = GetDefender();
			foreach( Monster monster in targets )
			{
				// Taunt them
				monster.SetTarget( me );
				
				// Fear them!
				monster.Feared = true;
			}
			
			Vector3 scalar	= me.transform.localScale;
			scalar.x 		*= 1.05f;
			scalar.y 		*= 1.05f;
			scalar.z 		*= 1.05f;
			me.transform.localScale = scalar;
			me.armor += 1;
			
			if( targets.Count > 0)
				Invoke("Unfear", fearDuration );
		}
		
		/// <summary>
		/// Heal the specified target.
		/// </summary>
		/// <param name='target'>
		/// Target.
		/// </param>
		private void Heal(Defender target)
		{
			target.CurrentHealth = Mathf.Min(target.CurrentHealth+damage, target.health);	
			
			ParticleSystem system 	= Play(healEffect, target.transform.position, target.transform.rotation);
			system.transform.parent = target.transform;			
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

