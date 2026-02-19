using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UI_GameOver : MonoBehaviour
{
    UIController_GameOver uiController;

    VisualElement panel;
    Button restartButton;
    Button loadButton;
    Button quiteButton;

    private void Awake()
    {
        uiController = new UIController_GameOver();

        var root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;

        panel = root.Q<VisualElement>("Panel");
        panel.AddToClassList("gameover-panel--hide");

        restartButton = root.Q<Button>("RestartButton");
        loadButton = root.Q<Button>("LoadButton");
        quiteButton = root.Q<Button>("QuiteButton");
    }


    private void OnEnable()
    {
        EventHub.PlayerDied += OnPlayerDied;
        restartButton.clicked += OnRestartButtonClicked;
        loadButton.clicked += OnLoadButtonClicked;
        quiteButton.clicked += OnQuiteButtonClicked;
    }

    private void OnDisable()
    {
        EventHub.PlayerDied -= OnPlayerDied;
        restartButton.clicked -= OnRestartButtonClicked;
        loadButton.clicked -= OnLoadButtonClicked;
        quiteButton.clicked -= OnQuiteButtonClicked;
    }

    private void OnPlayerDied()
    {
        Debug.Log("UI: Player Died");
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.Flex;
        
        panel.RemoveFromClassList("gameover-panel--hide");
    }

    private void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnLoadButtonClicked()
    {

    }

    private void OnQuiteButtonClicked()
    {

    }

}
