using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStrikeAbility : AbilityLogicBase
{
    private readonly List<(Vector3 left, Vector3 right)> thunderPairs = new();
    private Coroutine ThunderStrikeCo;

    // TODO: SO 파일로 옮겨야함
    float spacing = 1.5f;
    float interval = 0.2f;


    public override void ActivateAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        base.ActivateAbility(spec, context);

        PlayAbilityAnimationAndWait(spec, context, () =>
        {
            context.EndAbility(spec);
        });

        Vector3 origin = context.Owner.Transform.position;
        Vector3 rightDir = context.Owner.Transform.right;
        SetThunderPosition(origin, rightDir);

        WaitAnimationEvent(spec, context, EAnimationEventType.Attack, () =>
        {
            if (ThunderStrikeCo != null)
                context.StopCoroutine(ThunderStrikeCo);

            ThunderStrikeCo = context.StartCoroutine(SpawnThunderEffect(spec, context));
        });

    }


    private IEnumerator SpawnThunderEffect(AbilitySpec spec, IAbilitySystemContext context)
    {

        if (spec.abilityData is not DamageAbilityDataSO attackDataSO)
            yield break;
        
        FAttackData attackData = new()
        {
            context = context,
            damageDataSO = attackDataSO,
            isKnockbackFromInstigator = true
        };
        foreach (var (left, right) in thunderPairs)
        {
            PoolingManager.Instance.ActivateAttackableEffect(EEffectType.Thunder, left, attackData);
            PoolingManager.Instance.ActivateAttackableEffect(EEffectType.Thunder, right, attackData);

            yield return new WaitForSeconds(interval);
        }
        
    }

    private void SetThunderPosition(Vector3 origin, Vector3 rightDir)
    {
        thunderPairs.Clear();

        for (int i = 1; i <= 3; i++)
        {
            Vector3 left = origin - i * spacing * rightDir;
            Vector3 right = origin + i * spacing * rightDir;

            thunderPairs.Add((left, right));
        }
    }


    public override bool CanActivate(AbilitySpec spec, IAbilitySystemContext context)
    {
        return IsCooldownReady(spec) && !isActivated;
    }

    public override void OnEndAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        base.OnEndAbility(spec, context);

        spec.lastActivatedTime = Time.time;
    }
}
