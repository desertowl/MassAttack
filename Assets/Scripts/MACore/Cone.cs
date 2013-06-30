using System;
using System.Collections.Generic;
using UnityEngine;

namespace MACore
{
	public class Cone
	{
		public float dir;
		public Vector3 origin;
		public float spread;
		public float range;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MACore.MACone"/> class.
		/// </summary>
		/// <param name='origin'>
		/// Origin.
		/// </param>
		/// <param name='dir'>
		/// Dir.
		/// </param>
		/// <param name='angle'>
		/// Angle.
		/// </param>
		/// <param name='range'>
		/// Range.
		/// </param>
		public Cone (Vector3 origin, float spread, float dir, float range)
		{
			this.origin 	= origin;
			this.dir		= dir;
			this.spread		= spread;
			this.range		= range;
		}
		
		/// <summary>
		/// Gets the cone bounds.
		/// </summary>
		/// <param name='upper'>
		/// Upper.
		/// </param>
		/// <param name='lower'>
		/// Lower.
		/// </param>
		public void GetConeBounds(out Vector3 upper, out Vector3 lower)
		{			
			Vector3 mag	= new Vector3(0,0,range);
			upper 		= Quaternion.Euler(0, spread/2, 0)  * mag;
			lower 		= Quaternion.Euler(0, -spread/2, 0) * mag;
		}
		
		/// <summary>
		/// Determines whether this instance is within the specified other.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance is within the specified other; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='other'>
		/// If set to <c>true</c> other.
		/// </param>
		public bool IsWithin(Vector3 other)
		{
			// Draw the cone
			Vector3 mag			= new Vector3(0,0,range);
			Vector3 endpoint	= Quaternion.Euler(0, dir, 0)  * mag;			
			
			// get the angle between the two !
			float angle = Vector3.Angle(endpoint, other);
			// Find out what direction im facing
			return (angle < spread/2);
		}
	}
}

