using UnityEngine;
using System.Collections;

namespace MAUnit
{
	public abstract class Power : MonoBehaviour
	{
		// Use this for initialization
		public abstract void OnActivateBegin ();
		
		// Update is called once per frame
		public abstract void OnActivateUpdate ();
		
		// Use this for execution
		public abstract void OnActivateEnd ();		
	}
}