using System;
using UnityEngine;

public class UIControllerBase : MonoBehaviour
{
    public event Action<EAttributeType, float> OnAttributeValueChanged;
    // vital attribute 변화시 비율을 전송하는 event
    public event Action<bool /* true == health, false == mana */, float> OnVitalRatioChanged;

    private BaseCharacter character;

    private void Awake()
    {
        character = GetComponent<BaseCharacter>();
    }

    private void Start()
    {
        character.ASC.AttributeSet.OnAttributeChanged += OnAttritbuteChanged;
    }

    private void OnAttritbuteChanged(EAttributeType attribute, float currentValue)
    {
        OnAttributeValueChanged?.Invoke(attribute, currentValue);

        bool isHealthValueChanged =
            attribute == EAttributeType.currentHealth ||
            attribute == EAttributeType.currentMana;
        if (isHealthValueChanged)
        {
            float health = character.ASC.AttributeSet.GetAttributeValue(EAttributeType.currentHealth);
            float maxHealth = character.ASC.AttributeSet.GetAttributeValue(EAttributeType.maxHealth);

            OnVitalRatioChanged?.Invoke(true, health / maxHealth);
        }

        bool isManaValueChanged =
            attribute == EAttributeType.currentMana ||
            attribute == EAttributeType.maxMana;
        if (isManaValueChanged)
        {
            float mana = character.ASC.AttributeSet.GetAttributeValue(EAttributeType.currentMana);
            float maxMana = character.ASC.AttributeSet.GetAttributeValue(EAttributeType.maxMana);

            OnVitalRatioChanged?.Invoke(false, mana / maxMana);
        }
    }

    private void OnDisable()
    {
        character.ASC.AttributeSet.OnAttributeChanged -= OnAttritbuteChanged;

    }
}
