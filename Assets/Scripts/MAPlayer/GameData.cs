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
		public int unlocks;
		public int gold;
		public int level;
		public Dictionary<string, DefenderData> roster;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MAPlayer.GameData"/> class.
		/// </summary>
		public GameData ()
		{
			roster 	= new Dictionary<string, DefenderData>();
			level 	= 0;//25;
			unlocks = 0;//4;
			gold 	= 0;//10000;	
			
			roster.Add( ""+EDefender.Sniper, 	new DefenderData(EDefender.Sniper, false) );
			
			if( false )
			{
				level = 15;
				unlocks = 4;
				gold = 10000;
			}
		}
		
		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear()
		{
			if( HasGameData() )
				PlayerPrefs.DeleteKey("data");
					
			// Unlock the first two defenders by default
			//roster.Add( ""+EDefender.Sniper, 	new DefenderData(EDefender.Sniper, false) );
			//roster.Add( ""+EDefender.Guardian,	new DefenderData(EDefender.Guardian, false) );
			//roster.Add( ""+EDefender.Berserker,	new DefenderData(EDefender.Berserker, false) );
			//roster.Add( ""+EDefender.Engineer,	new DefenderData(EDefender.Engineer, false) );
		}
		
		/// <summary>
		/// Levels the complete.
		/// </summary>
		/// <param name='id'>
		/// Identifier.
		/// </param>
		/// <param name='goldEarned'>
		/// Gold earned.
		/// </param>
		public void LevelComplete(Game game)
		{
			if( game.State == EGameState.Victory && game.level.id == level )
			{
				level++;
				
				if( game.level.unlockDefenderReward )
					unlocks++;
			}
			gold += game.GetGoldEarned();
		}
		
		/// <summary>
		/// Unlock the specified type.
		/// </summary>
		/// <param name='type'>
		/// If set to <c>true</c> type.
		/// </param>
		public DefenderData Unlock(EDefender type)
		{
			// Check to see if its already unlocked
			if( !HasUnlocks() || roster.ContainsKey(""+type) )
				return null;
			
			// Add it to the roster
			DefenderData data = new DefenderData(type, false);
			roster.Add( ""+type, data);
			unlocks--;
			return data;
		}
		
		/// <summary>
		/// Determines whether this instance has unlocks.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance has unlocks; otherwise, <c>false</c>.
		/// </returns>
		public bool HasUnlocks()
		{
			return unlocks>0;
		}
		
		/// <summary>
		/// Determines whether this instance has unlocks.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance has unlocks; otherwise, <c>false</c>.
		/// </returns>
		public bool HasDefenders()
		{
			return roster.Count>0;
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
		/// Gets the defender data.
		/// </summary>
		/// <returns>
		/// The defender data.
		/// </returns>
		/// <param name='type'>
		/// Type.
		/// </param>
		public DefenderData GetDefenderData(EDefender type)
		{
			List<DefenderData> defenders = GetDefenderData();
			foreach( DefenderData data in defenders )
				if( data.GetDefenderType() == type )
					return data;
			return null;
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

