using UnityEngine;

[CreateAssetMenu(menuName = "ASC/Ability/Proejctile")]
public class Ability_ProjectileSO : DamageAbilityDataSO
{
    public EObjectId projectileId;

    public override AbilityLogicBase CreateInstance()
    {
        return new ProjectileAbility();
    }
}
