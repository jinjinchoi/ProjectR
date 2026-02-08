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
        if (attackData.IsValid && animType != EAnimationEventType.Attack) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackData.Radius, attackData.TargetLayerMask);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                // TODO: 크리티컬 적용이나 넉백 적용등을 위해 여기서 직접 만들어서 전송해야함.
                FDamageInfo damageInfo = attackData.damageInfo;
                damageable.TakeDamage(damageInfo);
            }
        }
    }
}
