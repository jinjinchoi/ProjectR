using UnityEngine;
using System.Collections.Generic;

public struct ChoiceInfo
{
    public string dialogueId;
    public string text;
    public string nextNodeId;
}

public class DialogueChoiceNode : DialogueNodeBase
{
    public string groupId;
    public List<ChoiceInfo> choices = new();
}
