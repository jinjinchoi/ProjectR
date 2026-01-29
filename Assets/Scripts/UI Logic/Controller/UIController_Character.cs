using UnityEngine;

public class UIController_Character : BaseUIController
{

    protected override void Awake()
    {
        base.Awake();

        BaseCharacter character = GetComponent<BaseCharacter>();
        abilitySystem = character.ASC;
    }

}
