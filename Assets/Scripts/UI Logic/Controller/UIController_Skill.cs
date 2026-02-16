
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
    BaseCharacter owner;

    public UIController_Skill(BaseCharacter owner)
    {
        this.owner = owner;
    }

    public FAbilityUIInfo[] GetAllAbilityUiInfo()
    {
        if (owner == null || owner.AbilityList?.Count == 0)
            return null;

        int count = owner.AbilityList.Count;
        var infoArray = new FAbilityUIInfo[count];

        for (int i = 0; i < count; i++)
        {
            var data = owner.AbilityList[i];

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
}
