using System;
using UnityEngine;

public class EnemyCharacter : BaseCharacter
{
    public event Action<FDamageInfo> OnHit;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void TakeDamage(FDamageInfo damageInfo)
    {
        base.TakeDamage(damageInfo);

        OnHit?.Invoke(damageInfo);

    }
}
