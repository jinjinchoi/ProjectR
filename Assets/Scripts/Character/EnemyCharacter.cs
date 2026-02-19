using System;
using UnityEngine;




public class EnemyCharacter : BaseCharacter
{
    public event Action<FDamageInfo> OnHit;

    [SerializeField] private EnemyAttribtueSO secondaryAttributeSO;
    [SerializeField] private EEnemyId enemyId;

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

        if (enemyId == EEnemyId.None)
        {
            DebugHelper.LogWarning($"Enemy ID is not set on [{gameObject.name}]");
        }

        if (secondaryAttributeSO == null)
        {
            DebugHelper.LogWarning($"Secondary Attribute SO is not set on [{gameObject.name}]");
        }
    }

    public void Init(int level)
    {
        EnemyInformation enemyInfo = secondaryAttributeSO.GetEnemyInfo(enemyId);

        foreach (var attribute in SecondaryAttributes)
        {
            float attributeValue = enemyInfo.GetAttributeValue(attribute, level);
            ASC.ApplyModifier(MakeSecondaryAttributeModifier(attribute, attributeValue));
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


    private FAttributeModifier MakeSecondaryAttributeModifier(EAttributeType attribute, float attributeValue)
    {
        return new FAttributeModifier()
        {
            attributeType = attribute,
            policy = EModifierPolicy.Infinite,
            operation = EModifierOp.Override,
            value = attributeValue
        };
    }

    public override void TakeDamage(FDamageInfo damageInfo)
    {
        base.TakeDamage(damageInfo);

        OnHit?.Invoke(damageInfo);

    }
}
