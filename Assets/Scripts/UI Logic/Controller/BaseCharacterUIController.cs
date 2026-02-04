using System;
using UnityEngine;

public class BaseCharacterUIController : MonoBehaviour
{
    public event Action<EAttributeType, float> OnAttributeValueChanged;
    // vital attribute 변화시 비율을 전송하는 event
    public event Action<bool /* true == health, false == mana */, float> OnVitalRatioChanged;

    protected IAbilitySystemContext abilitySystem;

    protected virtual void Awake()
    {
        abilitySystem = GetComponent<IAbilitySystemContext>();
    }

    protected virtual void Start()
    {
        if (abilitySystem == null)
        {
            Debug.LogWarning($"Ability Component not set to {gameObject.name}");
            return;
        }

        abilitySystem.AttributeSet.OnAttributeChanged += OnAttritbuteChanged;
    }

    private void OnAttritbuteChanged(EAttributeType attribute, float currentValue)
    {
        if (abilitySystem == null) return;

        OnAttributeValueChanged?.Invoke(attribute, currentValue);

        bool isHealthValueChanged =
            attribute == EAttributeType.currentHealth ||
            attribute == EAttributeType.currentMana;
        if (isHealthValueChanged)
        {
            float health = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.currentHealth);
            float maxHealth = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.maxHealth);

            OnVitalRatioChanged?.Invoke(true, health / maxHealth);
        }

        bool isManaValueChanged =
            attribute == EAttributeType.currentMana ||
            attribute == EAttributeType.maxMana;
        if (isManaValueChanged)
        {
            float mana = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.currentMana);
            float maxMana = abilitySystem.AttributeSet.GetAttributeValue(EAttributeType.maxMana);

            OnVitalRatioChanged?.Invoke(false, mana / maxMana);
        }
    }

    protected virtual void OnDisable()
    {
        abilitySystem.AttributeSet.OnAttributeChanged -= OnAttritbuteChanged;

    }
}
