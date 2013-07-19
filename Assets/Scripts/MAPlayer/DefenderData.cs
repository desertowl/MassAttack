using System;
using System.Collections.Generic;
using UnityEngine;
using MACore;
using MAUnit;

namespace MAPlayer
{
	/// <summary>
	/// Defender data.
	/// </summary>
	public class DefenderData
	{
		public int id;
		public bool bLocked;
		public bool taughtPower;
		public Dictionary<string, int> upgrades;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MAPlayer.DefenderData"/> class.
		/// </summary>
		public DefenderData()
		{
			taughtPower = false;
			bLocked		= true;
			this.id		= 0;
			upgrades 	= new Dictionary<string, int>();			
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MAPlayer.DefenderData"/> struct.
		/// </summary>
		/// <param name='type'>
		/// Type.
		/// </param>
		public DefenderData(EDefender type, bool locked)
		{
			taughtPower = false;
			bLocked		= locked;
			this.id		= (int)type;
			upgrades 	= new Dictionary<string, int>();
		}
		
		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <returns>
		/// The type.
		/// </returns>
		public EDefender GetDefenderType()
		{
			return (EDefender)id;
		}
		
		/// <summary>
		/// Add the specified talent.
		/// </summary>
		/// <param name='talent'>
		/// Talent.
		/// </param>
		public void Add(Talent talent)
		{
			upgrades.Add(talent.GetHash(), talent.GetUnlocked());			
		}
		
		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <returns>
		/// The count.
		/// </returns>
		public int GetCount()
		{
			int count =0;
			foreach( int i in upgrades.Values)
				count += i;
			return count;
		}
		
		/// <summary>
		/// Gets the defender.
		/// </summary>
		/// <returns>
		/// The defender.
		/// </returns>
		public Defender GetDefender()
		{
			
			GameObject go 		= Resources.Load("Defenders/"+GetDefenderType() ) as GameObject;
			
			if( go == null )
				Debug.LogError("Unable to load resource: Defenders/"+GetDefenderType() );			
			
			Defender defender 	= go.GetComponent<Defender>();
			
			// Apply all the talents
			Talent [] talents 	= defender.GetComponents<Talent>();
			
			foreach( Talent talent in talents )
			{
				int count = 0;
				if( upgrades.ContainsKey(talent.GetHash()) )
					count = upgrades[talent.GetHash()];
				
				// Set the count
				talent.SetUnlocked(count);
				
			}
			
			return defender;
		}	
		
		public override string ToString()
		{
			string result = GetDefenderType() + "? " + bLocked + " ("+upgrades.Count+") \n";
			
			foreach( string key in upgrades.Keys )
				result += " 	"+key+": " + upgrades[key] + "\n";
			return result;
		}
	}
}

