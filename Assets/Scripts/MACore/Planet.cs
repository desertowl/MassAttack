using System;
using UnityEngine;

namespace MACore
{
	public class Planet : MonoBehaviour
	{
		public int id;
		public Level level;
		
		private bool isCurrent = false;
		private Vector3 defaultScale;
		public void Awake()
		{
			defaultScale = transform.lossyScale;
		}
		
		public void SetCurrent(bool isCurrent)
		{
			isCurrent = true;
		}
		
		public void Update()
		{
			if( !isCurrent ) return;
			
			// Goes from 0 to 1
			float mod = Mathf.Sin(Time.fixedTime)/2 + 0.5f;
			
			Vector3 nextScale = new Vector3();
			nextScale.x = defaultScale.x + (mod*defaultScale.x);
			nextScale.y = defaultScale.x;
			nextScale.z = defaultScale.z;
			
			transform.localScale = nextScale;
			
		}
	}
}

