using System;

public struct FEnemySecondaryAttribute
{
    public int level;
    public float physicalAttackPower;
    public float physicalDefensePower;
    public float magicAttackPower;
    public float magicDefensePower;
    public float criticalChance;
    public float maxHealth;

    public float GetValueByType(EAttributeType attributeType)
    {
        return attributeType switch
        {
            EAttributeType.PhysicalAttackPower => physicalAttackPower,
            EAttributeType.MagicAttackPower => magicAttackPower,
            EAttributeType.PhysicalDefensePower => physicalDefensePower,
            EAttributeType.MagicDefensePower => magicDefensePower,
            EAttributeType.CriticalChance => criticalChance,
            EAttributeType.MaxHealth => maxHealth,
            _ => 0,
        };
    }
}


public class EnemyCharacter : BaseCharacter
{
    public event Action<FDamageInfo> OnHit;

    private readonly EAttributeType[] SecondaryAttributes =
    {
        EAttributeType.PhysicalAttackPower,
        EAttributeType.PhysicalDefensePower,
        EAttributeType.MagicAttackPower,
        EAttributeType.MagicDefensePower,
        EAttributeType.CriticalChance,
        EAttributeType.MaxHealth,
    };


    protected override void Awake()
    {
        base.Awake();
    }


    public void Init(FEnemySecondaryAttribute attributeInfo)
    {
        foreach (var attribute in SecondaryAttributes)
        {
            ASC.ApplyModifier(MakeSecondaryAttributeModifier(attribute, attributeInfo));
        }
        
        FAttributeModifier healthModifier = new()
        {
            attributeType = EAttributeType.CurrentHealth,
            policy = EModifierPolicy.Instant,
            operation = EModifierOp.Add,
            value = ASC.AttributeSet.GetAttributeValue(EAttributeType.MaxHealth)
        };
        ASC.ApplyModifier(healthModifier);
    }

    private FAttributeModifier MakeSecondaryAttributeModifier(EAttributeType attribute, FEnemySecondaryAttribute attributeInfo)
    {
        return new FAttributeModifier()
        {
            attributeType = attribute,
            policy = EModifierPolicy.Infinite,
            operation = EModifierOp.Override,
            value = attributeInfo.GetValueByType(attribute)
        };
    }

    public override void TakeDamage(FDamageInfo damageInfo)
    {
        base.TakeDamage(damageInfo);

        OnHit?.Invoke(damageInfo);

    }


}
