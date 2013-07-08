using System;
using UnityEngine;

namespace MAUnit
{
	public class Launcher : Weapon
	{
		public Projectile projectile;
		
		public override void Attack(Unit target)
		{
			base.Attack(target);			
			
			// Fire a projectile at a random target
			// Unit target = owner.GetRandomTarget();
			
			// Fire away!
			Projectile instance				= GameObject.Instantiate(projectile) as Projectile;
			instance.transform.position 	= transform.position;
			//instance.transform.localPosition= Vector3.zero;
			instance.damage = damage;
			instance.Fire(this, target.DefaultTarget.transform.position);
		}
	}
}

