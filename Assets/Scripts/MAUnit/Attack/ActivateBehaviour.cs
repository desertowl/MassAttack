using System;
using UnityEngine;

namespace MAUnit
{
	public abstract class ActivateBehaviour : MonoBehaviour
	{
		public float spinup;
		public float cooldown;
		public AudioClip attack;
		public bool PlayAudioAtCamera = false;
		
		private float lastActivate;
		
		/// <summary>
		/// Begins the cooldown
		/// </summary>
		public void CooldownBegin()
		{
			lastActivate = Time.fixedTime;			
		}			
		
		/// <summary>
		/// Plaies the sound.
		/// </summary>
		public void PlaySound()
		{
			if( attack != null )
				UnityEngine.AudioSource.PlayClipAtPoint(attack, PlayAudioAtCamera?Camera.main.transform.position:transform.position);
		}
		
		public virtual void Awake()
		{
			lastActivate = 0;
		}
		
		/// <summary>
		/// Gets the cooldown.
		/// </summary>
		/// <returns>
		/// The cooldown.
		/// </returns>
		public float GetCooldown()
		{
			return Mathf.Max(0, lastActivate+cooldown - Time.fixedTime);
		}
		
		
		/// <summary>
		/// Spawns the particle.
		/// </summary>
		/// <param name='system'>
		/// System.
		/// </param>
		protected void SpawnParticleSystem(ParticleSystem system)
		{
			// Sanity check
			if( system == null )
				return;
			
			ParticleSystem effect = Instantiate(system, transform.position, Quaternion.identity) as ParticleSystem;
			Destroy(effect.gameObject, system.duration);			
		}
	}
}

