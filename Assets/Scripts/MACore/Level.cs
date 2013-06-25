using System.Collections.Generic;
using UnityEngine;
using System;

namespace MACore
{
	public class Level : MonoBehaviour
	{
		public Texture icon;
		public float duration = 30; // THe duration of this level, in seconds
		public List<Wave> waves;
		
		private List<Wave> remaining;
		public Level ()
		{
		}
		
		public void Start()
		{
			remaining = new List<Wave>(waves);
		}
		
		public bool AllWavesSent()
		{
			return remaining.Count == 0;
		}
		
		public List<Wave> GetNextWave(float now)
		{
			// Initialize the result set
			List<Wave> nexts = new List<Wave>();
			
			// Quick sanity check
			if( remaining.Count == 0 )
				return nexts;

			// Get the percent
			float p = now/duration;
			
			foreach( Wave wave in remaining )
			{
				if( p >= wave.spawnTime )
					nexts.Add(wave);
				else break;
			}
			
			// Pop them from the remaining list
			foreach( Wave wave in nexts )
				remaining.Remove(wave);
			
			return nexts;
		}
	}
}

