using UnityEngine;
using UnityEngine.UIElements;

public class UI_RestAreaMain : MonoBehaviour
{
    private UIController_RestArea uiController;

    private UI_RestAreaStatView statArea;
    private UI_RestAreaStateButtons statButtons;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement panel = root.Q<VisualElement>("Panel");
        RestAreaManager.Instance.SetOverlayPannel(panel);
    }

    private void Start()
    {
        uiController = GetComponentInParent<UIController_RestArea>();
        var root = GetComponent<UIDocument>().rootVisualElement;

        statArea = new UI_RestAreaStatView();
        statArea.Init(uiController, root);

        statButtons = new UI_RestAreaStateButtons();
        statButtons.Init(uiController, root);
    }
}
