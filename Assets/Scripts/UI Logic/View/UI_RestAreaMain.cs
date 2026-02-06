using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_RestAreaMain : MonoBehaviour
{
    private UIController_RestArea uiController;

    private UI_RestAreaStatView statArea;
    private UI_RestAreaStateButtons statButtons;

    private void Awake()
    {
        uiController = new UIController_RestArea();
        uiController.Init(GetComponentInParent<IAbilitySystemContext>());

        var root = GetComponent<UIDocument>().rootVisualElement;

        statArea = new UI_RestAreaStatView();
        statArea.Init(uiController, root);

        statButtons = new UI_RestAreaStateButtons();
        statButtons.Init(uiController, root);
    }

    private void OnEnable()
    {
        EventHub.DialogueRequested += OnDialogueRequested;
        EventHub.DialogueFinished += OnDialogueFinished;
    }

    private void OnDisable()
    {
        EventHub.DialogueRequested -= OnDialogueRequested;
        EventHub.DialogueFinished -= OnDialogueFinished;
        GameManager.Instance.SceneChangingAsync -= HandleSceneLoadingUI;
        statArea?.Dispose();
    }

    private void Start()
    {
        GameManager.Instance.SceneChangingAsync += HandleSceneLoadingUI;
    }

    private Task HandleSceneLoadingUI()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;
        return Task.CompletedTask;
    }

    private void OnDialogueRequested(string dialogueId)
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        if (dialogueId == "End")
        {
            root.style.display = DisplayStyle.Flex;
        }
        else
        {
            root.style.display = DisplayStyle.None;
        }
    }

    private void OnDialogueFinished()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.Flex;
    }
}
