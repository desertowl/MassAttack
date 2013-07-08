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
		BitBoss,
		VectorBoss,
		HeapBoss,
		KobolBoss,
		MegaBoss
	}
	
	[System.Serializable]
	public class Wave
	{
		public float spawnTime;
		public EMonsterType type;
		public int count;
	}
}

