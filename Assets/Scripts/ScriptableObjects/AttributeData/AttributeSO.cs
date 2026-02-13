using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AttributeInitInfo
{
    public EAttributeType attributeType;
    public float baseValue;
}

[CreateAssetMenu(fileName = "Attribute_" ,menuName = "ASC/Attribute/Config")]
public class AttributeSO : ScriptableObject
{
    public List<AttributeInitInfo> Attributes;
    
}

