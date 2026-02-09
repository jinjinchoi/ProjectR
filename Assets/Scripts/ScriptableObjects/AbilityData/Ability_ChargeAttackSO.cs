using UnityEngine;

[CreateAssetMenu(menuName = "ASC/Ability/ChargeAttack")]
public class Ability_ChargeAttackSO : DamageAbilityDataSO
{
    public float ChargeSpeed = 10f;

    public override AbilityLogicBase CreateInstance()
    {
        return new ChargeAttackAbility();
    }
}
