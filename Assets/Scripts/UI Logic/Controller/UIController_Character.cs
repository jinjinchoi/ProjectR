using UnityEngine;

public class UIController_Character : BaseCharacterUIController
{

    public float GetHealthRatio()
    {
        if (abilitySystem == null)
        {
            Debug.LogWarning("ASC not set");
            return 1f;
        }

        float health = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.currentHealth);
        float maxHealth = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.maxHealth);

        return health / maxHealth;
    }

}
