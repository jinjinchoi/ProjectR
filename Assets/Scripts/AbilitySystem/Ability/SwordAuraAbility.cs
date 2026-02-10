using UnityEngine;

public class SwordAuraAbility : AbilityLogicBase
{
    public override bool CanActivate(AbilitySpec spec, IAbilitySystemContext context)
    {
        return IsCooldownReady(spec) && !isActivated;
    }

    public override void ActivateAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        base.ActivateAbility(spec, context);

        if (spec.abilityData is not Ability_SwordAuraSO data)
        {
            context.EndAbility(spec);
            return;
        }

        PlayAbilityAnimationAndWait(spec, context, () =>
        {
            context.EndAbility(spec);
        });

        WaitAnimationEvent(spec, context, EAnimationEventType.Attack, () =>
        {
            SpawnProjectile(context, data);

        });
    }

    private static void SpawnProjectile(IAbilitySystemContext context, Ability_SwordAuraSO data)
    {
        GameObject go = PoolingManager.Instance.GetPooledObject(data.projectileId);
        var projectile = go.GetComponent<TimedProjectile>();

        FAttackData attackData = new()
        {
            context = context,
            damageDataSO = data,
            isKnockbackFromInstigator = true
        };
        projectile.Init(context.Owner.AttackPoint.position, context.Owner, attackData, context.Owner.FacingDir);
    }

}
