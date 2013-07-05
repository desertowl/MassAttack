using UnityEngine;
using System;
using System.Collections.Generic;
using MACore;
using MAPlayer;

namespace MAUnit
{
	public class Defender : Unit
	{

		private static readonly Color BERSERKER = new Color(1,0,0);
		private static readonly Color GUARDIAN  = new Color(0,0,1);
		private static readonly Color SNIPER  	= new Color(1,0,1);
		private static readonly Color ENGINEER 	= new Color(0,1,0);		
		
		public EDefender type;
		
		
		[HideInInspector]
		public Power power;
		
		public override void Awake()
		{
			powerTargeting = false;
			
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
			
			PickNearestTarget();
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
		/// Picks the nearest target.
		/// </summary>
		protected void PickNearestTarget()
		{
			// Get the active defenders
			List<Monster> monsters = Game.Instance.Monsters;
			
			float nearestDist = float.MaxValue;
			Monster nearest = null;

			for( int x=0;x< monsters.Count;x ++ )
			{
				Monster monster = monsters[x];
				if( IsWithinCombatArea(monster) )
				{
					float dist = Vector3.Distance(transform.position, monster.transform.position);
					
					if( dist < nearestDist )
					{
						dist 	= nearestDist;
						nearest = monster;
						
						if( dist < 2 )
							break;
					}
				}
			}
			target = nearest;			
		}
		
		/// <summary>
		/// Determines whether this instance is within combat area the specified subject.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance is within combat area the specified subject; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='subject'>
		/// If set to <c>true</c> subject.
		/// </param>
		private bool IsWithinCombatArea(Unit subject)
		{
			return Game.Instance.level.area.IsWithin(subject.transform.position);
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
		
		/// <summary>
		/// Gets the color.
		/// </summary>
		/// <returns>
		/// The color.
		/// </returns>
		/// <param name='type'>
		/// Type.
		/// </param>
		public static Color GetDefenderColor(EDefender type)
		{
			switch( type )
			{
				case EDefender.Berserker:
					return BERSERKER;
				case EDefender.Guardian:
					return GUARDIAN;
				case EDefender.Sniper:
					return SNIPER;				
			}
			return ENGINEER;
		}			
	}
	
	public enum EDefender
	{
		Sniper,
		Guardian,
		Berserker,
		Engineer,
		Sentry,
	}
}

