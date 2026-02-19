using UnityEngine;

public class AreaStrikeAbility : AbilityLogicBase
{
    public override bool CanActivate(AbilitySpec spec, IAbilitySystemContext context)
    {
        return IsCooldownReadyAndNotActivated(spec);
    }

    public override void ActivateAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        base.ActivateAbility(spec, context);

        PlayAbilityAnimationAndWait(spec, context, () =>
        {
            context.EndAbility(spec);
        });

        WaitAnimationEvent(spec, context, EAnimationEventType.Attack, () =>
        {
            PerformAttackWithEffect(spec, context);
        });

    }

    private void PerformAttackWithEffect(AbilitySpec spec, IAbilitySystemContext context)
    {
        if (spec.abilityData is not Ability_AreaStrikeSO dataSO)
            return;

        Collider2D[] EnemiesCollider =
            Physics2D.OverlapBoxAll(context.Owner.Transform.position, dataSO.detectRange, 0f, dataSO.hostileTargetLayer);

        FAttackData attackData = new()
        {
            context = context,
            damageDataSO = dataSO,
            isKnockbackFromInstigator = false
        };

        foreach (var enemyCollider in EnemiesCollider)
        {
            Vector2 enemyPos = enemyCollider.transform.position;
            PoolingManager.Instance.ActivateAttackableEffect(dataSO.effectType, enemyPos, attackData);
        }

    }

}
