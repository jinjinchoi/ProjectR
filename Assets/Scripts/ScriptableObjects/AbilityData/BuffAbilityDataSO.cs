using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "ASC/Ability/Buff")]
public class BuffAbilityDataSO : DamageAbilityDataSO
{
    [Header("Buff Info")]
    public EAttributeType attribute;
    public float value;
    public float duration;
    public EEffectType effectType;
    public Image buffIcon;

    public override AbilityLogicBase CreateInstance()
    {
        return new BuffAbility();
    }
}
