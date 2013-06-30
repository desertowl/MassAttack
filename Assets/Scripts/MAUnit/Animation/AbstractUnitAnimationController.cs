using System;
using UnityEngine;

namespace MAUnit
{
	[HideInInspector]
	public abstract class AbstractUnitAnimationController : MonoBehaviour
	{
		protected bool _attacking;
		public bool Attacking
		{
			get
			{
				return _attacking;
			}
			set
			{
				_attacking = value;
			}
		}		
	}
}

