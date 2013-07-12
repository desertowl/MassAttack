using System;
using MAUnit;
using MACore;
using MAPlayer;
using UnityEngine;
using System.Collections.Generic;

namespace MAUnit
{
	public class Suicide : Power
	{		
		public ParticleSystem spawn;		
		// Use this for initialization
		public override void OnActivateBegin ()
		{
		}
		
		// Use this for execution
		public override void OnActivateEnd ()
		{
		}
		
		
		public override void OnAvailable()
		{
			PlaySound();
			CooldownBegin();
			
			// Kill myself
			Defender def = GetDefender();
			Game.Instance.Kill( def, Vector3.up*100 );
			Destroy(def.gameObject);
			
			Play(spawn, transform.position, transform.rotation);
		}
	}
}

