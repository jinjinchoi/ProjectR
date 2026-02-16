using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillUIWidget
{
    public EAbilityId Id { get; private set; }
    public VisualElement Slot { get; private set; }
    public Image Icon { get; private set; }
    public VisualElement DescriptionArea { get; private set; }
    public Label Description { get; private set; }
    public VisualElement BtnArrea { get; private set; }
    public Button AcquireButton { get; private set; }

    public SkillUIWidget(EAbilityId id, VisualElement slot, Image icon, VisualElement descArea, Label description, VisualElement btnArea , Button acquireButton)
    {
        this.Id = id;
        this.Slot = slot;
        this.Icon = icon;
        this.DescriptionArea = descArea;
        this.Description = description;
        this.BtnArrea = btnArea;
        this.AcquireButton = acquireButton;
    }
}

public class UI_SkillWindow : MonoBehaviour
{
    private VisualElement panel;
    private ScrollView skillScrollView;
    private UIController_Skill uiController;
    private Dictionary<EAbilityId, SkillUIWidget> skillUIMap = new();

    private const string skillSlotClassName = "skill-slot";
    private const string skillIconClassName = "skill-icon";
    private const string skillDescriptionClassName = "skill-description";
    private const string descAreaClassName = "skillTextArea";
    private const string acquireButtonAreaClassName = "acquireBtnArea";
    private const string skillAcquireButtonClassName = "skill-learn-button";

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        panel = root.Q<VisualElement>("Panel");
        panel.AddToClassList("panel-close");
        skillScrollView = root.Q<ScrollView>("SkillScroll");

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

    void OnPanelClicked(ClickEvent evt)
    {
        if (evt.target != panel)
            return;
        panel.AddToClassList("panel-close");
        panel.pickingMode = PickingMode.Ignore;
        evt.StopPropagation();
    }

    private void OnSkillWindowOpenRequested()
    {
        FAbilityUIInfo[] infoArray = uiController.GetAllAbilityUiInfo();
        if (infoArray == null || infoArray.Length == 0)
            return;
        
        panel.RemoveFromClassList("panel-close");
        panel.pickingMode = PickingMode.Position;

        foreach (var info in infoArray)
        {
            EAbilityId id = info.id;

            if (skillUIMap.ContainsKey(id))
            {
                UpdateSlot(id);
                continue;
            }

            Image icon = new() { image = info.icon ? info.icon.texture : null };
            icon.AddToClassList(skillIconClassName);

            Label description = new() {text = info.description };
            description.AddToClassList(skillDescriptionClassName);

            VisualElement descArea = new();
            descArea.AddToClassList(descAreaClassName);
            descArea.Add(description);

            Button acquireBtn = new() { text = "½Àµæ" };
            acquireBtn.AddToClassList(skillAcquireButtonClassName);
            BindAcquireButten(acquireBtn, id);

            VisualElement btnArea = new();
            btnArea.AddToClassList(acquireButtonAreaClassName);
            btnArea.Add(acquireBtn);

            VisualElement slot = new();
            slot.AddToClassList(skillSlotClassName);

            slot.Add(icon);
            slot.Add(descArea);
            slot.Add(btnArea);

            skillScrollView.Add(slot);

            SkillUIWidget skillWidget = new (id, slot, icon, descArea, description, btnArea, acquireBtn);
            skillUIMap.Add(id, skillWidget);
        }
    }

    void UpdateSlot(EAbilityId id)
    {
        if (uiController.IsUnLockedAbility(id))
        {
            skillUIMap.TryGetValue(id, out var widgetInfo);
            if (widgetInfo == null) return;

            widgetInfo.Slot.SetEnabled(false);
            Debug.Log($"Unlicked Ability : {id}");
        }
    }

    void BindAcquireButten(Button button, EAbilityId id)
    {
        button.clicked += () => OnAcquireButtonClicked(id);
    }
    
    void OnAcquireButtonClicked(EAbilityId id)
    {
        uiController.UnlockAbility(id);
        UpdateSlot(id);
    }
}
