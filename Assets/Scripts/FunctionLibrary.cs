using UnityEngine;

public static class FunctionLibrary 
{
    public static string GetAttributeNameByType(EAttributeType type)
    {
        switch (type)
        {
            case EAttributeType.Strength:
                return "힘";

            case EAttributeType.Dexterity:
                return "민첩";

            case EAttributeType.Intelligence:
                return "지능";

            case EAttributeType.Vitality:
                return "활력";

            case EAttributeType.PhysicalAttackPower:
                return "물리 공격력";

            case EAttributeType.MagicAttackPower:
                return "마법 공격력";

            case EAttributeType.PhysicalDefensePower:
                return "물리 방어력";
                
            case EAttributeType.MagicDefensePower:
                return "마법 방어력";
                
            case EAttributeType.CriticalChance:
                return "치명타 확률";

            case EAttributeType.MaxHealth:
                return "최대 체력";
                
            case EAttributeType.MaxMana:
                return "최대 마력";

            case EAttributeType.CurrentHealth:
                return "체력";
                
            case EAttributeType.CurrentMana:
                return "마력";
                
            case EAttributeType.IncommingDamage:
                return string.Empty;

            default:
                Debug.LogError("Invalid type detected");
                return string.Empty;
        }
    }
}
