
using System;
using System.Collections.Generic;
using UnityEngine;

public struct FNormalDialogueInfo
{
    public string speakerName;
    public string text;
}

public struct FChoiceButtonInfo
{
    public string choiceText;
    public string nextNodeId;
}

public class UIController_Dialogue
{
    public event Action<FNormalDialogueInfo> TextDialogueRequested;
    public event Action<List<FChoiceButtonInfo>> ChoiceDialogueRequested;

    private string nodeId;
    private IAbilitySystemContext abilitySystemComponent;

    public void Init(IAbilitySystemContext asc)
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

    public void CallDialogueById(string dialoguId)
    {
        OnDialogueRequested(dialoguId);
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
                HandleChoiceDialogue();
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

    private void HandleChoiceDialogue()
    {
        DialogueChoiceNode node = GameManager.Instance.DialogueManager.GetDialogueNodeBy<DialogueChoiceNode>(nodeId);
        if (node == null) return;

        List<FChoiceButtonInfo> choiceButtonInfo = new();

        foreach (var choice in node.choices)
        {
            FChoiceButtonInfo choiceInfo = new()
            {
                choiceText = choice.text,
                nextNodeId = choice.nextNodeId,
            };

            choiceButtonInfo.Add(choiceInfo);
        }

        ChoiceDialogueRequested?.Invoke(choiceButtonInfo);
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
            policy = EModifierPolicy.Instant
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
