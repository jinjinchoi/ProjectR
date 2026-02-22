using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class UI_SkillWindow : MonoBehaviour
{
    [SerializeField] VisualTreeAsset skillInWidgetTemplate;

    private VisualElement panel;
    private ScrollView skillScrollView;
    private Label currentSPText;

    private UIController_Skill uiController;
    private readonly Dictionary<EAbilityId, VisualElement> skillUIMap = new();

    const string unlockButtonName = "UnlockButton";

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        panel = root.Q<VisualElement>("Panel");
        panel.AddToClassList("panel-close");
        panel.pickingMode = PickingMode.Ignore;

        skillScrollView = root.Q<ScrollView>("SkillScroll");
        currentSPText = root.Q<Label>("CurrentSP");

        uiController = new UIController_Skill(GetComponentInParent<PlayerCharacter>());
    }

    private void OnEnable()
    {
        EventHub.SkillButtonClicked += OnSkillWindowOpenRequested;
        panel.RegisterCallback<ClickEvent>(OnPanelClicked);
    }

    private void OnDisable()
    {
        EventHub.SkillButtonClicked -= OnSkillWindowOpenRequested;
        panel.UnregisterCallback<ClickEvent>(OnPanelClicked);

    }

    private void OnSkillWindowOpenRequested()
    {
        panel.pickingMode = PickingMode.Position;
        panel.RemoveFromClassList("panel-close");

        UpdateCurrentSpText();
        CreateOrUpdateSkillWidget();
    }

    // Close skill window on panel click.
    void OnPanelClicked(ClickEvent evt)
    {
        if (evt.target != panel)
            return;
        panel.pickingMode = PickingMode.Ignore;
        panel.AddToClassList("panel-close");
        evt.StopPropagation();
    }

    void OnUnloackButtonClicked(EAbilityId id)
    {
        DebugHelper.Log($"[{id}] slot is clikced");
        uiController.UnlockAbility(id);
        CreateOrUpdateSkillWidget();
        UpdateCurrentSpText();
    }

    private void CreateOrUpdateSkillWidget()
    {
        FAbilityUIInfo[] infoArray = uiController.GetAllAbilityUiInfo();
        if (infoArray.Length == 0)
            return;

        foreach (var info in infoArray)
        {
            if (skillUIMap.ContainsKey(info.id))
            {
                UpdateWidget(info.id);
                continue;
            }

            var elemet = CreateSkillWidget(info);
            skillUIMap.Add(info.id, elemet);
            skillScrollView.Add(elemet);
        }
    }

    private VisualElement CreateSkillWidget(FAbilityUIInfo skillInfo)
    {
        VisualElement template = skillInWidgetTemplate.CloneTree();

        Image icon = template.Q<Image>("Icon");
        Label skillName = template.Q<Label>("SkillName");
        Label skillDesc = template.Q<Label>("Description");
        Label sp = template.Q<Label>("SkillPoint");
        Button unlockBtn = template.Q<Button>(unlockButtonName);

        icon.image = skillInfo.icon.texture;
        skillName.text = skillInfo.name;
        skillDesc.text = skillInfo.description;
        sp.text = skillInfo.sp.ToString();

        var id = skillInfo.id;
        unlockBtn.clicked += () =>
        {
            OnUnloackButtonClicked(id);
        };

        if (uiController.GetCurrentSp() < skillInfo.sp)
            unlockBtn.SetEnabled(false);

        UpdateButtonEnabled(skillInfo.id, unlockBtn);

        return template;
    }


    void UpdateWidget(EAbilityId slotId)
    {
        if (!skillUIMap.TryGetValue(slotId, out VisualElement skillWidget))
            return;

        if (uiController.IsUnLockedAbility(slotId))
        {
            skillWidget.SetEnabled(false);
            return;
        }

        var button = skillWidget.Q<Button>(unlockButtonName);
        UpdateButtonEnabled(slotId, button);

    }

    private void UpdateButtonEnabled(EAbilityId slotId, Button button)
    {
        if (uiController.GetCurrentSp() < uiController.GetAbilityRequiredSp(slotId))
            button.SetEnabled(false);
        else
            button.SetEnabled(true);
    }

    private void UpdateCurrentSpText()
    {
        currentSPText.text = uiController.GetCurrentSp().ToString();
    }
}
