using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_Dialogue : MonoBehaviour
{
    VisualElement root;
    VisualElement panel;
    VisualElement nextArrow;
    Label speakerName;
    Label dialogueText;

    List<Button> selectButtons = new();
    List<FChoiceButtonInfo> choiceButtonInfos = new();

    UIController_Dialogue uiController;

    string fullText;
    int textIndex;
    IVisualElementScheduledItem typingSchedule;
    bool isTyping = false;
    bool isWatingForSelect = false;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;

        panel = root.Q<VisualElement>("Panel");
        panel.RegisterCallback<MouseDownEvent>(OnPanelMouseClicked);

        speakerName = root.Q<Label>("Text_CharacterName");
        dialogueText = root.Q<Label>("Text_Main");
        nextArrow = root.Q<VisualElement>("Img_NextArrow");

        selectButtons = new List<Button>
        {
            root.Q<Button>("SelectButton_01"),
            root.Q<Button>("SelectButton_02"),
            root.Q<Button>("SelectButton_03"),
        };

        
        for (int i = 0; i < selectButtons.Count; i++)
        {
            int index = i;
            selectButtons[i].clicked += () => OnChoiceButtonClicked(index);
        }

        AbilitySystemComponent asc = GetComponentInParent<AbilitySystemComponent>();
        uiController = new UIController_Dialogue();
        uiController.Init(asc);
    }

    private void OnEnable()
    {
        EventHub.DialogueRequested += OnDialogueRequested;
        EventHub.DialogueFinished += OnDialogueFinished;
        uiController.TextDialogueRequested += OnNormalDialogueReceived;
        uiController.ChoiceDialogueRequested += OnChoiceDialogueReceived;
    }

    private void OnDisable()
    {
        EventHub.DialogueRequested -= OnDialogueRequested;
        EventHub.DialogueFinished -= OnDialogueFinished;
        uiController.TextDialogueRequested -= OnNormalDialogueReceived;
        uiController.ChoiceDialogueRequested -= OnChoiceDialogueReceived;
        uiController?.Dispose();
        typingSchedule?.Pause();
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
        StartTypeWriterEffect(dialogueInfo.text);
    }

    private void OnChoiceDialogueReceived(List<FChoiceButtonInfo> choiceInfo)
    {
        isWatingForSelect = true;

        if (choiceInfo.Count > selectButtons.Count)
        {
            Debug.LogWarning($"[Dialogue] Choice count ({choiceInfo.Count}) exceeds button count ({selectButtons.Count}).");
            return;
        }

        for (int i = 0; i < choiceInfo.Count; i++)
        {
            selectButtons[i].text = choiceInfo[i].choiceText;
            selectButtons[i].style.display = DisplayStyle.Flex;
            choiceButtonInfos.Add(choiceInfo[i]);
        }
    }

    private void OnChoiceButtonClicked(int buttonIndex)
    {
        if (string.IsNullOrEmpty(choiceButtonInfos[buttonIndex].nextNodeId))
        {
            Debug.LogWarning("next node id is empty");
            return;
        }

        foreach (Button button in selectButtons)
        {
            button.text = string.Empty;
            button.style.display = DisplayStyle.None;
        }

        uiController.CallDialogueById(choiceButtonInfos[buttonIndex].nextNodeId);
        choiceButtonInfos.Clear();
        isWatingForSelect = false;
    }

    private void OnPanelMouseClicked(MouseDownEvent evt)
    {
        if (isWatingForSelect) return;

        if (isTyping)
            SkipTypeWriterEffect();
        else
            uiController.CallCurrentDialgoue();

    }

    private void StartTypeWriterEffect(string fullDialogue)
    {
        fullText = fullDialogue;
        textIndex = 0;
        dialogueText.text = string.Empty;
        typingSchedule?.Pause();
        isTyping = true;

        typingSchedule = dialogueText.schedule.Execute(() =>
        {
            dialogueText.text = fullText[..textIndex];
            textIndex++;

            if (textIndex > fullText.Length)
            {
                typingSchedule.Pause();
                isTyping = false;
            }

        }).Every(50);
    }

    private void SkipTypeWriterEffect()
    {
        typingSchedule?.Pause();
        dialogueText.text = fullText;
        isTyping = false;
    }

}