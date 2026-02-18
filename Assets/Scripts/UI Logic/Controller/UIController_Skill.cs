using UnityEngine;

public struct FAbilityUIInfo
{
    public EAbilityId id;
    public string name;
    public string description;
    public Sprite icon;
    public int sp;
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
        int count = owner.UnLockableAbilities.Count;
        var infoArray = new FAbilityUIInfo[count];

        for (int i = 0; i < count; i++)
        {
            var data = owner.UnLockableAbilities[i];

            infoArray[i] = new()
            {
                id = data.abilityId,
                name = data.abilityName,
                description = data.description,
                icon = data.icon,
                sp = data.sp,
            };
        }

        return infoArray;
    }

    public float GetCurrentSp()
    {
        return owner.ASC.AttributeSet.GetAttributeValue(EAttributeType.SkillPoint);
    }

    public float GetAbilityRequiredSp(EAbilityId id)
    {
        if (owner.UnLockableAbilityMap.TryGetValue(id, out var data))
        {
            return data.sp;
        }

        return 0f;
    }

    public void UnlockAbility(EAbilityId id)
    {
        if (GetAbilityRequiredSp(id) <= GetCurrentSp())
            owner.TryUnlockAbility(id);
    }

    public bool IsUnLockedAbility(EAbilityId id)
    {
        return owner.IsUnLockedAbility(id);
    }
}
