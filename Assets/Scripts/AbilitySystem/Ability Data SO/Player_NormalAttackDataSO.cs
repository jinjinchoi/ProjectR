using UnityEngine;

[CreateAssetMenu(menuName = "ASC/Ability/Player/NormalAttack")]
public class Player_NormalAttackDataSO : DamageAbilityDataSO
{

    public override AbilityLogicBase CreateInstance()
    {
        return new NormalAttackAbility();
    }
}
