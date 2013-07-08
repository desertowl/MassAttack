using UnityEngine;
using System;

namespace MACore
{
	public enum EMonsterType
	{
		Weenie,
		Vector,
		Heap,
		Kobol,
		
		// Bosses
		BitBoss,
		VectorBoss,
		HeapBoss,
		KobolBoss,
		MegaBoss,
		
		// Specials
		RocketHeap,
	}
	
	[System.Serializable]
	public class Wave
	{
		public float spawnTime;
		public EMonsterType type;
		public int count;
	}
}

