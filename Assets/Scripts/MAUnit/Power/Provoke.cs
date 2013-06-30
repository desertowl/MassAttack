using System;
using System.Collections.Generic;
using MACore;
using UnityEngine;

namespace MAUnit
{
	public class Provoke : RadialPower
	{
		public ParticleSystem healEffect;
		public Light healingLight;
		public float slowPercent = 0.8f;
		public float fearDuration = 6;
		public float heal = 10;
		
		private List<Monster> targets;	
		
		// Use this for initialization
		public override void OnActivateBegin ()
		{
			// Construct the area
			ConstructArea();
		}
		
		// Update is called once per frame
		public override void OnActivateUpdate ()
		{
		}
		
		// Use this for execution
		public override void OnActivateEnd ()
		{
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
			
			GameObject.Destroy(instance);
			PlayEffect(healEffect);
		}
		
		private void Unfear()
		{
			foreach( Monster monster in targets )
				monster.Feared = false;
		}
		
		protected void PlayEffect(ParticleSystem effect)
		{
			if( effect == null )
				return;

			ParticleSystem inst = Instantiate(effect, transform.position, transform.rotation) as ParticleSystem;
			inst.transform.Rotate( new Vector3(270, 0, 0) );
			Destroy(inst.gameObject, effect.duration);
			
			healingLight.enabled = true;
			healingLight.intensity = 1;
		}		
		
		public void FixedUpdate()
		{
			if( healingLight.enabled )
			{
				healingLight.intensity -= 0.1f;
				
				if( healingLight.intensity <= 0 )
					healingLight.enabled = false;
			}
		}
		
		public override void OnAvailable(){}
	}
}

