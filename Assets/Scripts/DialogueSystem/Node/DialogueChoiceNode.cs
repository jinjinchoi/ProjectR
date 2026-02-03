using UnityEngine;
using System.Collections.Generic;

public struct FChoiceInfo
{
    public string dialogueId;
    public string text;
    public string nextNodeId;
}

public class DialogueChoiceNode : DialogueNodeBase
{
    public string groupId;
    public List<FChoiceInfo> choices = new();
}
