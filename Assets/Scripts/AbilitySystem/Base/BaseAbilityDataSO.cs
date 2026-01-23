using UnityEngine;

/* 어빌리티의 정보를 저장하는 베이스 SO. 하위 클래스의 CreateInstance 함수에서 알맞은 Abiilty를 생성하고 반환.  */
public abstract class BaseAbilityDataSO : ScriptableObject
{
    [Header("Ability Info")]
    public EAbilityId abilityId;
    public string abilityName;
    public float cooldown;
    [Space]

    [Header("Anim Info")]
    public string animBoolName;
    public string animTag;

    public abstract AbilityLogicBase CreateInstance();
}
