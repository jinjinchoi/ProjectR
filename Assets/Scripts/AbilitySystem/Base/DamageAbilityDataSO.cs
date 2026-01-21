using UnityEngine;

public abstract class DamageAbilityDataSO : BaseAbilityDataSO
{
    [Header("Damage Info")]
    public float attackRadius = 1f;
    public float damageMultiplier = 100f;
}
