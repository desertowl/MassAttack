using System;
using System.Collections.Generic;
using MACore;
using UnityEngine;

namespace MAUnit
{
	public class Provoke : RadialPower
	{
		public float slowPercent = 0.8f;
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
				monster.speed = -Mathf.Abs(monster.speed*2);
			}
			
			GameObject.Destroy(instance);
		}
		
		public override void OnAvailable(){}
	}
}

