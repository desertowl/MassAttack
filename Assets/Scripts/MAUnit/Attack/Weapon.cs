using UnityEngine;
using System;
using MAFX;

namespace MAUnit
{
	public class Weapon : Attack
	{
		[HideInInspector]
		public Unit owner;		
		public float range;
		public GameObject weaponFX;

		
		/// <summary>
		/// Plays the attack.
		/// </summary>
		public virtual void Attack(Unit target)
		{
			CooldownBegin();
			PlaySound();
			
			// If I have a line FX, show it
			if( weaponFX != null )
			{
				
				GameObject effect 	= Instantiate(weaponFX, transform.position, Quaternion.identity) as GameObject;
				if( effect == null )return;
				
				FX fx 				= effect.GetComponent<FX>();
				
				if( fx != null )
				{
					fx.SetTarget(target);
				}
				else
				{
					// Rotate such that I am aiming at my target
					effect.transform.LookAt(target.transform.position);
				}
				
			}
		}	
		
		/// <summary>
		/// Determines whether this instance can hit the specified target.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance can hit the specified source target; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='source'>
		/// If set to <c>true</c> source.
		/// </param>
		/// <param name='target'>
		/// If set to <c>true</c> target.
		/// </param>
		public bool CanHit(Unit source, Unit target)
		{			
			// This weapon is still on cooldown
			if( GetCooldown() > 0 ) return false;			
			
			// Get the distance between the source and the target
			return true;
		}
		
		
		/// <summary>
		/// Determines whether this instance can hit the specified target.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance can hit the specified source target; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='source'>
		/// If set to <c>true</c> source.
		/// </param>
		/// <param name='target'>
		/// If set to <c>true</c> target.
		/// </param>
		public bool IsInRange(Unit source, Unit target)
		{
			// Get the distance between the source and the target
			float dist = Vector3.Distance(source.transform.position, target.transform.position);
			
			// Get the radius of each unit
			dist -= source.GetRadius() + target.GetRadius();
			
			return dist < range;
		}
	}
}

