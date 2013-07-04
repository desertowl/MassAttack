using System;
using UnityEngine;
using MAUnit;

namespace MAPlayer
{
	public enum EScaleType
	{
		Add,
		Multiply
	}
	
	[System.Serializable]
	public abstract class Talent : MonoBehaviour
	{
		public new string name;
		public string desc;
		public Texture icon;
		public int cost;
		public int prerequisite;
		public int unlocked;
		public int max;
		
		public Talent ()
		{
		}
		
		public void Unlock()
		{
			SetUnlocked(unlocked+1);
		}
		public void SetUnlocked(int amount)
		{
			unlocked = Math.Min(amount, max);
		}		
		
		public int GetUnlocked()
		{
			return unlocked;
		}
		
		public bool IsCompletlyUnlocked()
		{
			return unlocked==max;
		}
		
		public abstract void Apply(Defender defender);
		
		public string GetHash()
		{
			return name;
		}
		
		public Defender GetDefender()
		{
			return gameObject.GetComponent<Defender>();
		}

	}
}

