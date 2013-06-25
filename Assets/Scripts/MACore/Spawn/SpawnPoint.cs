using System;
using UnityEngine;

namespace MACore
{
	public class SpawnPoint : MonoBehaviour
	{
		public SpawnPoint ()
		{
		}
		
	    void OnDrawGizmosSelected()
		{
			if( tag == "Respawn" )
	        	Gizmos.color = Color.white;
			else
				Gizmos.color = Color.yellow;
			
	        Gizmos.DrawSphere(transform.position, 0.3f);
	    }
		
		public virtual Vector3 GetNextSpawn()
		{
			return transform.position;
		}
	}
}

