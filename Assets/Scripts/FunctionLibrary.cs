using UnityEngine;

public static class FunctionLibrary 
{
    public static string GetAttributeNameByType(EAttributeType type)
    {
        switch (type)
        {
            case EAttributeType.strength:
                return "힘";

            case EAttributeType.dexterity:
                return "민첩";

            case EAttributeType.intelligence:
                return "지능";

            case EAttributeType.vitality:
                return "활력";

            case EAttributeType.physicalAttackPower:
                return "물리 공격력";

            case EAttributeType.magicAttackPower:
                return "마법 공격력";

            case EAttributeType.physicalDefensePower:
                return "물리 방어력";
                
            case EAttributeType.magicDefensePower:
                return "마법 방어력";
                
            case EAttributeType.criticalChance:
                return "치명타 확률";

            case EAttributeType.maxHealth:
                return "최대 체력";
                
            case EAttributeType.maxMana:
                return "최대 마력";

            case EAttributeType.currentHealth:
                return "체력";
                
            case EAttributeType.currentMana:
                return "마력";
                
            case EAttributeType.incommingDamage:
                return string.Empty;

            default:
                Debug.LogError("Invalid type detected");
                return string.Empty;
        }
    }
}
