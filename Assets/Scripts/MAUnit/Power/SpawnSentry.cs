using System;
using MAUnit;
using MACore;
using MAPlayer;
using UnityEngine;
using System.Collections.Generic;

namespace MAUnit
{
	public class SpawnSentry : Power
	{
		public ParticleSystem spawn;
		public float lifetime;
		private List<Defender> sentries = new List<Defender>();
		// Use this for initialization
		public override void OnActivateBegin ()
		{
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
			Play(spawn, transform.position, transform.rotation);
			
			// Create a new defender 
			DefenderData data = new DefenderData(EDefender.Sentry, false);
			
			
			Defender sentry   = Game.Instance.Spawn(data);
			sentry.transform.position = transform.position;
			sentry.power.CooldownBegin();
			
			
			// Apply all the sentry stat boost talents to the sentry gun
			SentryStatBoostTalent [] talents 	= GetDefender().GetComponents<SentryStatBoostTalent>();
			foreach( SentryStatBoostTalent talent in talents )
				for( int c=0;c<talent.GetUnlocked();c++ )
					talent.Apply(sentry);			
			
			sentries.Add(sentry);
		}
		
		public override void OnAvailable(){}
	}
}

