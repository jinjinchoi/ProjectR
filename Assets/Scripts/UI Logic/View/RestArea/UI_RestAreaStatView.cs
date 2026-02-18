using UnityEngine;
using UnityEngine.UIElements;

public class UI_RestAreaStatView
{
    #region component names
    const string strTextName = "StrengthText";
    const string intelliTextName = "IntelligenceText";
    const string dexTextName = "DexText";
    const string vitalTextName = "VitalText";
    const string successChanceTextName = "Text_SuccessChance";
    const string costTextName = "Text_Cost";
    const string strUpTextName = "StrUpText";
    const string intelliUpTextName = "IntelliUpText";
    const string dexUpTextName = "DexUpText";
    const string vitalUpTextName = "VitalUpText";
    const string relaxTextName = "RelaxText";
    const string dayTextName = "DayText";
    const string skillPointTextName = "SkillPoint";
    #endregion


    private UIController_RestArea uiController;

    private VisualElement root;

    private Label strText;
    private Label intelliText;
    private Label dexText;
    private Label vitalText;
    private Label successChanceText;
    private Label costText;
    private Label skillPointText;

    private Label strUpText;
    private Label intelliUpText;
    private Label dexUpText;
    private Label vitalUpText;
    private Label relaxText;

    private Label dayText;

    public void Init(UIController_RestArea uiController, VisualElement root)
    {
        this.uiController = uiController;
        this.root = root;

        InitUIComponents();
        RegisterCallbacks();
        InitAttributeText();
        UpdateUpgradeValueText();
        UpdateCostAndChanceText();
    }

    public void Dispose()
    {
        uiController.OnAttributeValueChanged -= OnAttributeValueChanged;
        GameManager.Instance.DayChanged -= OnDayChanged;
    }
    
    private void InitUIComponents()
    {
        strText = root.Q<Label>(strTextName);
        intelliText = root.Q<Label>(intelliTextName);
        dexText = root.Q<Label>(dexTextName);
        vitalText = root.Q<Label>(vitalTextName);
        successChanceText = root.Q<Label>(successChanceTextName);
        costText = root.Q<Label>(costTextName);
        skillPointText = root.Q<Label>(skillPointTextName);

        strUpText = root.Q<Label>(strUpTextName);
        intelliUpText = root.Q<Label>(intelliUpTextName);
        dexUpText = root.Q<Label>(dexUpTextName);
        vitalUpText = root.Q<Label>(vitalUpTextName);
        relaxText = root.Q<Label>(relaxTextName);

        dayText = root.Q<Label>(dayTextName);
    }

    private void RegisterCallbacks()
    {
        uiController.OnAttributeValueChanged += OnAttributeValueChanged;
        GameManager.Instance.DayChanged += OnDayChanged;
    }

    private void InitAttributeText()
    {
        float str = uiController.GetAttributeValue(EAttributeType.Strength);
        float intelli = uiController.GetAttributeValue(EAttributeType.Intelligence);
        float dex = uiController.GetAttributeValue(EAttributeType.Dexterity);
        float vital = uiController.GetAttributeValue(EAttributeType.Vitality);
        float maxHealth = uiController.GetAttributeValue(EAttributeType.MaxHealth);
        float currentHealth = uiController.GetAttributeValue(EAttributeType.CurrentHealth);
        float skillPoint = uiController.GetAttributeValue(EAttributeType.SkillPoint);

        strText.text = str.ToString();
        intelliText.text = intelli.ToString();
        dexText.text = dex.ToString();
        vitalText.text = vital.ToString();
        skillPointText.text = skillPoint.ToString();
    }
    

    private void UpdateUpgradeValueText()
    {
        strUpText.text = uiController.GetUpgradeValue(EAttributeType.Strength).ToString();
        intelliUpText.text = uiController.GetUpgradeValue(EAttributeType.Intelligence).ToString();
        dexUpText.text = uiController.GetUpgradeValue(EAttributeType.Dexterity).ToString();
        vitalUpText.text = uiController.GetUpgradeValue (EAttributeType.Vitality).ToString();

        relaxText.text = uiController.GetRelaxValue().ToString();
    }

    private void UpdateCostAndChanceText()
    {
        successChanceText.text = $"{uiController.GetSuccessChance() : 0}%";
        costText.text = uiController.GetUpgradeCost().ToString();
    }

    private void OnAttributeValueChanged(EAttributeType attribute, float value)
    {
        switch (attribute)
        {
            case EAttributeType.Strength:
                strText.text = value.ToString();
                break;

            case EAttributeType.Dexterity:
                dexText.text = value.ToString();
                break;

            case EAttributeType.Intelligence:
                intelliText.text = value.ToString();
                break;

            case EAttributeType.Vitality:
                vitalText.text = value.ToString();
                break;

            case EAttributeType.SkillPoint:
                skillPointText.text = value.ToString();
                break;

            default:
                break;
        }
    }

    private void OnDayChanged(int day)
    {
        dayText.text = day.ToString();

        UpdateCostAndChanceText();
        UpdateUpgradeValueText();
    }

}
