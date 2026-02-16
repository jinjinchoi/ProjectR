using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillUIWidget
{
    public EAbilityId Id { get; private set; }
    public VisualElement Slot { get; private set; }
    public Image Icon { get; private set; }
    public Label Description { get; private set; }
    public VisualElement BtnArrea { get; private set; }
    public Button AcquireButton { get; private set; }

    public SkillUIWidget(EAbilityId id, VisualElement slot, Image icon, Label description, VisualElement btnArea , Button acquireButton)
    {
        this.Id = id;
        this.Slot = slot;
        this.Icon = icon;
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
    private const string acquireButtonAreaClassName = "acquireBtnArea";
    private const string skillAcquireButtonClassName = "skill-learn-button";

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        panel = root.Q<VisualElement>("Panel");
        panel.AddToClassList("panel-close");
        skillScrollView = root.Q<ScrollView>("SkillScroll");

        uiController = new UIController_Skill(GetComponentInParent<BaseCharacter>());
    }

    private void OnEnable()
    {
        EventHub.SkillButtonClicked += OnSkillWindowOpenRequested;
    }

    private void OnDisable()
    {
        EventHub.SkillButtonClicked -= OnSkillWindowOpenRequested;
    }

    private void OnSkillWindowOpenRequested()
    {
        FAbilityUIInfo[] infoArray = uiController.GetAllAbilityUiInfo();
        if (infoArray == null || infoArray.Length == 0)
            return;
        
        panel.RemoveFromClassList("panel-close");

        foreach (var info in infoArray)
        {
            if (skillUIMap.ContainsKey(info.id))
                continue;
            
            EAbilityId id = info.id;

            Image icon = new() { image = info.icon ? info.icon.texture : null };
            icon.AddToClassList(skillIconClassName);

            Label description = new() {text = info.description };
            description.AddToClassList(skillDescriptionClassName);

            Button acquireBtn = new() { text = "½Àµæ" };
            acquireBtn.AddToClassList(skillAcquireButtonClassName);

            VisualElement btnArea = new();
            btnArea.AddToClassList(acquireButtonAreaClassName);
            btnArea.Add(acquireBtn);

            VisualElement slot = new();
            slot.AddToClassList(skillSlotClassName);

            slot.Add(icon);
            slot.Add(description);
            slot.Add(acquireBtn);

            skillScrollView.Add(slot);

            SkillUIWidget skillWidget = new (id, slot, icon, description, btnArea, acquireBtn);
            skillUIMap.Add(id, skillWidget);
        }
    }

    
}
