using UnityEngine;
using UnityEngine.UIElements;

public class UI_RestAreaStatView
{
    #region component names
    const string strTextName = "StrengthText";
    const string intelliTextName = "IntelligenceText";
    const string dexTextName = "DexText";
    const string vitalTextName = "VitalText";
    const string currentHealthTextName = "Text_CurrentHealth";
    const string maxHealthTextName = "Text_MaxHealth";
    const string successChanceTextName = "Text_SuccessChance";
    const string costTextName = "Text_Cost";
    const string progressbarName = "HealthBar-Main";
    const string strUpTextName = "StrUpText";
    const string intelliUpTextName = "IntelliUpText";
    const string dexUpTextName = "DexUpText";
    const string vitalUpTextName = "VitalUpText";
    const string relaxTextName = "RelaxText";
    const string dayTextName = "DayText";
    #endregion

    #region css names
    private const string HealthWaningClassName = "progressbar-lobby-main-warning";
    private const string HealthDangerClassName = "progressbar-lobby-main-danger";
    #endregion

    private UIController_RestArea uiController;

    private VisualElement root;

    private VisualElement progressbarMain;
    private Label strText;
    private Label intelliText;
    private Label dexText;
    private Label vitalText;
    private Label currentHealthText;
    private Label maxHealthText;
    private Label successChanceText;
    private Label costText;

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
        uiController.OnVitalRatioChanged -= OnHeathRatioChanged;
        GameManager.Instance.DayChanged -= OnDayChanged;
    }
    
    private void InitUIComponents()
    {
        strText = root.Q<Label>(strTextName);
        intelliText = root.Q<Label>(intelliTextName);
        dexText = root.Q<Label>(dexTextName);
        vitalText = root.Q<Label>(vitalTextName);
        currentHealthText = root.Q<Label>(currentHealthTextName);
        maxHealthText = root.Q<Label>(maxHealthTextName);
        successChanceText = root.Q<Label>(successChanceTextName);
        costText = root.Q<Label>(costTextName);
        progressbarMain = root.Q<VisualElement>(progressbarName);

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
        uiController.OnVitalRatioChanged += OnHeathRatioChanged;

        GameManager.Instance.DayChanged += OnDayChanged;
    }

    private void InitAttributeText()
    {
        float str = uiController.GetAttributeValue(EAttributeType.strength);
        float intelli = uiController.GetAttributeValue(EAttributeType.intelligence);
        float dex = uiController.GetAttributeValue(EAttributeType.dexterity);
        float vital = uiController.GetAttributeValue(EAttributeType.vitality);
        float maxHealth = uiController.GetAttributeValue(EAttributeType.maxHealth);
        float currentHealth = uiController.GetAttributeValue(EAttributeType.currentHealth);

        strText.text = str.ToString();
        intelliText.text = intelli.ToString();
        dexText.text = dex.ToString();
        vitalText.text = vital.ToString();
        currentHealthText.text = currentHealth.ToString();
        maxHealthText.text = maxHealth.ToString();

        float HealthRatio = uiController.GetHealthPercent();
        OnHeathRatioChanged(true, HealthRatio);
    }
    

    private void UpdateUpgradeValueText()
    {
        strUpText.text = uiController.GetUpgradeValue(EAttributeType.strength).ToString();
        intelliUpText.text = uiController.GetUpgradeValue(EAttributeType.intelligence).ToString();
        dexUpText.text = uiController.GetUpgradeValue(EAttributeType.dexterity).ToString();
        vitalUpText.text = uiController.GetUpgradeValue (EAttributeType.vitality).ToString();

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
            case EAttributeType.strength:
                strText.text = value.ToString();
                break;

            case EAttributeType.dexterity:
                dexText.text = value.ToString();
                break;

            case EAttributeType.intelligence:
                intelliText.text = value.ToString();
                break;

            case EAttributeType.vitality:
                vitalText.text = value.ToString();
                float maxHealth = uiController.GetAttributeValue(EAttributeType.maxHealth);

                break;

            case EAttributeType.currentHealth:
                currentHealthText.text = value.ToString();
                break;

            case EAttributeType.maxHealth:
                maxHealthText.text = value.ToString();
                break;

            default:
                break;
        }

        
    }

    private void OnHeathRatioChanged(bool isHealth, float ratio)
    {
        if (!isHealth) return;
        
        progressbarMain.style.width = Length.Percent(ratio * 100f);

        progressbarMain.RemoveFromClassList(HealthWaningClassName);
        progressbarMain.RemoveFromClassList(HealthDangerClassName);

        if (ratio < 0.6f)
        {
            progressbarMain.AddToClassList(HealthWaningClassName);
        }

        if (ratio < 0.3f)
        {
            progressbarMain.AddToClassList(HealthDangerClassName);
        }

        float maxHealth = uiController.GetAttributeValue(EAttributeType.maxHealth);
        float currentHealth = uiController.GetAttributeValue(EAttributeType.currentHealth);
        currentHealthText.text = currentHealth.ToString();
        maxHealthText.text = maxHealth.ToString();
    }

    private void OnDayChanged(int day)
    {
        dayText.text = day.ToString();

        UpdateCostAndChanceText();
        UpdateUpgradeValueText();
    }

}
