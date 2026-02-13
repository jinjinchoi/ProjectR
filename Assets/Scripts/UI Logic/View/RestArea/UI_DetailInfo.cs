using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_DetailInfo : MonoBehaviour
{
    VisualElement panel;
    Button closeButton;
    Label descriptionText;
    Dictionary<EAttributeType, Label> statLabels;
    Dictionary<EAttributeType, VisualElement> statAreaMap;

    UIController_RestArea uiController;

    [SerializeField] AttributeInformationSO attributeInfoSO;


    private void Awake()
    {
        uiController = new UIController_RestArea();
        uiController.Init(GetComponentInParent<IAbilitySystemContext>());

        var root = GetComponent<UIDocument>().rootVisualElement;
        panel = root.Q<VisualElement>("Panel");
        panel.AddToClassList("panel-close");

        descriptionText = root.Q<Label>("DetailInfoText");
        descriptionText.text = string.Empty;

        closeButton = root.Q<Button>("CloseButton");
        closeButton.clicked += OnCloseButtonClicked;

        statLabels = new Dictionary<EAttributeType, Label>
        {
            { EAttributeType.physicalAttackPower,        root.Q<Label>("Text_PAttackPower") },
            { EAttributeType.magicAttackPower,    root.Q<Label>("Text_MAttakcPower") },
            { EAttributeType.physicalDefensePower, root.Q<Label>("Text_PhysicalDefence") },
            { EAttributeType.magicDefensePower,    root.Q<Label>("Text_MagicDefence") },
            { EAttributeType.criticalChance,  root.Q<Label>("Text_CriticalChance") },
            { EAttributeType.maxHealth,       root.Q<Label>("Text_MaxHealth") },
            { EAttributeType.maxMana,         root.Q<Label>("Text_MaxMana") }
        };

        statAreaMap = new Dictionary<EAttributeType, VisualElement>
        {
            { EAttributeType.physicalAttackPower,       root.Q<VisualElement>("DetailTextArea_PA") },
            { EAttributeType.magicAttackPower,    root.Q<VisualElement>("DetailTextArea_MA") },
            { EAttributeType.physicalDefensePower, root.Q<VisualElement>("DetailTextArea_PD") },
            { EAttributeType.magicDefensePower,    root.Q<VisualElement>("DetailTextArea_MD") },
            { EAttributeType.criticalChance,  root.Q<VisualElement>("DetailTextArea_CC") },
            { EAttributeType.maxHealth,       root.Q<VisualElement>("DetailTextArea_MH") },
            { EAttributeType.maxMana,       root.Q<VisualElement>("DetailTextArea_MM") },
        };

        if (attributeInfoSO) attributeInfoSO.Init();
        RegisterHover();

    }

    private void OnEnable()
    {
        EventHub.DetailButtonClicked += OnDetailOpenRequested;
    }

    private void OnDisable()
    {
        EventHub.DetailButtonClicked -= OnDetailOpenRequested;
    }

    private void OnDetailOpenRequested()
    {
        foreach (var item in statLabels)
        {
            UpdateStatLable(item.Key);
        }

        panel.RemoveFromClassList("panel-close");
        panel.pickingMode = PickingMode.Position;
    }

    private void UpdateStatLable(EAttributeType attribute)
    {
        if (statLabels.TryGetValue(attribute, out var statLabel))
        {
            statLabel.text = uiController.GetAttributeValue(attribute).ToString();
        }
    }

    private void RegisterHover()
    {
        if (attributeInfoSO == null) return;

        foreach (var areaMap in statAreaMap)
        {
            var attributeType = areaMap.Key;
            var area = areaMap.Value;

            area.RegisterCallback<PointerEnterEvent>(e =>
            {
                descriptionText.text =
                    attributeInfoSO.GetAttributeDescription(attributeType);
            });
        }

    }

    private void OnCloseButtonClicked()
    {
        panel.AddToClassList("panel-close");
        panel.pickingMode = PickingMode.Ignore;
        descriptionText.text = string.Empty;
    }

}
