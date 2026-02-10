using UnityEngine;

[CreateAssetMenu(menuName = "ASC/Ability/SwordAura")]
public class Ability_SwordAuraSO : DamageAbilityDataSO
{
    public EObjectId projectileId;

    public override AbilityLogicBase CreateInstance()
    {
        return new SwordAuraAbility();
    }
}
