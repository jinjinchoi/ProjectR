using System;
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
    private AbilitySystemComponent abilitySystemComponent;

    public void Init(AbilitySystemComponent asc)
    {
        EventHub.DialogueRequested += OnDialogueRequested;
        abilitySystemComponent = asc;
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

            case EDialogueType.Reward:
                HandleRewardNode();
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

    private void HandleRewardNode()
    {
        DialogueRewardNode node = GameManager.Instance.DialogueManager.GetDialogueNodeBy<DialogueRewardNode>(nodeId);
        if (node == null) return;

        FAttributeModifier modifier = new()
        {
            attributeType = node.attribute,
            value = node.reward,
            operation = EModifierOp.Add,
            isPermanent = true
        };

        abilitySystemComponent.ApplyModifier(modifier);

        string attributeName = FunctionLibrary.GetAttributeNameByType(node.attribute);

        string msg = node.reward switch
        {
            > 0 => $"{attributeName}이(가) {node.reward} 증가하였습니다",
            < 0 => $"{attributeName}이(가) {Mathf.Abs(node.reward)} 감소하였습니다",
            _ => $"{attributeName}은(는) 변화가 없습니다"
        };

        FNormalDialogueInfo dialogueInfo = new()
        {
            speakerName = string.Empty,
            text = msg
        };

        TextDialogueRequested?.Invoke(dialogueInfo);
        nodeId = node.nextNodeId;
    }

}
