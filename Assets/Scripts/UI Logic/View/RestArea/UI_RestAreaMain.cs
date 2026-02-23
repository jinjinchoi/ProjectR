using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_RestAreaMain : MonoBehaviour
{
    private UIController_RestArea uiController;
    private UIController_HealthBar HealthBarUIController;

    private UI_RestAreaStatView statArea;
    private UI_RestAreaStateButtons statButtons;
    private UI_TextHealthBarView healthBarView;
    private UI_RestAreaGameButtons gameButtons;

    private VisualElement root;

    private void Start()
    {
        uiController = new UIController_RestArea();
        uiController.Init(GetComponentInParent<IAbilitySystemContext>());

        HealthBarUIController = new UIController_HealthBar();
        HealthBarUIController.Init(GetComponentInParent<IAbilitySystemContext>());

        root = GetComponent<UIDocument>().rootVisualElement;

        statArea = new UI_RestAreaStatView();
        statArea.Init(uiController, root);

        statButtons = new UI_RestAreaStateButtons();
        statButtons.Init(uiController, root);

        healthBarView = new UI_TextHealthBarView(HealthBarUIController, root);
        gameButtons = new UI_RestAreaGameButtons(root);

        GameManager.Instance.SceneChangingAsync += HandleSceneLoadingUI;
        GameManager.Instance.EventManager.OnBattleStarting += HideOverlay;
        GameManager.Instance.GameClear += HideOverlay;
    }

    private void OnEnable()
    {
        EventHub.DialogueRequested += OnDialogueRequested;
        EventHub.DialogueFinished += ShowOverlay;
    }

    private void OnDisable()
    {
        EventHub.DialogueRequested -= OnDialogueRequested;
        EventHub.DialogueFinished -= ShowOverlay;
        GameManager.Instance.SceneChangingAsync -= HandleSceneLoadingUI;
        GameManager.Instance.EventManager.OnBattleStarting -= HideOverlay;
        GameManager.Instance.GameClear -= HideOverlay;

        statArea?.Dispose();
        healthBarView?.Dispose();
        gameButtons?.Dispose();
    }

    private Task HandleSceneLoadingUI()
    {
        root.style.display = DisplayStyle.None;
        return Task.CompletedTask;
    }

    private void OnDialogueRequested(string dialogueId)
    {
        if (dialogueId == "End")
            root.style.display = DisplayStyle.Flex;
        else
            root.style.display = DisplayStyle.None;
    }

    private void ShowOverlay()
    {
        root.style.display = DisplayStyle.Flex;
    }

    private void HideOverlay()
    {
        root.style.display = DisplayStyle.None;
    }
}
