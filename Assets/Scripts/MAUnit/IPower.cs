using UnityEngine;
using System.Collections;

namespace MAUnit
{
	public interface IPower
	{
		// Use this for initialization
		void OnActivateBegin ();
		
		// Update is called once per frame
		void OnActivateUpdate ();
		
		// Use this for execution
		void OnActivateEnd ();		
	}
}