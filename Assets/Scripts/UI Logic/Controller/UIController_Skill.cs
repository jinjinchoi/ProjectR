
using System.Collections.Generic;
using UnityEngine;

public struct FAbilityUIInfo
{
    public EAbilityId id;
    public string name;
    public string description;
    public Sprite icon;
}

public class UIController_Skill
{
    PlayerCharacter owner;

    public UIController_Skill(PlayerCharacter owner)
    {
        this.owner = owner;
    }

    public FAbilityUIInfo[] GetAllAbilityUiInfo()
    {
        if (owner == null || owner.UnLockableAbilities?.Count == 0)
            return null;

        int count = owner.UnLockableAbilities.Count;
        var infoArray = new FAbilityUIInfo[count];

        for (int i = 0; i < count; i++)
        {
            var data = owner.UnLockableAbilities[i];

            infoArray[i] = new()
            {
                id = data.abilityId,
                name = data.name,
                description = data.description,
                icon = data.icon,
            };
        }

        return infoArray;
    }

    public void UnlockAbility(EAbilityId id)
    {
        owner.UnlockAbility(id);
    }

    public bool IsUnLockedAbility(EAbilityId id)
    {
        return owner.IsUnLockedAbility(id);
    }
}
