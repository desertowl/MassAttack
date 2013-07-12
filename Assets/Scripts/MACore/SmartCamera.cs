using System;
using UnityEngine;

namespace MACore
{
	public class SmartCamera : MonoBehaviour
	{
		private bool initialized = false;
		public void Initialize()
		{
			CombatArea area = Level.Instance.area;
			// Set me to the default position
			Vector3 centroid = area.transform.position;
			Vector3 edge	 = area.GetRadiusAtAngle(180);
			
			edge.y 	+= 5;//Mathf.Abs(edge.y-centroid.y);
			edge.x  += (centroid.x - edge.x)/3;
			transform.position = edge;
			
			initialized = true;
		}
		
		public void Update()
		{
			// Setup if needed 
			if( !initialized ) Initialize();
			
			transform.LookAt( Level.Instance.area.transform.position );
		}
	}
}

