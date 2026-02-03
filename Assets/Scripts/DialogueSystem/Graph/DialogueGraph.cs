using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueGraph
{
    private Dictionary<string /* node id */, DialogueNodeBase> nodeMap = new();

    public void Init()
    {
        CreateTextNode();
        CreateChoiceNode();
        CreateRewardNode();
        CreateEndNode();
    }


    public DialogueNodeBase GetNodeById(string nodeId)
    {
        if (nodeMap.TryGetValue(nodeId, out DialogueNodeBase node))
        {
            return node;
        }

        return null;
    }

    private void CreateTextNode()
    {
        if (GameManager.Instance.NormalDialogueSO == null)
        {
            Debug.LogWarning("NormalDialogue Not Set");
            return;
        }

        List<NormalDialogueInfo> dialogueInfo = GameManager.Instance.NormalDialogueSO.dialogueInfo;
        foreach (var info in dialogueInfo)
        {
            DialogueTextNode textNode = new()
            {
                dialogueType = EDialogueType.Text,
                nodeId = info.dialogueId,
                nextNodeId = info.nextDialogueId,
                text = info.text,
                speakerName = info.speakerName
            };

            nodeMap.Add(info.dialogueId, textNode);
        }
    }

    private void CreateChoiceNode()
    {
        if (GameManager.Instance.ChoiceDialogueSO == null)
        {
            Debug.LogWarning("ChoiceDialogue Not Set");
            return;
        }

        List<ChoiceDialogueInfo> choicesInfo = GameManager.Instance.ChoiceDialogueSO.choiceDialogueInfo;
        foreach (var info in choicesInfo)
        {
            FChoiceInfo choice = new()
            {
                dialogueId = info.dialogueId,
                text = info.choiceText,
                nextNodeId = info.nextNodeId,
            };

            if (!nodeMap.TryGetValue(info.groupId, out DialogueNodeBase node))
            {
                node = new DialogueChoiceNode()
                {
                    dialogueType = EDialogueType.Choice,
                    groupId = info.groupId,
                    nodeId = info.dialogueId
                };

                nodeMap.Add(info.groupId, node);
            }

            if (node is DialogueChoiceNode choiceNode)
            {
                choiceNode.choices.Add(choice);
            }
        }
    }

    private void CreateRewardNode()
    {
        if (GameManager.Instance.RewardDialogueSO == null)
        {
            Debug.LogWarning("RewardDialogueSO Not Set");
            return;
        }

        List<RewardNodeInfo> rewardNodeInfo = GameManager.Instance.RewardDialogueSO.rewardNodeInfo;
        foreach (RewardNodeInfo info in rewardNodeInfo)
        {
            DialogueRewardNode node = new()
            {
                nodeId = info.nodeId,
                dialogueType = EDialogueType.Reward,
                attribute = info.attribute,
                reward = info.reward,
                nextNodeId = info.nextNodeId
            };

            nodeMap.Add(info.nodeId, node);
        }
    }

    private void CreateEndNode()
    {
        DialogueNodeBase endNode = new()
        {
            dialogueType = EDialogueType.End,
            nodeId = "End"
        };

        nodeMap.Add(endNode.nodeId, endNode);
    }


}
