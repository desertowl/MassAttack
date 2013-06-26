using UnityEngine;
using System;

namespace MACore
{
	public enum EMonsterType
	{
		Weenie,
		Solider,
		Disruptor
	}
	
	[System.Serializable]
	public class Wave
	{
		public float spawnTime;
		public EMonsterType type;
		public int count;
	}
}

