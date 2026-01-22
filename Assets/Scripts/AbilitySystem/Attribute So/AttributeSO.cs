using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AttributeInitInfo
{
    public EAttributeType AttributeType;
    public float BaseValue;
}

[CreateAssetMenu(fileName = "Attribute_" ,menuName = "ASC/Attribute")]
public class AttributeSO : ScriptableObject
{
    public List<AttributeInitInfo> Attributes;
    
}

