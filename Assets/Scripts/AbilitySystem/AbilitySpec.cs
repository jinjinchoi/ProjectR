using UnityEngine;

// AbilitySpec을 저장하는 클래스. Level이나 쿨다운을 설정
// ASC에 어빌리티가 spec으로 관리되기 때문에 SO의 속성을 바꾸는 것으로 동일한 Ability(id)라고 할지라도
// 다른 애니메이션과 대미지 구현이 가능
public class AbilitySpec
{
    public BaseAbilityDataSO abilityData;
    public AbilityLogicBase ability;
    public int level;

    public AbilitySpec(BaseAbilityDataSO data, AbilitySystemComponent asc)
    {
        this.abilityData = data;
        ability = data.CreateInstance();
        ability.Init(asc);
    }
}
