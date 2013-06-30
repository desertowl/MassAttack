using System;
using UnityEngine;

namespace MAUnit
{
	[RequireComponent(typeof (Defender))]
	public class SentryAnimationController  : AbstractUnitAnimationController
	{
		public GameObject yaw;
		public GameObject barrel;

		
		private bool spinning;
		
		
		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start()
		{
			spinning = false;
		}
		
		/// <summary>
		/// Fixeds the update.
		/// </summary>
		public void Update()
		{
			Unit unit = GetComponent<Unit>();
			
			if( unit.IsDead() ) return;
			
			
			// Get my target direction
			Vector3 dir = (unit.GetTargetPosition()-unit.transform.position).normalized;
			
			//yaw.transform.rotation = Quaternion.LookRotation(dir);
			float targetAngle = Quaternion.LookRotation(dir).eulerAngles.y;
			float currentAngle= yaw.transform.rotation.eulerAngles.y;
			
			yaw.transform.RotateAround(Vector3.up, (targetAngle-currentAngle) * 0.2f * Time.deltaTime);

			// If aim attacking, sping the barrel
			if( Attacking )
			{
				Attacking 	= false;
				spinning	= true;
				Invoke("CancelBarrelRoll", 1);
			}
			
			if( spinning )
				barrel.transform.RotateAround(Vector3.right, 3*Time.deltaTime);			
		}
		
		private void CancelBarrelRoll()
		{
			spinning = false;
		}
	}
}


