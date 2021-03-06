using UnityEngine;
using System.Collections;

namespace MAUnit
{
	public abstract class Power : Attack
	{
		public Texture label;
		public string displayName;
		public string desc;
		
		[HideInInspector]
		public bool Activating = false;
		
		// Use this for initialization
		public abstract void OnActivateBegin ();
		
		// Use this for execution
		public abstract void OnActivateEnd ();	
		
		public abstract void OnAvailable();
		
		
		/// <summary>
		/// Gets the defender.
		/// </summary>
		/// <returns>
		/// The defender.
		/// </returns>
		protected Defender GetDefender()
		{
			return GetComponent<Defender>();
		}
		
		protected virtual ParticleSystem Play(ParticleSystem effect, Vector3 offset, Quaternion rotation)
		{
			if( effect == null )
				return null;

			ParticleSystem inst = Instantiate(effect, offset, rotation) as ParticleSystem;
			inst.transform.Rotate( new Vector3(270, 0, 0) );
			Destroy(inst.gameObject, effect.duration*2f);
			
			return inst;
		}			
	}
}