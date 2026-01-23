using UnityEngine;

public struct FDamageInfo
{
    public IAbilityOwner Instigator;
    public float Damage;
    public EDamageType DamageType;
    public bool IsCritical;

    public FDamageInfo(IAbilityOwner instigator, float damage, EDamageType damageType, bool isCritical)
    {
        Instigator = instigator;
        Damage = damage;
        DamageType = damageType;
        IsCritical = isCritical;
    }
}

public interface IDamageable
{
    public void TakeDamage(FDamageInfo damageInfo);
}
