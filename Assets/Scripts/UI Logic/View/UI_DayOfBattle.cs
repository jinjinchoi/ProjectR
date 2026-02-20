using UnityEngine;
using UnityEngine.UIElements;

public class UI_DayOfBattle : MonoBehaviour
{
    VisualElement root;

    VisualElement panel;
    Label titleText;
    Label guideText;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;

        panel = root.Q<VisualElement>("Panel");

        titleText = root.Q<Label>("TitleText");
        titleText.AddToClassList("start-text-hide");

        guideText = root.Q<Label>("ClickText");
        guideText.AddToClassList("click-text-hide");
    }

    private void Start()
    {
        GameManager.Instance.EventManager.OnBattleStarting += OnBattleStarting;
    }

    void OnBattleStarting()
    {
        root.style.display = DisplayStyle.Flex;
        titleText.RemoveFromClassList("start-text-hide");
        guideText.RemoveFromClassList("click-text-hide");

        panel.RegisterCallbackOnce<KeyDownEvent>(OnKeyDown);
        panel.RegisterCallbackOnce<MouseDownEvent>(OnMouseDownEvent);
        panel.Focus();
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        TravelMap();
    }


    private void OnMouseDownEvent(MouseDownEvent evt)
    {
        TravelMap();
    }

    void TravelMap()
    {
        panel.SetEnabled(false);
        GameManager.Instance.EventManager.BattleStart();
    }
}
