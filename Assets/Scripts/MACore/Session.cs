using System;
using UnityEngine;
using MAPlayer;

namespace MACore
{
	public class Session : MonoBehaviour
	{
		// Class Variables
		private static Session instance;
		public static Session Instance { get {return instance;} }
		
		private Player player;
		public Player Player { get { return player; } set { player = value; } }
		
		
		// To handle loading of levels
		private Level target;
		public Level TargetLevel { get { return target; } set { target = value; } }	
		
		private GameData data;
		public GameData GameData { get { return data; } set { data = value; } }	
		
		/// <summary>
		/// Called when this instance is awoken
		/// </summary>
		public void Awake ()
		{
			instance = this;
			DontDestroyOnLoad(transform.gameObject);
		}
		
		/// <summary>
		/// Determines whether this instance can unlock the specified talent.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance can unlock the specified talent; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='talent'>
		/// If set to <c>true</c> talent.
		/// </param>
		public bool CanUnlock(Talent talent)
		{
			// Can I afford it?
			if( data.gold < talent.cost )
				return false;
			
			// Is it totally unlocked already
			if( talent.GetUnlocked() >= talent.max )
				return false;
			
			// Do I meet the pre-reqs?
			if( talent.GetDefender().GetUnlocked() < talent.prerequisite )
				return false;
			
			return true;
		}
		
		public void Unlock(Talent talent)
		{
			data.gold -= talent.cost;
			talent.Unlock();
			
			data.Save(talent.GetDefender());
			data.Save();
		}
		
		
	}
}

