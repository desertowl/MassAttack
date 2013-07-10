using System;
using UnityEngine;

namespace MAUnit
{
	public abstract class Attack : ActivateBehaviour
	{
		public float damage;
		public float force = 0.0f;
		
		[HideInInspector]
		public bool Active = true;

		public bool alwaysApplyForce = false;
	}
}

