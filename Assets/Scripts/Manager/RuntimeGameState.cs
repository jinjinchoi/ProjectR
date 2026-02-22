
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PrimaryAttributeData
{
    public float strength;
    public float dexterity;
    public float intelligence;
    public float vitality;
    public float currentHeath;

    public float GetValueByType(EAttributeType attribute)
    {
        return attribute switch
        {
            EAttributeType.Strength => strength,
            EAttributeType.Dexterity => dexterity,
            EAttributeType.Intelligence => intelligence,
            EAttributeType.Vitality => vitality,
            EAttributeType.CurrentHealth => currentHeath,
            _ => 0
        };
    }
}

public class RuntimeGameState
{
    private PrimaryAttributeData playerData;
    private List<EAbilityId> unlokcedAbilityIds = new();

    private AttributeGrowthData currentGrowthData;
    private readonly AttributeGrowthCalculator growthCalculator;

    public PrimaryAttributeData PlayerData => playerData;
    public List<EAbilityId> UnlokcedAbilityIds => unlokcedAbilityIds;
    public AttributeGrowthData CurrentGrowthData => currentGrowthData;
    public AttributeGrowthCalculator GrowthCalculator => growthCalculator;

    public RuntimeGameState()
    {
        growthCalculator = new AttributeGrowthCalculator();
    }

    public void UpdatePlayerData(PrimaryAttributeData data, List<EAbilityId> abilityList)
    {
        playerData = data;
        unlokcedAbilityIds = abilityList;
    }

    public void GenerateGrowthData(float health, float maxHealth)
    {
        currentGrowthData = growthCalculator.Generate(health, maxHealth);
    }

    public void LoadGrowthData(AttributeGrowthData growthData)
    {
        currentGrowthData = growthData;
    }

    public void Reset()
    {
        playerData = null;
        currentGrowthData = null;
        unlokcedAbilityIds.Clear();
    }
}