using UnityEngine;
using UnityEngine.UIElements;

public class UI_Loading : MonoBehaviour
{
    VisualElement panel;
    VisualElement guideText;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        panel = root.Q<VisualElement>("Panel");
        panel.focusable = true;
        panel.Focus();
        panel.RegisterCallback<KeyDownEvent>(OnKeyDown);
        panel.RegisterCallback<MouseDownEvent>(OnMouseDownEvent);

        guideText = root.Q<Label>("Text_Guide");
        guideText.RegisterCallbackOnce<GeometryChangedEvent>(OnGuideTextGeometryChanged);
        guideText.RegisterCallback<TransitionEndEvent>(OnGuideTextTransitionEnd);
    }

    private void OnGuideTextGeometryChanged(GeometryChangedEvent evt)
    {
        guideText.ToggleInClassList("loading-guide--translucent");
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        HideLoadingScreen();
    }


    private void OnMouseDownEvent(MouseDownEvent evt)
    {
        HideLoadingScreen();
    }

    private void HideLoadingScreen()
    {
        if (GameManager.Instance == null) return;

        panel.UnregisterCallback<KeyDownEvent>(OnKeyDown);
        panel.UnregisterCallback<MouseDownEvent>(OnMouseDownEvent);

        guideText.UnregisterCallback<TransitionEndEvent>(OnGuideTextTransitionEnd);

        var root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;

        GameManager.Instance.ProcessDay();
    }


    private void OnGuideTextTransitionEnd(TransitionEndEvent evt)
    {
        guideText.ToggleInClassList("loading-guide--translucent");
    }
}

