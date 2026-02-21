using System.Collections.Generic;
using UnityEngine;

public class TimedProjectile : ProjectileObjectBase
{
    // 한번 피해 입힌 대상을 다시 피해 입힐 수 있는지 설정하는 변수.
    [SerializeField] private bool canMultiHit = false;
    [SerializeField] private EEffectType hitEffect;

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

        Vector2 contactPoint = other.ClosestPoint(transform.position);
        if (hitEffect != EEffectType.None)
            PoolingManager.Instance.ActivateEffect(hitEffect, contactPoint);

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<IAbilityOwner>(out var target))
            return;

        if (!hitTargets.Contains(target) && canMultiHit)
            hitTargets.Remove(target);
    }

    public override void OnLifeTimeExpired()
    {
        base.OnLifeTimeExpired();

        hitTargets.Clear();
    }
}
