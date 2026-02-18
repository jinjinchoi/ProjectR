using System.Collections;
using UnityEngine;

public class BuffAbility : AbilityLogicBase
{
    FModifierHandle? buffModHandle;

    public override bool CanActivate(AbilitySpec spec, IAbilitySystemContext context)
    {
        return IsCooldownReadyAndNotActivated(spec);
    }

    public override void ActivateAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        base.ActivateAbility(spec, context);

        if (spec.abilityData is not BuffAbilityDataSO buffData)
        {
            DebugHelper.LogWarning("Invalid SO type Detected");
            context.EndAbility(spec);
            return;
        }

        PlayAbilityAnimationAndWait(spec, context, () =>
        {
            context.EndAbility(spec);
        });

        PoolingManager.Instance.ActivateEffect(buffData.effectType, context.Owner.Transform.position);

        FAttributeModifier modifier = new()
        {
            attributeType = buffData.attribute,
            operation = EModifierOp.Add,
            policy = EModifierPolicy.Infinite,
            value = buffData.value,
        };
        buffModHandle = context.ApplyModifier(modifier);

        context.StartCoroutine(BuffCo(buffData.duration, spec, context));
        BuffUIData buffUiData = new()
        {
            Duration = buffData.duration,
            Icon = buffData.buffIcon,
            Id = buffData.abilityId
        };
        context.RaiseOnBuffActivated(buffUiData);
    }

    private IEnumerator BuffCo(float duration, AbilitySpec spec, IAbilitySystemContext context)
    {
        yield return new WaitForSeconds(duration);

        if (spec.abilityData is not BuffAbilityDataSO buffData) yield break;

        if (buffModHandle.HasValue)
        {
            context.AttributeSet.RemoveModifier(buffModHandle.Value);
            buffModHandle = null;
            context.RaiseOnBuffDeactivated(buffData.abilityId);
        }
    }
}
