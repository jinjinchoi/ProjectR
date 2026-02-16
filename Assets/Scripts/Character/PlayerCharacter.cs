using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerCharacter : BaseCharacter
{
    private readonly EAttributeType[] PrimaryAttributes =
    {
        EAttributeType.strength,
        EAttributeType.intelligence,
        EAttributeType.dexterity,
        EAttributeType.vitality
    };

    [Header("Player Abilities")]
    [SerializeField] private List<BaseAbilityDataSO> unlockableAbilities; // 습득이 필요한 ability

    private HashSet<EAbilityId> unlockedAbilityIdSet = new();
    public List<BaseAbilityDataSO> UnLockableAbilities => unlockableAbilities;


    protected override void Awake()
    {
        base.Awake();
    }


    protected override void Start()
    {
        base.Start();

        GameManager.Instance.SceneChangingAsync += SaveBeforeSceneChange;
        ApplySavedPrimaryAttribute();
        GiveUnlockedAbility();
    }

    private void OnDisable()
    {
        GameManager.Instance.SceneChangingAsync -= SaveBeforeSceneChange;
    }


    private Task SaveBeforeSceneChange()
    {
        GameManager.Instance.SaveManager.BackupPrimaryData(MakePrimaryAttributeData());
        GameManager.Instance.SaveManager.unlokcedAbilityIds = unlockedAbilityIdSet.ToList();

        return Task.CompletedTask;
    }

    private PrimaryAttributeData MakePrimaryAttributeData()
    {
        return new()
        {
            strength = ASC.AttributeSet.GetAttributeValue(EAttributeType.strength),
            dexterity = ASC.AttributeSet.GetAttributeValue(EAttributeType.dexterity),
            intelligence = ASC.AttributeSet.GetAttributeValue(EAttributeType.intelligence),
            vitality = ASC.AttributeSet.GetAttributeValue(EAttributeType.vitality),
            currentHeath = ASC.AttributeSet.GetAttributeValue(EAttributeType.currentHealth),
        };
    }

    void ApplySavedPrimaryAttribute()
    {
        if (GameManager.Instance.SaveManager.PlayerData == null) return;

        foreach (var attribute in PrimaryAttributes)
        {
            ASC.ApplyModifier(MakePrimaryModifier(attribute, GameManager.Instance.SaveManager.PlayerData));
        }

        FAttributeModifier healthModifier = new()
        {
            attributeType = EAttributeType.currentHealth,
            policy = EModifierPolicy.Instant,
            operation = EModifierOp.Override,
            value = GameManager.Instance.SaveManager.PlayerData.currentHeath
        };
        ASC.ApplyModifier(healthModifier);
    }

    private FAttributeModifier MakePrimaryModifier(EAttributeType attribute, PrimaryAttributeData data)
    {
        return new()
        {
            attributeType = attribute,
            value = data.GetValueByType(attribute),
            policy = EModifierPolicy.Instant,
            operation = EModifierOp.Override
        };
    }
    private void GiveUnlockedAbility()
    {
        var unlickedIdList = GameManager.Instance.SaveManager.unlokcedAbilityIds;
        unlockedAbilityIdSet = new HashSet<EAbilityId>(unlickedIdList);

        foreach (BaseAbilityDataSO abilityData in unlockableAbilities)
        {
            if (IsUnLockedAbility(abilityData.abilityId))
                ASC.GiveAbility(abilityData);
        }
    }

    public bool IsUnLockedAbility(EAbilityId id)
    {
        return unlockedAbilityIdSet.Contains(id);
    }

    public void UnlockAbility(EAbilityId id)
    {
        unlockedAbilityIdSet.Add(id);
    }


    protected override void OnDead()
    {
        base.OnDead();

        EventHub.RaisePlayerDied();
    }
}
