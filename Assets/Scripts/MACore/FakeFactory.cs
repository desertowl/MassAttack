using System;
using MAUnit;
using MAPlayer;
using UnityEngine;

namespace MACore
{
	public class FakeFactory : MonoBehaviour
	{
		// Class variable
		private static FakeFactory instance;
		
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns>
		/// The instance.
		/// </returns>
		public static FakeFactory GetInstance()
		{
			return instance;
		}		
		
		public FakeFactory ()
		{
		}
		
		public void Awake()
		{
			instance = this;
		}
	}
}

