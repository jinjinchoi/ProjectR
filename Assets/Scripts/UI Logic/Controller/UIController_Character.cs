using UnityEngine;

public class UIController_Character : BaseCharacterUIController
{

    protected override void Awake()
    {
        base.Awake();

        BaseCharacter character = GetComponent<BaseCharacter>();
        abilitySystem = character.ASC;
    }

}
