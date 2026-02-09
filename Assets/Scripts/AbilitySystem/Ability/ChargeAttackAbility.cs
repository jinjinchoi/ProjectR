using System.Collections.Generic;
using UnityEngine;

public class ChargeAttackAbility : AbilityLogicBase, IHitHandler
{
    private readonly HashSet<IDamageable> hitTargets = new();
    private DamageDealer damageDealer;
    private IAbilitySystemContext cachedContext;
    private DamageAbilityDataSO cachedDataSO;

    public override bool CanActivate(AbilitySpec spec, IAbilitySystemContext context)
    {
        return IsCooldownReady(spec) && !isActivated;
    }

    public override void ActivateAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        base.ActivateAbility(spec, context);

        cachedDataSO = spec.abilityData as DamageAbilityDataSO;
        cachedContext = context;

        if (damageDealer == null)
        {
            if (context?.Owner is BaseCharacter player && player.DamageDealer != null)
            {
                damageDealer = player.DamageDealer;
                damageDealer.Init(this);
            }
            else
            {
                Debug.LogError("[ChargeAttackAbility] DamageDealer 초기화 실패. context 또는 Owner, DamageDealer 확인 필요");
                return;
            }
        }

        if (damageDealer) damageDealer.gameObject.SetActive(true);

        PlayAbilityAnimationAndWait(spec, context, () =>
        {
            if (context.Owner is BaseCharacter character)
            {
                character.SetVelocity(0f, character.Rb.linearVelocity.y);
            }
            context.EndAbility(spec);
        });

        Charge(spec, context);

    }

    private void Charge(AbilitySpec spec, IAbilitySystemContext context)
    {
        if (context.Owner is not BaseCharacter character)
        {
            Debug.LogWarning($"[AbilityContext] Owner is not BaseCharacter. Type: {context.Owner?.GetType().Name ?? "null"}");
            return;
        }

        if (spec.abilityData is not Ability_ChargeAttackSO chargeAttackSO)
        {
            Debug.LogWarning(
                $"[Ability] Invalid abilityData type. " +
                $"Expected: {nameof(Ability_ChargeAttackSO)}, " +
                $"Actual: {spec.abilityData?.GetType().Name ?? "null"}"
            );
            return;
        }

        character.SetVelocity(character.FacingDir * chargeAttackSO.ChargeSpeed, character.Rb.linearVelocity.y);

    }

    public override void OnEndAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        base.OnEndAbility(spec, context);

        damageDealer.gameObject.SetActive(false);
        hitTargets.Clear();

        cachedContext = null;
        cachedDataSO = null;
    }

    public void OnHit(IDamageable target, Collider2D hitCollider)
    {
        if (cachedContext == null || cachedDataSO == null) return;
        if (hitTargets.Contains(target)) return;

        hitTargets.Add(target);

        FDamageInfo damageInfo = DamageCalculator.CalculateOutgoingDamage(cachedContext, cachedDataSO, cachedContext.Owner.OwnerTransform);
        target.TakeDamage(damageInfo);

    }
}
