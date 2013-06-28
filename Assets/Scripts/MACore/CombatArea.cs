using System;
using UnityEngine;

namespace MACore
{
	public class CombatArea : MonoBehaviour
	{		
		/// <summary>
		/// Raises the draw gizmos selected event.
		/// </summary>
	    void OnDrawGizmosSelected()
		{
			float inc = 10.0f;
			Vector3 last = GetRadiusAtAngle(0);
			last.y += 1;
			
			// Set the gizmo color according to its type
			Gizmos.color = Color.red;
			for( float a=inc;a<=360.0f;a+= inc )
			{
				Vector3 pos = GetRadiusAtAngle(a);
				pos.y += 1;
				
				Gizmos.DrawLine(last, pos);
				//Gizmos.DrawSphere(pos, 0.3f);
				last = pos;
			}
	    }
		
		void OnGUI()
		{
			float inc = 10.0f;
			Vector3 last = GetRadiusAtAngle(0);
			last.y += 1;
			
			// Set the gizmo color according to its type
			
			for( float a=inc;a<=360.0f;a+= inc )
			{
				Vector3 pos = GetRadiusAtAngle(a);
				pos.y += 1;
			
				Debug.DrawLine(last, pos);
				//Gizmos.DrawSphere(pos, 0.3f);
				last = pos;
			}
		}
		
		/// <summary>
		/// Gets the position at angle in degrees!
		/// </summary>
		/// <returns>
		/// The position at angle.
		/// </returns>
		/// <param name='deg'>
		/// Deg.
		/// </param>
		private Vector3 GetRadiusAtAngle(float deg)
		{
			// Get the angle
			float a = Mathf.Deg2Rad * deg;
			
			// Create the position
			return new Vector3( Mathf.Sin(a) * transform.localScale.x, 0.0f, Mathf.Cos(a) * transform.localScale.z) + transform.position;
		}
		
		/// <summary>
		/// Determines whether this instance is within the specified other.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance is within the specified other; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='other'>
		/// Other.
		/// </param>
		public bool IsWithin(Vector3 other)
		{
			// Get the angle between the center and other
			float deg = Vector3.Angle(transform.position, other);
			float max = GetRadiusAtAngle(deg).magnitude;
			float dist= (transform.position - other).magnitude;

			return dist < max;
		}
	}
}


