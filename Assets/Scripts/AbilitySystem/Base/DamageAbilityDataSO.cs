using UnityEngine;

public abstract class DamageAbilityDataSO : BaseAbilityDataSO
{
    [Header("Damage Info")]
    public float damageMultiplier = 100f;
    public float attackDistance = 1.5f; // 공격에 들어갈 수 있는 사거리
    public float attackRadius = 1.5f; // 공격 지점에서 피해를 입힐 수 있는 범위
    public LayerMask hostileTargetLayer;
    public EDamageType damageType;
}
