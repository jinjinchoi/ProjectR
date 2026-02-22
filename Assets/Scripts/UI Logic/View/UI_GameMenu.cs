using UnityEngine;
using UnityEngine.UIElements;

public class UI_GameMenu : MonoBehaviour
{
    private VisualElement panel;
    private Button gameQuitButton;
    private Button saveButton;
    private Button menuCloseButton;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        panel = root.Q<VisualElement>("Panel");
        panel.AddToClassList("panel-close");
        panel.pickingMode = PickingMode.Ignore;

        gameQuitButton = root.Q<Button>("QuitButton");
        saveButton = root.Q<Button>("SaveButton");
        menuCloseButton = root.Q<Button>("CloseButton");
    }

    private void OnEnable()
    {
        EventHub.GameMenuOpen += OnMenuOpenReceived;

        menuCloseButton.clicked += OnCloseButtonClicked;
        gameQuitButton.clicked += OnQuitButtonClicked;
        saveButton.clicked += OnSaveButtonClicked;
    }

    private void OnDisable()
    {
        EventHub.GameMenuOpen -= OnMenuOpenReceived;

        menuCloseButton.clicked -= OnCloseButtonClicked;
        gameQuitButton.clicked -= OnQuitButtonClicked;
        saveButton.clicked -= OnSaveButtonClicked;
    }

    void OnMenuOpenReceived()
    {
        panel.pickingMode = PickingMode.Position;
        panel.RemoveFromClassList("panel-close");
    }

    void OnCloseButtonClicked()
    {
        panel.pickingMode = PickingMode.Ignore;
        panel.AddToClassList("panel-close");
    }

    void OnSaveButtonClicked()
    {
        var player = GetComponentInParent<PlayerCharacter>();
        if (player != null) player.SaveDataToRuntimeGameState();

        GameManager.Instance.SaveGame();
    }

    void OnQuitButtonClicked()
    {
        GameManager.Instance.TravelToMainMenu();
    }
}
