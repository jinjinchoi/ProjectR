using UnityEngine;
using UnityEngine.UIElements;

public class UI_TextHealthBar
{
    private UIController_HealthBar uiController;

    private readonly VisualElement root;
    private VisualElement progressbarMain;
    private Label currentHealthText;
    private Label maxHealthText;

    [SerializeField] const string progressbarName = "HealthBar-Main";
    private const string HealthWaningClassName = "progressbar-lobby-main-warning";
    private const string HealthDangerClassName = "progressbar-lobby-main-danger";
    const string currentHealthTextName = "Text_CurrentHealth";
    const string maxHealthTextName = "Text_MaxHealth";

    public UI_TextHealthBar(UIController_HealthBar uiController, VisualElement root)
    {
        this.uiController = uiController;
        this.root = root;

        InitUIComponents();
        RegisterCallbacks();
    }

    private void InitUIComponents()
    {
        progressbarMain = root.Q<VisualElement>(progressbarName);
        currentHealthText = root.Q<Label>(currentHealthTextName);
        maxHealthText = root.Q<Label>(maxHealthTextName);

        float maxHealth = uiController.GetAttributeValue(EAttributeType.maxHealth);
        float currentHealth = uiController.GetAttributeValue(EAttributeType.currentHealth);
        currentHealthText.text = currentHealth.ToString();
        maxHealthText.text = maxHealth.ToString();

        float HealthRatio = uiController.GetHealthRatio();
        OnHeathRatioChanged(true, HealthRatio);
    }

    public void Dispose()
    {
        uiController.OnVitalRatioChanged -= OnHeathRatioChanged;
    }

    private void RegisterCallbacks()
    {
        uiController.OnVitalRatioChanged += OnHeathRatioChanged;

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
}
