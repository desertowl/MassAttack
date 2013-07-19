using System;
using UnityEngine;
using MACore;

namespace MAUnit
{
	[RequireComponent(typeof (Unit))]
	public class VanishCombatAreaEdge : ActivateBehaviour
	{
		public float minAngle = -90;
		public float maxAngle = 90;
		public ParticleSystem teleport;
		
		public override void Awake()
		{
			CooldownBegin();
		}		
		
		/// <summary>
		/// Update this instance.
		/// </summary>
		public void Update()
		{
			// dont vanish if your dead
			if( GetComponent<Unit>().IsDead() )
				return;
			
			if( GetCooldown() == 0 )
			{
				CooldownBegin();
				Vanish();
				
				Invoke("Reappear", cooldown/2 );
			}
		}
		
		/// <summary>
		/// Teleport this instance.
		/// </summary>
		public void Vanish()
		{	
			gameObject.SetActive(false);
			

			PlaySound();
			SpawnParticleSystem(teleport);
		}		
		
		/// <summary>
		/// Teleport this instance.
		/// </summary>
		public void Reappear()
		{			
			Vector3 edge 		= Game.Instance.level.area.GetRadiusAtAngle( UnityEngine.Random.Range(minAngle, maxAngle) );
			edge.y 				+= 0.5f;
			transform.position 	= edge;
			gameObject.SetActive(true);
			
			PlaySound();
			SpawnParticleSystem(teleport);
		}
	}
}

