using System;
using System.Collections.Generic;
using UnityEngine;
using MACore;

namespace MAUnit
{
	public class Whirlwind : RadialPower
	{
		public float speedBonusPercent = 1.5f;
		public float duration = 3.0f;
		
		// Use this for initialization
		public override void OnActivateBegin ()
		{
			// Construct the area
			ConstructArea();
			GetDefender().weapon.Active = false;
		}
		
		// Update is called once per frame
		public override void OnActivateUpdate ()
		{
			GetDefender().transform.RotateAroundLocal( Vector3.up, (5*(speedBonusPercent))*Time.deltaTime);
		}
		
		// Use this for execution
		public override void OnActivateEnd ()
		{
			PlaySound();
			CooldownBegin();
			
			Invoke("EndEffect", duration);
			InvokeRepeating("AttackEveryone", 0, GetDefender().weapon.cooldown/3 );
			GetDefender().speed *= speedBonusPercent;
			GameObject.Destroy(instance);
		}
		
		/// <summary>
		/// Attacks the everyone.
		/// </summary>
		private void AttackEveryone()
		{
			// Get all the monsters in range
			List<Monster> monsters = GetMonstersInRange();
			
			Defender defender 	= GetDefender();
			Weapon weapon 		= defender.weapon;
			damage 				= weapon.damage;
			
			foreach( Monster monster in monsters )
				Game.Instance.DoDamage(defender, this, monster);
		}
		
		/// <summary>
		/// Ends the effect.
		/// </summary>
		private void EndEffect()
		{
			GetDefender().weapon.Active = true;
			
			Defender me = GetDefender();
			me.speed 	/= speedBonusPercent;
			Destroy(instance);
		}		
	}
}

