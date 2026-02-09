using UnityEngine;

[CreateAssetMenu(menuName = "ASC/Ability/ThunderStrike")]
public class Ability_ThunderStrikeSO : DamageAbilityDataSO
{
    public override AbilityLogicBase CreateInstance()
    {
        return new ThunderStrikeAbility();
    }
}
