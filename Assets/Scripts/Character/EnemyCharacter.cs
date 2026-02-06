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
            EAttributeType.physicalAttackPower => physicalAttackPower,
            EAttributeType.magicAttackPower => magicAttackPower,
            EAttributeType.physicalDefensePower => physicalDefensePower,
            EAttributeType.magicDefensePower => magicDefensePower,
            EAttributeType.criticalChance => criticalChance,
            EAttributeType.maxHealth => maxHealth,
            _ => 0,
        };
    }
}


public class EnemyCharacter : BaseCharacter
{
    public event Action<FDamageInfo> OnHit;

    private readonly EAttributeType[] SecondaryAttributes =
    {
        EAttributeType.physicalAttackPower,
        EAttributeType.physicalDefensePower,
        EAttributeType.magicAttackPower,
        EAttributeType.magicDefensePower,
        EAttributeType.criticalChance,
        EAttributeType.maxHealth,
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
            attributeType = EAttributeType.currentHealth,
            isPermanent = true,
            operation = EModifierOp.Add,
            value = ASC.AttributeSet.GetAttributeValue(EAttributeType.maxHealth)
        };
        ASC.ApplyModifier(healthModifier);
    }

    private FAttributeModifier MakeSecondaryAttributeModifier(EAttributeType attribute, FEnemySecondaryAttribute attributeInfo)
    {
        return new FAttributeModifier()
        {
            attributeType = attribute,
            isPermanent = false, // secondary attribute는 base value와 상관없이 계산식에 의해 값을 가져오기 때문에 base value를 바꿔도 의미 없음.
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
