using UnityEngine;
using System.Collections;

namespace MAUnit
{
	
	public class SimpleSet : MonoBehaviour
	{
		public GameObject subject;
	
		// Use this for initialization
		void Awake ()
		{
			
		}
		
		// Update is called once per frame
		void Update ()
		{
			subject.transform.localPosition = transform.position;
		}
	}
}