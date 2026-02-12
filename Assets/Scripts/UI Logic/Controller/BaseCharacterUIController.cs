using System;
using UnityEngine;

public abstract class BaseCharacterUIController
{
    public event Action<EAttributeType, float> OnAttributeValueChanged;
    // vital attribute 변화시 비율을 전송하는 event
    public event Action<bool /* true == health, false == mana */, float> OnVitalRatioChanged;

    protected IAbilitySystemContext abilitySystem;

    public virtual void Init(IAbilitySystemContext asc)
    {
        abilitySystem = asc;
        abilitySystem.AttributeSet.OnAttributeChanged += OnAttritbuteChanged;
    }

    public virtual void Dispose()
    {
        abilitySystem.AttributeSet.OnAttributeChanged -= OnAttritbuteChanged;
    }

    private void OnAttritbuteChanged(EAttributeType attribute, float currentValue)
    {
        if (abilitySystem == null) return;

        OnAttributeValueChanged?.Invoke(attribute, currentValue);

        if (attribute is EAttributeType.maxHealth)
            Debug.Log($"{attribute} : {currentValue}");

        if (attribute is EAttributeType.currentHealth or EAttributeType.maxHealth)
        {
            float health = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.currentHealth);
            float maxHealth = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.maxHealth);

            OnVitalRatioChanged?.Invoke(true, health / maxHealth);
        }

        if (attribute is EAttributeType.currentMana or EAttributeType.maxMana)
        {
            float mana = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.currentMana);
            float maxMana = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.maxMana);

            OnVitalRatioChanged?.Invoke(false, mana / maxMana);
        }
    }
}
