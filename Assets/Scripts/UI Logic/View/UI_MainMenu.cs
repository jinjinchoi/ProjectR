using UnityEngine;
using UnityEngine.UIElements;

public class UI_MainMenu : MonoBehaviour
{
    private UIController_MainMenu uiController;

    private Button newGameButton;
    private Button loadButton;
    private Button exitButton;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        newGameButton = root.Q<Button>("NewGameButton");
        loadButton = root.Q<Button>("LoadButton");
        exitButton = root.Q<Button>("ExitButton");
    }

    private void OnEnable()
    {
        newGameButton.clicked += OnNewGameButtonClicked;
        loadButton.clicked += OnLoadButtonClicked;
        exitButton.clicked += OnExitButtonClicked;
    }

    private void Start()
    {
        uiController = new UIController_MainMenu();
        if (!uiController.IsSaveDataExisted())
        {
            loadButton.SetEnabled(false);
        }
    }

    private void OnNewGameButtonClicked()
    {
        GameManager.Instance.NewGame();
    }

    private void OnLoadButtonClicked()
    {
        GameManager.Instance.LoadGame();
    }

    private void OnExitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
