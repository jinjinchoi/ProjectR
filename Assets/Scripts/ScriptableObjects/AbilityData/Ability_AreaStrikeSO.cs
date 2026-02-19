using UnityEngine;

[CreateAssetMenu(menuName = "ASC/Ability/AreaStrike")]
public class Ability_AreaStrikeSO : DamageAbilityDataSO
{
    public EEffectType effectType;
    public Vector2 detectRange = new (20f, 20f); 

    public override AbilityLogicBase CreateInstance()
    {
        return new AreaStrikeAbility();
    }
}
