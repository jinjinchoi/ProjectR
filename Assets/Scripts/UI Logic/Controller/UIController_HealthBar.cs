using UnityEngine;

public class UIController_HealthBar : BaseCharacterUIController
{
    public float GetHealthRatio()
    {
        if (abilitySystem == null)
        {
            Debug.LogWarning("ASC not set");
            return 0f;
        }

        float health = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.CurrentHealth);
        float maxHealth = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.MaxHealth);

        return health / maxHealth;
    }

    public float GetAttributeValue(EAttributeType attributeType)
    {
        if (abilitySystem == null) return 0f;

        return abilitySystem.AttributeSet.GetAttributeValue(attributeType);
    }

}
