using UnityEngine;

public class ImpactProjectile : ProjectileObjectBase
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroying) return;

        if (!other.TryGetComponent<IAbilityOwner>(out var target))
            return;

        if (target.Faction == owner.Faction)
            return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            FDamageInfo damageInfo = DamageCalculator.CalculateOutgoingDamage(attackData.context, attackData.damageDataSO, transform);
            damageable.TakeDamage(damageInfo);
        }

        PlayDestroyAnimation();
    }
}
