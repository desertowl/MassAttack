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
			
			// Create a new defender 
			DefenderData data = new DefenderData(EDefender.Sentry, false);
			
			Defender sentry   = Game.Instance.Spawn(data);
			sentry.transform.position = transform.position;
			sentry.power.CooldownBegin();
			
			sentries.Add(sentry);
		}
		
		public override void OnAvailable(){}
	}
}

