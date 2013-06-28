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
			
			// Get the active defenders
			List<Defender> defenders = Game.Instance.Defenders;
			
			if( defenders.Count > 0 ) 
				target = defenders[(int)(UnityEngine.Random.value*defenders.Count)];
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

