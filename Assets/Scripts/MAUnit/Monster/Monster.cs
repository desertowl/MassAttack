using System;
using UnityEngine;
using System.Collections.Generic;
using MACore;

namespace MAUnit
{
	public class Monster : Unit
	{
		public int gold;

		public Monster ()
		{
		}
		
		/// <summary>
		/// Picks the target.
		/// </summary>
		protected override void PickTarget()
		{
			// I must have already picked a target
			if( target != null && !target.IsDead() )
				return;
			
			target = GetRandomTarget();
		}
		
		/// <summary>
		/// Picks the random target.
		/// </summary>
		public override Unit GetRandomTarget()
		{
			// Get the active defenders
			List<Defender> defenders = Game.Instance.Defenders;
			
			if( defenders.Count > 0 ) 
				return defenders[(int)(UnityEngine.Random.value*defenders.Count)];
			
			return null;
		}			
		
		/// <summary>
		/// Sets the target.
		/// </summary>
		/// <param name='target'>
		/// Target.
		/// </param>
		public void SetTarget(Unit target)
		{
			this.target = target;
		}
	}
}

