using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_BattleOverlay : MonoBehaviour
{
    private UIController_HealthBar HealthBarUIController;
    private UIController_BattleOverlay battleOverlayController;
    private UI_TextHealthBarView healthBarUI;
    private VisualElement root;
    private VisualElement buffArea;
    private Label dayText;

    private Dictionary<EAbilityId, Image> activeBuffIcons = new();
    private const string buffIconUSSName = "buff-image";

    private void Awake()
    {
        HealthBarUIController = new UIController_HealthBar();
        HealthBarUIController.Init(GetComponentInParent<IAbilitySystemContext>());

        battleOverlayController = new UIController_BattleOverlay();
        battleOverlayController.Init(GetComponentInParent<IAbilitySystemContext>());

        root = GetComponent<UIDocument>().rootVisualElement;
        buffArea = root.Q<VisualElement>("BuffArea");
        dayText = root.Q<Label>("DayText");

        healthBarUI = new UI_TextHealthBarView(HealthBarUIController, root);
    }

    private void Start()
    {
        dayText.text = battleOverlayController.GetCurrentDay().ToString();
    }

    private void OnEnable()
    {
        battleOverlayController.BuffStart += CreateBuffIcon;
        battleOverlayController.BuffEnd += RemoveBuffIcon;
    }

    private void OnDisable()
    {
        healthBarUI?.Dispose();
        battleOverlayController.BuffStart -= CreateBuffIcon;
        battleOverlayController.BuffEnd -= RemoveBuffIcon;
    }

    private void CreateBuffIcon(BuffUIData data)
    {
        if (!activeBuffIcons.TryGetValue(data.Id, out var icon))
        {
            icon = new Image { image = data.Icon.texture };
            icon.AddToClassList(buffIconUSSName);
            activeBuffIcons[data.Id] = icon;
            buffArea.Add(icon);
        }

        icon.style.display = DisplayStyle.Flex;
    }

    private void RemoveBuffIcon(EAbilityId id)
    {
        if (activeBuffIcons.TryGetValue(id, out var icon))
            icon.style.display = DisplayStyle.None;
    }


}
