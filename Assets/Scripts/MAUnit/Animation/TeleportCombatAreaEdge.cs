using System;
using UnityEngine;
using MACore;

namespace MAUnit
{
	[RequireComponent(typeof (Unit))]
	public class TeleportCombatAreaEdge : ActivateBehaviour
	{
		public ParticleSystem teleport;
		
		public void Awake()
		{
			CooldownBegin();
		}		
		
		public void Update()
		{
			
			if( GetCooldown() == 0 )
			{
				CooldownBegin();
				Teleport();
			}
		}
		
		public void Teleport()
		{
			SpawnParticleSystem(teleport);
			Vector3 edge 		= Game.Instance.level.area.GetRadiusAtAngle( UnityEngine.Random.value * 360 );
			transform.position 	= edge;
			PlaySound();
			SpawnParticleSystem(teleport);
		}
	}
}

