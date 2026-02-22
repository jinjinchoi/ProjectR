using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public PrimaryAttributeData PrimaryAttributeData;
    public AttributeGrowthData GrowthData;
    public List<EAbilityId> UnlokcedAbilityIds;
    public int Day;
}
