using System;
using MAUnit;

namespace MAPlayer
{
	public class JusticeSpreadTalent : Talent
	{
		public override void Apply(Defender defender)
		{
			Provoke power = defender.power as Provoke;
			power.healAll = true;
		}
	}
}

