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

        if (attribute is EAttributeType.CurrentHealth or EAttributeType.MaxHealth)
        {
            float health = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.CurrentHealth);
            float maxHealth = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.MaxHealth);

            OnVitalRatioChanged?.Invoke(true, health / maxHealth);
        }

        if (attribute is EAttributeType.CurrentMana or EAttributeType.MaxMana)
        {
            float mana = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.CurrentMana);
            float maxMana = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.MaxMana);

            OnVitalRatioChanged?.Invoke(false, mana / maxMana);
        }
    }
}
