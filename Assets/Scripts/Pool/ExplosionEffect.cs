using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{

    AnimationTrigger animTrigger;
    FAttackData attackData;

    private void Awake()
    {
        animTrigger = GetComponentInChildren<AnimationTrigger>();
    }

    private void OnEnable()
    {
        if (animTrigger) animTrigger.OnAnimTriggered += OnAnimTriggered;
    }

    private void OnDisable()
    {
        if (animTrigger) animTrigger.OnAnimTriggered -= OnAnimTriggered;
    }

    public void SetDamageData(FAttackData attackData)
    {
        this.attackData = attackData;
    }

    private void OnAnimTriggered(EAnimationEventType animType)
    {
        if (animType != EAnimationEventType.Attack) return;

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(transform.position, attackData.damageDataSO.attackDamageRadius, attackData.damageDataSO.hostileTargetLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                Transform KnockbackSource = attackData.isKnockbackFromInstigator ? attackData.context.Owner.OwnerTransform : transform;
                FDamageInfo damageInfo = DamageCalculator.CalculateOutgoingDamage(attackData.context, attackData.damageDataSO, KnockbackSource);
                damageable.TakeDamage(damageInfo);
            }
        }
    }
}
