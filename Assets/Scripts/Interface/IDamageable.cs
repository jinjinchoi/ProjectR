using UnityEngine;

public struct FDamageInfo
{
    public IAbilityOwner Instigator;
    public Transform DamageSource;
    public float OriginalDamage;
    public EDamageType DamageType;
    public bool IsCritical;
    public Vector2 KnockbackPower;
    public float KnockbackDuration;

    public FDamageInfo(IAbilityOwner instigator, Transform damageSource, float damage, EDamageType damageType, bool isCritical, Vector2 knockbackPower, float knockbackDuration)
    {
        Instigator = instigator;
        DamageSource = damageSource;
        OriginalDamage = damage;
        DamageType = damageType;
        IsCritical = isCritical;
        KnockbackPower = knockbackPower;
        KnockbackDuration = knockbackDuration;
    }
}

public interface IDamageable
{
    public void TakeDamage(FDamageInfo damageInfo);
}
