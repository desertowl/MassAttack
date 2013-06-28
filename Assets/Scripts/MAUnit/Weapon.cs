using UnityEngine;
using System;
using MAFX;

namespace MAUnit
{
	public class Weapon : MonoBehaviour
	{
		public float damage;
		public float range;
		public float rate;
		public float force = 0.0f;
		private float lastAttack;
		public AudioClip attack;
		public GameObject weaponFX;
		
		public void Awake()
		{
			lastAttack = 0;
		}
		
		/// <summary>
		/// Gets the cooldown.
		/// </summary>
		/// <returns>
		/// The cooldown.
		/// </returns>
		private float GetCooldown()
		{
			return Mathf.Max(0, lastAttack+rate - Time.fixedTime);
		}
		
		/// <summary>
		/// Plays the attack.
		/// </summary>
		public void Attack(Unit target)
		{
			lastAttack = Time.fixedTime;
			if( attack != null )
				UnityEngine.AudioSource.PlayClipAtPoint(attack, transform.position);
			
			// If I have a line FX, show it
			if( weaponFX != null )
			{
				
				FX effect = (Instantiate(weaponFX, transform.position, Quaternion.identity) as GameObject).GetComponent<FX>();
				effect.SetTarget(target);
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

