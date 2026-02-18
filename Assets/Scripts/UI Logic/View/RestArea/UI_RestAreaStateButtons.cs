using UnityEngine;
using UnityEngine.UIElements;

public class UI_RestAreaStateButtons 
{
    private UIController_RestArea uiController;

    const string strButtonName = "Button_Strength";
    const string intelliButtonName = "Button_Intelligence";
    const string dexButtonName = "Button_Dex";
    const string vitalButtonName = "Button_Vital";
    const string relaxButtonName = "Button_Relax";

    private VisualElement root;

    private Button strButton;
    private Button intelliButton;
    private Button dexButton;
    private Button vitalButton;
    private Button relaxButton;

    private bool isBound = false;

    public void Init(UIController_RestArea uiController, VisualElement root)
    {
        this.uiController = uiController;
        this.root = root;

        InitComponents();
        BindButtons();
    }
    private void InitComponents()
    {
        strButton = root.Q<Button>(strButtonName);
        intelliButton = root.Q<Button>(intelliButtonName);
        dexButton = root.Q<Button>(dexButtonName);
        vitalButton = root.Q<Button>(vitalButtonName);
        relaxButton = root.Q<Button>(relaxButtonName);
    }


    private void BindButtons()
    {
        if (isBound) return;

        BindUpgradeButton(strButton, EAttributeType.Strength);
        BindUpgradeButton(intelliButton, EAttributeType.Intelligence);
        BindUpgradeButton(dexButton, EAttributeType.Dexterity);
        BindUpgradeButton(vitalButton, EAttributeType.Vitality);
        relaxButton.clicked += OnRelaxButtonClicked;

        isBound = true;

    }

    void BindUpgradeButton(Button button, EAttributeType attributeToUpgrade)
    {
        button.clicked += () => OnUpgradeButtonClicked(attributeToUpgrade);
    }

    void OnUpgradeButtonClicked(EAttributeType attributeToUpgrade)
    {
        uiController.UpgaradeAttribute(attributeToUpgrade);
    }

    void OnRelaxButtonClicked()
    {
        uiController.Relax();
    }
}
