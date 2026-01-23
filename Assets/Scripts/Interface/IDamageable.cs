using UnityEngine;

public struct FDamageInfo
{
    public IAbilityOwner Instigator;
    public float Damage;
    public EDamageType DamageType;
    public bool IsCritical;
    public Vector2 KnockbackPower;

    public FDamageInfo(IAbilityOwner instigator, float damage, EDamageType damageType, bool isCritical, Vector2 knockbackPower)
    {
        Instigator = instigator;
        Damage = damage;
        DamageType = damageType;
        IsCritical = isCritical;
        KnockbackPower = knockbackPower;
    }
}

public interface IDamageable
{
    public void TakeDamage(FDamageInfo damageInfo);
}
