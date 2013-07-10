using System.Collections.Generic;
using UnityEngine;
using System;

namespace MACore
{
	public class Level : MonoBehaviour
	{
		public Material sky;
		public bool unlockDefenderReward = false;
		public float duration = 30; // THe duration of this level, in seconds
		public List<Wave> waves;
		
		[HideInInspector]
		public int id; // this actually comes from PLANET!!!
		
		private List<Wave> remaining;
		public CombatArea area;
		
		public Level ()
		{			
		}
		
		public virtual void Start()
		{
			/*
			Camera.main.clearFlags = CameraClearFlags.Color;
			if( sky != null )
			{
				RenderSettings.skybox = sky;
				Camera.main.clearFlags = CameraClearFlags.Skybox;
			}
			*/

			remaining = new List<Wave>(waves);
			
			// get the combat area
			//GameObject obj 	= GameObject.FindGameObjectWithTag("Combat Area");
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

