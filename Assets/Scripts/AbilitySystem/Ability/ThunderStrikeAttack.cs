using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStrikeAttack : AbilityLogicBase
{
    private List<(Vector3 left, Vector3 right)> thunderPairs = new();
    private Coroutine ThunderStrikeCo;

    // TODO: SO 파일로 옮겨야함
    float spacing = 1.5f;
    float interval = 0.2f;

    bool isActive = false;

    public override void ActivateAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        isActive = true;

        PlayAnimationAndWait(spec, context, () =>
        {
            context.EndAbility(spec);
        });

        Vector3 origin = context.Owner.OwnerTransform.position;
        Vector3 rightDir = context.Owner.OwnerTransform.right;
        SetThunderPosition(origin, rightDir);

        WaitAnimationEvent(spec, context, EAnimationEventType.Attack, () =>
        {
            if (ThunderStrikeCo != null)
                context.StopCoroutine(ThunderStrikeCo);

            ThunderStrikeCo = context.StartCoroutine(SpawnThunderEffect());
        });

    }


    private IEnumerator SpawnThunderEffect()
    {
        foreach (var pair in thunderPairs)
        {
            EffectManager.Instance.ActivateEffect(EEffectType.Thunder, pair.left);
            EffectManager.Instance.ActivateEffect(EEffectType.Thunder, pair.right);

            // Apply Damage

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
        return IsCooldownReady(spec) && !isActive;
    }

    public override void CancelAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        context.EndAbility(spec);
    }

    public override void OnEndAbility(AbilitySpec spec, IAbilitySystemContext context)
    {
        spec.lastActivatedTime = Time.time;
        isActive = false;
    }
}
