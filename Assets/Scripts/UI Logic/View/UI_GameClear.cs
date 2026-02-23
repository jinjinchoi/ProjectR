using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class UI_GameClear : MonoBehaviour
{
    VisualElement root;
    VisualElement panel;
    Label titleText;
    Label descriptionText;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;

        panel = root.Q<VisualElement>("Panel");

        titleText = root.Q<Label>("Title");
        titleText.AddToClassList("clear-title-hide");

        descriptionText = root.Q<Label>("Description");
        descriptionText.AddToClassList("clear-desc-hide");

    }

    void Start()
    {
        GameManager.Instance.GameClear += OnGameClear;
    }

    void OnGameClear()
    {
        root.style.display = DisplayStyle.Flex;
        titleText.RemoveFromClassList("clear-title-hide");
        descriptionText.RemoveFromClassList("clear-desc-hide");

        panel.RegisterCallbackOnce<MouseDownEvent>(OnPanelClikced);
    }

    void OnPanelClikced(MouseDownEvent evt)
    {
        panel.SetEnabled(false);
        GameManager.Instance.TravelToMainMenu();
    }
}
