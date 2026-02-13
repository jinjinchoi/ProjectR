using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AttributeInfo
{
    public EAttributeType attribute;
    [TextArea(3, 10)] public string description;
}

[CreateAssetMenu(fileName = "Attribute Information", menuName = "ASC/Attribute/Description")]
public class AttributeInformationSO : ScriptableObject
{
    public List<AttributeInfo> attributeInfos;

    private Dictionary<EAttributeType, AttributeInfo> infoMap;
    public void Init()
    {
        infoMap = attributeInfos.ToDictionary(e => e.attribute);
    }

    public string GetAttributeDescription(EAttributeType attribute)
    {
        infoMap.TryGetValue(attribute, out var data);
        if (data == null)
        {
            Debug.LogWarning($"Description data is null for {attribute}");
            return string.Empty;
        }

        return data.description;
    }

}
