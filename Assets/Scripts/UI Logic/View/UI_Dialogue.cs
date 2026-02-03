using UnityEngine;
using UnityEngine.UIElements;

public class UI_Dialogue : MonoBehaviour
{
    VisualElement root;
    VisualElement panel;
    VisualElement nextArrow;
    Label speakerName;
    Label dialogueText;


    UIController_Dialogue uiController;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;

        panel = root.Q<VisualElement>("Panel");
        panel.RegisterCallback<MouseDownEvent>(OnPanelMouseClicked);

        speakerName = root.Q<Label>("Text_CharacterName");
        dialogueText = root.Q<Label>("Text_Main");
        nextArrow = root.Q<VisualElement>("Img_NextArrow");

        uiController = new UIController_Dialogue();
        uiController.Init();
    }

    private void OnEnable()
    {
        EventHub.DialogueRequested += OnDialogueRequested;
        EventHub.DialogueFinished += OnDialogueFinished;
        uiController.TextDialogueRequested += OnNormalDialogueReceived;
    }

    private void OnDisable()
    {
        EventHub.DialogueRequested -= OnDialogueRequested;
        EventHub.DialogueFinished -= OnDialogueFinished;
        uiController.TextDialogueRequested -= OnNormalDialogueReceived;
        uiController?.Dispose();
    }
    private void OnDialogueRequested(string dialogueId)
    {
        if (dialogueId == "End")
            root.style.display = DisplayStyle.None;
        else
            root.style.display = DisplayStyle.Flex;
    }

    private void OnDialogueFinished()
    {
        root.style.display = DisplayStyle.None;
    }

    private void OnNormalDialogueReceived(FNormalDialogueInfo dialogueInfo)
    {
        speakerName.text = dialogueInfo.speakerName;
        dialogueText.text = dialogueInfo.text;
    }

    private void OnPanelMouseClicked(MouseDownEvent evt)
    {
        uiController.CallCurrentDialgoue();
    }
}
