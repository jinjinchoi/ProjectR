using UnityEngine;

public class DamageCalculator
{
    // 대미지를 입힐때 얼마나 대미지를 입힐지 구하는 함수
    static public FDamageInfo CalculateOutgoingDamage(IAbilitySystemContext context, DamageAbilityDataSO damageDataSO, Transform damageSource)
    {
        float damage = 0;

        if (damageDataSO.damageType == EDamageType.Physical)
        {
            damage = context.AttributeSet.GetAttributeValue(EAttributeType.PhysicalAttackPower);
        }
        if (damageDataSO.damageType == EDamageType.Magic)
        {
            damage = context.AttributeSet.GetAttributeValue(EAttributeType.MagicAttackPower);
        }
        damage *= damageDataSO.damageMultiplier / 100;

        float criticalChance = context.AttributeSet.GetAttributeValue(EAttributeType.CriticalChance);
        float random = Random.value * 100f;
        bool isCritical = random < criticalChance;

        return new FDamageInfo(context.Owner, damageSource, Mathf.Round(damage), damageDataSO.damageType, isCritical, damageDataSO.knockbackPower, damageDataSO.KnockbackDuration);
    }

    // 대미지를 받을때 얼마나 받을지 구하는 함수
    static public float CalculateIncomingDamage(IAttributeSet victimAS, FDamageInfo damageInfo)
    {
        float defanse = 0;

        if (damageInfo.DamageType == EDamageType.Physical)
        {
            defanse = victimAS.GetAttributeValue(EAttributeType.PhysicalDefensePower);
        }
        if (damageInfo.DamageType == EDamageType.Magic)
        {
            defanse = victimAS.GetAttributeValue(EAttributeType.MagicDefensePower);
        }

        if (defanse > 0)
            defanse /= 100;

        float damage = damageInfo.OriginalDamage * (1 - defanse);

        if (damageInfo.IsCritical) damage *= 1.4f;

        return Mathf.Round(damage);
    }
}
