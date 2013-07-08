using System;
using MAUnit;

namespace MAPlayer
{
	public enum EStatType
	{
		WeaponDamage,
		WeaponSpeed,
		WeaponRange,
		
		DefenderHealth,
		DefenderArmor,
		DefenderSpeed,
		
		PowerDamage,
		PowerCooldown,
		
		WeaponAlternate1Damage,
		WeaponAlternate1Speed,
		WeaponAlternate1Range,		
	}
	
	[System.Serializable]
	public class StatBoostTalent : Talent
	{
		public float bonus;
		public EScaleType scaleType;
		public EStatType statType;
		
		/// <summary>
		/// Apply the stat change to the specified defender.
		/// </summary>
		/// <param name='defender'>
		/// Defender.
		/// </param>
		public override void Apply(Defender defender)
		{
			switch (statType)
			{
				case EStatType.WeaponDamage:
					GetNewStat(ref defender.weapon.damage);
					break;
				case EStatType.WeaponSpeed:
					GetNewStat(ref defender.weapon.cooldown);
					break;
				case EStatType.WeaponRange:
					GetNewStat(ref defender.weapon.range);
					break;
				
				case EStatType.DefenderHealth:
					GetNewIntStat(ref defender.health);
					defender.CurrentHealth = defender.health;
					break;
				case EStatType.DefenderArmor:
					GetNewStat(ref defender.armor);
					break;	
				case EStatType.DefenderSpeed:
					GetNewStat(ref defender.speed);
					break;	
				
				case EStatType.PowerDamage:
					GetNewStat(ref defender.power.damage);
					break;
				
				case EStatType.PowerCooldown:
					GetNewStat(ref defender.power.cooldown);
					break;	
				
				case EStatType.WeaponAlternate1Damage:
					GetNewStat(ref defender.alternates[0].damage);
					break;
				case EStatType.WeaponAlternate1Speed:
					GetNewStat(ref defender.alternates[0].cooldown);
					break;
				case EStatType.WeaponAlternate1Range:
					GetNewStat(ref defender.alternates[0].range);
					break;
			}
		}
		
		private void GetNewStat(ref float stat)
		{
			if( scaleType == EScaleType.Add )
				stat += bonus;
			else
				stat *= bonus;
		}
		
		private void GetNewIntStat(ref int stat)
		{
			float temp = (float) stat;
			GetNewStat(ref temp);
			stat = (int)temp;
		}		
	}
}

