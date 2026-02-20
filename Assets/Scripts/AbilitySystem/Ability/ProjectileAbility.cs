using UnityEngine;

public class ProjectileAbility : AbilityLogicBase
{
    public override bool CanActivate(AbilitySpec spec, IAbilitySystemContext context)
    {
        return IsCooldownReady(spec) && !isActivated;
    }

    public override void ActivateAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        base.ActivateAbility(spec, context);

        if (spec.abilityData is not Ability_ProjectileSO data)
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

    private static void SpawnProjectile(IAbilitySystemContext context, Ability_ProjectileSO data)
    {
        GameObject go = PoolingManager.Instance.GetPooledObject(data.projectileId);
        var projectile = go.GetComponent<ProjectileObjectBase>();

        FAttackData attackData = new()
        {
            context = context,
            damageDataSO = data,
            isKnockbackFromInstigator = true
        };
        projectile.Init(context.Owner.ProjectilePoint.position, context.Owner, attackData, context.Owner.FacingDir);
    }

}
