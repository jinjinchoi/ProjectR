
using System.Collections.Generic;
using UnityEngine;

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
            EAttributeType.strength => strength,
            EAttributeType.dexterity => dexterity,
            EAttributeType.intelligence => intelligence,
            EAttributeType.vitality => vitality,
            EAttributeType.currentHealth => currentHeath,
            _ => 0
        };
    }
}

public class SaveManager
{
    private PrimaryAttributeData playerData;

    public PrimaryAttributeData PlayerData => playerData;
    public List<EAbilityId> unlokcedAbilityIds = new();

    public void BackupPrimaryData(PrimaryAttributeData data)
    {
        playerData = data;
    }

    public void Reset()
    {
        playerData = null;
    }
}
