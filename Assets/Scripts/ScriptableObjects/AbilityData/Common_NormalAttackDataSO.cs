using UnityEngine;

[CreateAssetMenu(menuName = "ASC/Ability/NormalAttack")]
public class Common_NormalAttackDataSO : DamageAbilityDataSO
{
    [Header("ComboAttackInfo")]
    public int maxComboCount = 1;
    public string comboCountName = "comboCount";

    public override AbilityLogicBase CreateInstance()
    {
        return new NormalAttackAbility();
    }
}
