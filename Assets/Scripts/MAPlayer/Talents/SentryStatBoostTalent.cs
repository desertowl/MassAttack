using System;
using MAUnit;

namespace MAPlayer
{
	public class SentryStatBoostTalent : StatBoostTalent
	{		
		/// <summary>
		/// Apply the stat change to the specified defender iff the defender is a sentry gun
		/// </summary>
		/// <param name='defender'>
		/// Defender.
		/// </param>
		public override void Apply(Defender defender)
		{
			// Get the sentry gun power
			if( defender.type == EDefender.Sentry )
				base.Apply(defender);
		}	
	}
}

