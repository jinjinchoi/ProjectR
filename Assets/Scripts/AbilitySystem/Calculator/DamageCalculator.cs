using UnityEngine;

public class DamageCalculator
{
    static public FDamageInfo CalculateOutgoingDamage(IAttributeSet attackerAS, IAbilityOwner instigator, EDamageType damageType, float damageMultiplier)
    {
        float damage = 0;

        if (damageType == EDamageType.Physical)
        {
            damage = attackerAS.GetAttributeValue(EAttributeType.physicalAttackPower);
        }
        if (damageType == EDamageType.Magic)
        {
            damage = attackerAS.GetAttributeValue(EAttributeType.magicAttackPower);
        }
        damage *= damageMultiplier / 100;

        float criticalChance = attackerAS.GetAttributeValue(EAttributeType.criticalChance);
        float random = Random.value * 100f;
        bool isCritical = random < criticalChance;
        if (isCritical)
        {
            damage *= 1.4f;
        }

       return new FDamageInfo(instigator, Mathf.Round(damage), damageType, isCritical);
    }

    static public float CalculateIncomingDamage(IAttributeSet victimAS, FDamageInfo damageInfo)
    {
        float defanse = 0;

        if (damageInfo.DamageType == EDamageType.Physical)
        {
            defanse = victimAS.GetAttributeValue(EAttributeType.physicalDefensePower);
        }
        if (damageInfo.DamageType == EDamageType.Magic)
        {
            defanse = victimAS.GetAttributeValue(EAttributeType.magicDefensePower);
        }

        if (defanse > 0)
            defanse /= 100;

        return damageInfo.Damage * (1 - defanse);

    }
}
