using System;
using UnityEngine;
using MACore;

namespace MAUnit
{
	[RequireComponent(typeof (Unit))]
	public class TeleportCombatAreaEdge : ActivateBehaviour
	{
		public float minAngle = -90;
		public float maxAngle = 90;
		public ParticleSystem teleport;
		
		public override void Awake()
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
			Vector3 edge 		= Game.Instance.level.area.GetRadiusAtAngle( UnityEngine.Random.Range(minAngle, maxAngle) );
			edge.y += 0.5f;
			transform.position 	= edge;
			PlaySound();
			SpawnParticleSystem(teleport);
		}
	}
}

