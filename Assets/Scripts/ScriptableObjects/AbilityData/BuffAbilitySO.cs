using System;
using UnityEngine;

public class BuffUIData
{
    public EAbilityId Id;
    public Sprite Icon;
    public float Duration;
}

[CreateAssetMenu(menuName = "ASC/Ability/Buff")]
public class BuffAbilitySO : DamageAbilityDataSO
{
    [Header("Buff Info")]
    public EAttributeType attribute;
    public float value;
    public float duration;
    public EEffectType effectType;
    public Sprite buffIcon;

    public override AbilityLogicBase CreateInstance()
    {
        return new BuffAbility();
    }
}
