using UnityEngine;
using System;
using System.Collections.Generic;
using MACore;
using MAPlayer;

namespace MAUnit
{
	public class Defender : Unit
	{
		public EDefender type;
		public Power power;
		
		public void Awake()
		{
			base.Awake();
			
			// Accessor!
			power = GetComponent<Power>();
		}		
		
		/// <summary>
		/// Picks the target.
		/// </summary>
		protected override void PickTarget()
		{
			// I must have already picked a target
			if( target != null && !target.IsDead() )
				return;
			
			PickRandomTarget();
		}	
		
		/// <summary>
		/// Gets the identifier.
		/// </summary>
		/// <returns>
		/// The identifier.
		/// </returns>
		public int GetId()
		{
			return (int) type;
		}
		
		public override void Update()
		{
			base.Update();
			
			
			if( target != null )
				Debug.DrawLine(transform.position, target.transform.position, Color.green);
		}
		
		/// <summary>
		/// Picks the random target.
		/// </summary>
		protected void PickRandomTarget()
		{
			// Get the active defenders
			List<Monster> monsters = Game.Instance.Monsters;
			
			// The combat area!
			CombatArea area = Game.Instance.level.area;

			for( int x=0;x< monsters.Count;x ++ )
			{
				Monster monster = monsters[x];
				if( area.IsWithin(monster.transform.position) )
				{
					target = monster;
					return;
				}
			}
			target = null;
		}	
		
		/// <summary>
		/// Gets the unlocked.
		/// </summary>
		/// <returns>
		/// The unlocked.
		/// </returns>
		public int GetUnlocked()
		{
			int count =0;
			// Get all the talents on this defender
			Talent[] talents = GetComponents<Talent>();
			
			foreach( Talent t in talents )
				count += t.GetUnlocked();
			
			return count;
		}		
	}
	
	public enum EDefender
	{
		Sniper,
		Guardian,
		Berserker,
		Engineer
		
	}
}

