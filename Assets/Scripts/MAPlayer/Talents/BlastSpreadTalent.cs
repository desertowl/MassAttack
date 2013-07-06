using System;
using MAUnit;

namespace MAPlayer
{
	public class BlastSpreadTalent : Talent
	{
		public int spreadBonus;
		public override void Apply(Defender defender)
		{
			Blast blast = defender.power as Blast;
			blast.spread += spreadBonus;
		}
	}
}

