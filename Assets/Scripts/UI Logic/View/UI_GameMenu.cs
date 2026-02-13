using UnityEngine;
using UnityEngine.UIElements;

public class UI_GameMenu : MonoBehaviour
{
    private Button menuCloseButton;
    private Button gameQuitButton;
    private VisualElement panel;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        menuCloseButton = root.Q<Button>("CloseButton");
        gameQuitButton = root.Q<Button>("QuitButton");
        panel = root.Q<VisualElement>("Panel");
        panel.AddToClassList("panel-close");

        menuCloseButton.clicked += OnCloseButtonClicked;
        gameQuitButton.clicked += OnQuitButtonClicked;
    }

    private void OnEnable()
    {
        EventHub.GameMenuOpen += OnMenuOpenReceived;
    }

    void OnMenuOpenReceived()
    {
        panel.RemoveFromClassList("panel-close");
        panel.pickingMode = PickingMode.Position;
    }

    void OnCloseButtonClicked()
    {
        panel.AddToClassList("panel-close");
        panel.pickingMode = PickingMode.Ignore;
    }

    void OnQuitButtonClicked()
    {

    }
}
