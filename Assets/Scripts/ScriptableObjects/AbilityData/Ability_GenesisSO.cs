using UnityEngine;

[CreateAssetMenu(menuName = "ASC/Ability/Genesis")]
public class Ability_GenesisSO : DamageAbilityDataSO
{
    public EEffectType effectType;
    public Vector2 detectRange = new (20f, 20f); 

    public override AbilityLogicBase CreateInstance()
    {
        return new GenesisAbility();
    }
}
