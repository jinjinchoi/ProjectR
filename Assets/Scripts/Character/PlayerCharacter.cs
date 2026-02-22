using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerCharacter : BaseCharacter
{
    private readonly EAttributeType[] PrimaryAttributes =
    {
        EAttributeType.Strength,
        EAttributeType.Intelligence,
        EAttributeType.Dexterity,
        EAttributeType.Vitality
    };

    [Header("Player Abilities")]
    [SerializeField] private List<BaseAbilityDataSO> unlockableAbilities; // 습득이 필요한 ability
    private Dictionary<EAbilityId, BaseAbilityDataSO> unlockableAbilitiesMap;

    private HashSet<EAbilityId> unlockedAbilityIdSet = new();
    public List<BaseAbilityDataSO> UnLockableAbilities => unlockableAbilities;
    public Dictionary<EAbilityId, BaseAbilityDataSO> UnLockableAbilityMap => unlockableAbilitiesMap;

    protected override void Awake()
    {
        base.Awake();
        unlockableAbilitiesMap = unlockableAbilities.ToDictionary(e => e.abilityId);

        if (GameManager.Instance.RuntimeGameState.CurrentGrowthData == null)
        {
            float health = ASC.AttributeSet.GetAttributeValue(EAttributeType.MaxHealth);
            float maxHealth = ASC.AttributeSet.GetAttributeValue(EAttributeType.MaxHealth);

            GameManager.Instance.RuntimeGameState.GenerateGrowthData(health, maxHealth);
        }
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
        GameManager.Instance.RuntimeGameState.UpdatePlayerData(MakePrimaryAttributeData(), unlockedAbilityIdSet.ToList());

        return Task.CompletedTask;
    }

    public void SaveDataToRuntimeGameState()
    {
        GameManager.Instance.RuntimeGameState.UpdatePlayerData(MakePrimaryAttributeData(), unlockedAbilityIdSet.ToList());
    }

    private PrimaryAttributeData MakePrimaryAttributeData()
    {
        return new()
        {
            strength = ASC.AttributeSet.GetAttributeValue(EAttributeType.Strength),
            dexterity = ASC.AttributeSet.GetAttributeValue(EAttributeType.Dexterity),
            intelligence = ASC.AttributeSet.GetAttributeValue(EAttributeType.Intelligence),
            vitality = ASC.AttributeSet.GetAttributeValue(EAttributeType.Vitality),
            currentHeath = ASC.AttributeSet.GetAttributeValue(EAttributeType.CurrentHealth),
        };
    }

    private void ApplySavedPrimaryAttribute()
    {
        if (GameManager.Instance.RuntimeGameState.PlayerData == null) return;

        foreach (var attribute in PrimaryAttributes)
        {
            ASC.ApplyModifier(MakePrimaryModifier(attribute, GameManager.Instance.RuntimeGameState.PlayerData));
        }

        FAttributeModifier healthModifier = new()
        {
            attributeType = EAttributeType.CurrentHealth,
            policy = EModifierPolicy.Instant,
            operation = EModifierOp.Override,
            value = GameManager.Instance.RuntimeGameState.PlayerData.currentHeath
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
        var unlickedIdList = GameManager.Instance.RuntimeGameState.UnlokcedAbilityIds;
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

    public void TryUnlockAbility(EAbilityId id)
    {
        if (!unlockableAbilitiesMap.TryGetValue(id, out var abilityData))
            return;

        float currentSp = ASC.AttributeSet.GetAttributeValue(EAttributeType.SkillPoint);

        if (currentSp < abilityData.sp)
            return;

        FAttributeModifier spModifier = new()
        {
            attributeType = EAttributeType.SkillPoint,
            policy = EModifierPolicy.Instant,
            operation = EModifierOp.Add,
            value = -abilityData.sp
        };
        ASC.ApplyModifier(spModifier);

        DebugHelper.Log($"[{id}] is unlocked");

        unlockedAbilityIdSet.Add(id);
    }


    protected override void OnDead()
    {
        base.OnDead();

        EventHub.RaisePlayerDied();
    }
}
