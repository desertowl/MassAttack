using System;
using UnityEngine;

namespace MAFX
{
	public class Floating : MonoBehaviour
	{
		public float amp;
		public float offset;
		private Vector3 starting;
		
		public void Awake()
		{
			starting = transform.position;
		}
		
		public void Update()
		{
			Vector3 pos = transform.position;
			pos.y		= starting.y + Mathf.Sin(Time.fixedTime + offset)*amp;
			transform.position = pos;
		}
		
	}
}

