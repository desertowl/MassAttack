using UnityEngine;
using System.Collections;

namespace MACore
{
	public class Billboard : MonoBehaviour
	{	 
	    void Update()
	    {
			
	        transform.LookAt(	transform.position + Camera.main.transform.rotation * Vector3.back,
								Camera.main.transform.rotation * Vector3.up);
			transform.RotateAround(Vector3.left, 90);
	    }
	}
}

