using System;
using System.Threading.Tasks;

public class PlayerCharacter : BaseCharacter
{
    private readonly EAttributeType[] PrimaryAttributes =
    {
        EAttributeType.strength,
        EAttributeType.intelligence,
        EAttributeType.dexterity,
        EAttributeType.vitality
    };

    protected override void Start()
    {
        base.Start();

        GameManager.Instance.SceneChangingAsync += SaveBeforeSceneChange;
        ApplySavedPrimaryAttribute();
    }

    private void OnDisable()
    {
        GameManager.Instance.SceneChangingAsync -= SaveBeforeSceneChange;
    }

    private Task SaveBeforeSceneChange()
    {
        GameManager.Instance.SaveManager.BackupPrimaryData(MakePrimaryAttributeData());
        return Task.CompletedTask;
    }

    private PrimaryAttributeData MakePrimaryAttributeData()
    {
        return new()
        {
            strength = ASC.AttributeSet.GetAttributeValue(EAttributeType.strength),
            dexterity = ASC.AttributeSet.GetAttributeValue(EAttributeType.dexterity),
            intelligence = ASC.AttributeSet.GetAttributeValue(EAttributeType.intelligence),
            vitality = ASC.AttributeSet.GetAttributeValue(EAttributeType.vitality),
            currentHeath = ASC.AttributeSet.GetAttributeValue(EAttributeType.currentHealth),
        };
    }

    void ApplySavedPrimaryAttribute()
    {
        if (GameManager.Instance.SaveManager.PlayerData == null) return;

        DebugHelper.Log("Restore Primary Attribute");

        foreach (var attribute in PrimaryAttributes)
        {
            ASC.ApplyModifier(MakePrimaryModifier(attribute, GameManager.Instance.SaveManager.PlayerData));
        }

        FAttributeModifier healthModifier = new()
        {
            attributeType = EAttributeType.currentHealth,
            isPermanent = true,
            operation = EModifierOp.Override,
            value = GameManager.Instance.SaveManager.PlayerData.currentHeath
        };
        ASC.ApplyModifier(healthModifier);
    }

    private FAttributeModifier MakePrimaryModifier(EAttributeType attribute, PrimaryAttributeData data)
    {
        return new()
        {
            attributeType = attribute,
            value = data.GetValueByType(attribute),
            isPermanent = true,
            operation = EModifierOp.Override
        };
    }

    protected override void OnDead()
    {
        base.OnDead();

        EventHub.RaisePlayerDied();
    }
}
