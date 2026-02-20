using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class UI_WinOverlay : MonoBehaviour
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
        titleText.AddToClassList("title-text-big");

        guideText = root.Q<Label>("GuideText");
        guideText.AddToClassList("guide-text-hide");
    }

    private void OnEnable()
    {
        BattleManager.BattleWon += HandleBattleWon;
    }

    private void OnDisable()
    {
        BattleManager.BattleWon -= HandleBattleWon;
    }

    private void HandleBattleWon()
    {
        root.style.display = DisplayStyle.Flex;

        titleText.RemoveFromClassList("title-text-big");
        guideText.RemoveFromClassList("guide-text-hide");

        panel.RegisterCallbackOnce<KeyDownEvent>(OnKeyDown);
        panel.RegisterCallbackOnce<MouseDownEvent>(OnMouseDownEvent);
        panel.Focus();
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        TravelToRestArea();
    }


    private void OnMouseDownEvent(MouseDownEvent evt)
    {
        TravelToRestArea();
    }

    void TravelToRestArea()
    {
        panel.SetEnabled(false);
        GameManager.Instance.TravelToRestArea();
    }
}
