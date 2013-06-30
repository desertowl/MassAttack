using System;
using System.Collections.Generic;
using UnityEngine;

namespace MAUnit
{
	[RequireComponent(typeof (Unit))]
	public class LocalSpin : MonoBehaviour
	{
		public float speed = 1.0f;
		public List<GameObject> spinnies;
		
		
		public void Update()
		{
			if( spinnies == null ) return;
			
			foreach( GameObject spin in spinnies )
				spin.transform.RotateAroundLocal(Vector3.up, speed );
		}
	}
}

