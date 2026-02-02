using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueGraph
{
    private Dictionary<string /* node id */, DialogueNodeBase> nodeMap;

    public void Init()
    {
        CreateTextNode();
        CreateChoiceNode();
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
        if (DialogueManager.Instance.NormalDialogue == null)
        {
            Debug.LogWarning("NormalDialogue Not Set");
            return;
        }

        List<NormalDialogueInfo> dialogueInfo = DialogueManager.Instance.NormalDialogue.dialogueInfo;
        foreach (var info in dialogueInfo)
        {
            DialogueTextNode textNode = new()
            {
                dialogueType = EDialogueType.Text,
                nodeId = info.dialogueId,
                nextNodeName = info.nextDialogueId,
                text = info.text,
                speakerId = info.speakerName
            };

            nodeMap.Add(info.dialogueId, textNode);
        }
    }

    private void CreateChoiceNode()
    {
        if (DialogueManager.Instance.ChoiceDialogue == null)
        {
            Debug.LogWarning("ChoiceDialogue Not Set");
            return;
        }

        List<ChoiceDialogueInfo> choicesInfo = DialogueManager.Instance.ChoiceDialogue.choiceDialogueInfo;
        foreach (var info in choicesInfo)
        {
            ChoiceInfo choice = new()
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
