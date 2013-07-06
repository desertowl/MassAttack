using System;
using UnityEngine;

namespace MACore
{
	public class SpawnRing : SpawnPoint
	{		
		/// <summary>
		/// Raises the draw gizmos selected event.
		/// </summary>
	    void OnDrawGizmosSelected()
		{
			float inc = 10.0f;
			Vector3 last = GetPositionAtAngle(0);
			//last.y += 1;
			
			// Set the gizmo color according to its type
			Gizmos.color = gameObject.tag == "Spawn"?Color.cyan:Color.white;
			for( float a=inc;a<=360.0f;a+= inc )
			{
				Vector3 pos = GetPositionAtAngle(a);
				//pos.y += 1;
				
				Gizmos.DrawLine(last, pos);
				//Gizmos.DrawSphere(pos, 0.3f);
				last = pos;
			}
	    }
		
		/// <summary>
		/// Gets the position at angle.
		/// </summary>
		/// <returns>
		/// The position at angle.
		/// </returns>
		/// <param name='deg'>
		/// Deg.
		/// </param>
		private Vector3 GetPositionAtAngle(float deg)
		{
			// Get the angle
			float a = Mathf.Deg2Rad * deg;
			
			// Create the position
			return new Vector3( Mathf.Sin(a) * transform.localScale.x, 0.0f, Mathf.Cos(a) * transform.localScale.z) + transform.position;
		}
		
		/// <summary>
		/// Gets the next spawn.
		/// </summary>
		/// <returns>
		/// The next spawn.
		/// </returns>
		public override Vector3 GetNextSpawn()
		{
			return GetPositionAtAngle(360.0f * UnityEngine.Random.value);
		}
	}
}

