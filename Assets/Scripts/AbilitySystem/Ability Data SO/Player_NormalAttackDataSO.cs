using UnityEngine;

[CreateAssetMenu(menuName = "ASC/Ability/Player/NormalAttack")]
public class Player_NormalAttackDataSO : DamageAbilityDataSO
{
    [Header("ComboAttackInfo")]
    public int MaxComboCount = 1;
    public string ComboCountName = "comboCount";
    public float AttackRadius = 1.5f;

    public override AbilityLogicBase CreateInstance()
    {
        return new NormalAttackAbility();
    }
}
