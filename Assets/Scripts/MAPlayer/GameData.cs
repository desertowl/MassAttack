using System;
using System.Collections.Generic;
using UnityEngine;
using MACore;
using MAUnit;
using JsonFx.Json;

namespace MAPlayer
{	
	/// <summary>
	/// Game data.
	/// </summary>
	public class GameData
	{
		public int gold;
		public int level;
		public Dictionary<string, DefenderData> roster;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MAPlayer.GameData"/> class.
		/// </summary>
		public GameData ()
		{
			roster 	= new Dictionary<string, DefenderData>();
			level 	= 0;
			gold 	= 10000;			
		}
		
		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear()
		{
			if( HasGameData() )
				PlayerPrefs.DeleteKey("data");
					
			// Unlock the first two defenders by default
			roster.Add( ""+EDefender.Sniper, 	new DefenderData(EDefender.Sniper, false) );
			roster.Add( ""+EDefender.Berserker,new DefenderData(EDefender.Berserker, false) );
		}
		
		/// <summary>
		/// Gets the defender data.
		/// </summary>
		/// <returns>
		/// The defender data.
		/// </returns>
		public List<DefenderData> GetDefenderData()
		{
			return new List<DefenderData>(roster.Values);
		}		
		
		/// <summary>
		/// Save this instance.
		/// </summary>
		public void Save()
		{
			string data = MAUtil.JsonEncode(this);
			PlayerPrefs.SetString("data", data);
			
			PlayerPrefs.Save();
		}
		
		/// <summary>
		/// Save the specified defender.
		/// </summary>
		/// <param name='defender'>
		/// Defender.
		/// </param>
		public void Save(Defender defender)
		{
			// Get the defender data for this defender
			DefenderData dd 	= new DefenderData(defender.type, false);
			
			// Save all the defender information
			Talent [] talents 	= defender.GetComponents<Talent>();
			
			foreach( Talent talent in talents )
				dd.Add(talent);
			
			if( roster.ContainsKey(""+defender.type) )
				roster.Remove(""+defender.type);
			roster.Add(""+defender.type, dd);
		}
		
		/// <summary>
		/// Load this instance.
		/// </summary>
		public static GameData Load()
		{
			GameData gd;
			
			string data = PlayerPrefs.GetString("data");
			
			if( data.Length > 0 )
				gd = MAUtil.JsonDecode<GameData>(data);
			else
				gd = new GameData();
			
			Debug.Log("ROSTER: " + data );
			Debug.Log("ROSTER: " + gd.roster.Count );

			return gd;
		}
		
		/// <summary>
		/// Determines whether this instance has game data.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance has game data; otherwise, <c>false</c>.
		/// </returns>
		public static bool HasGameData()
		{
			return PlayerPrefs.HasKey("data");
		}
	}
}

