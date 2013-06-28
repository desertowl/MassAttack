using System;
using UnityEngine;

namespace MAUnit
{
	public abstract class Attack : MonoBehaviour
	{
		public float damage;
		public float force = 0.0f;
		public float cooldown;
		public bool Active = true;
		public AudioClip attack;
		
		private float lastAttack;
		
		/// <summary>
		/// Begins the cooldown
		/// </summary>
		public void CooldownBegin()
		{
			lastAttack = Time.fixedTime;			
		}			
		
		/// <summary>
		/// Plaies the sound.
		/// </summary>
		public void PlaySound()
		{
			if( attack != null )
				UnityEngine.AudioSource.PlayClipAtPoint(attack, transform.position);			
		}
		
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
		public float GetCooldown()
		{
			return Mathf.Max(0, lastAttack+cooldown - Time.fixedTime);
		}		
	}
}

