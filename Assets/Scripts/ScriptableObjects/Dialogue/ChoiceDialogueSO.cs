using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChoiceDialogueInfo
{
    public string groupId;
    public string dialogueId;
    public string choiceText;
    public string nextNodeId;
}

public class ChoiceDialogueSO : ScriptableObject
{
    public List<ChoiceDialogueInfo> choiceDialogueInfo;
}
