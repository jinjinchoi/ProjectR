using System.Collections.Generic;
using UnityEngine;

public class TimedProjectile : ProjectileObjectBase
{
    [SerializeField] private bool canMultiHit = false;

    private readonly HashSet<IAbilityOwner> hitTargets = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroying) return;

        if (!other.TryGetComponent<IAbilityOwner>(out var target))
            return;

        if (hitTargets.Contains(target))
            return;

        hitTargets.Add(target);

        if (target.Faction == owner.Faction)
            return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            FDamageInfo damageInfo = DamageCalculator.CalculateOutgoingDamage(attackData.context, attackData.damageDataSO, transform);
            damageable.TakeDamage(damageInfo);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<IAbilityOwner>(out var target))
            return;

        if (!hitTargets.Contains(target) && canMultiHit)
            hitTargets.Remove(target);
    }
}
