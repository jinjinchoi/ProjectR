using UnityEngine;

[CreateAssetMenu(menuName = "ASC/Ability/Player/NormalAttack")]
public class Player_NormalAttackDataSO : DamageAbilityDataSO
{
    [Header("ComboAttackInfo")]
    public int maxComboCount = 1;
    public string comboCountName = "comboCount";
    public Vector2 knockbackPower = Vector2.zero;

    public override AbilityLogicBase CreateInstance()
    {
        return new NormalAttackAbility();
    }
}
