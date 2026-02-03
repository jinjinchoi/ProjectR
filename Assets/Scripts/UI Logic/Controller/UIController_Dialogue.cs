using System;
using Unity.VisualScripting;
using UnityEngine;

public struct FNormalDialogueInfo
{
    public string speakerName;
    public string text;
}

public class UIController_Dialogue
{
    public event Action<FNormalDialogueInfo> TextDialogueRequested;

    private string nodeId;

    public void Init()
    {
        EventHub.DialogueRequested += OnDialogueRequested;
    }

    public void Dispose()
    {
        EventHub.DialogueRequested -= OnDialogueRequested;
    }

    public void CallCurrentDialgoue()
    {
        OnDialogueRequested(nodeId);
    }

    private void OnDialogueRequested(string dialogueId)
    {
        nodeId = dialogueId;

        switch (GameManager.Instance.DialogueManager.GetDialogueTypeBy(nodeId))
        {
            case EDialogueType.None:
                Debug.LogError("invalid dialogue id received");
                break;

            case EDialogueType.Text:
                HandleNormalDialogue();
                break;

            case EDialogueType.Choice:
                break;

            case EDialogueType.End:
                EventHub.RaiseDialogueFinished();
                break;
        }
    }

    private void HandleNormalDialogue()
    {
        DialogueTextNode node = GameManager.Instance.DialogueManager.GetDialogueNodeBy<DialogueTextNode>(nodeId);
        if (node == null) return;

        FNormalDialogueInfo dialogueInfo = new()
        {
            speakerName = node.speakerName,
            text = node.text
        };

        TextDialogueRequested?.Invoke(dialogueInfo);
        nodeId = node.nextNodeId;
    }
}
