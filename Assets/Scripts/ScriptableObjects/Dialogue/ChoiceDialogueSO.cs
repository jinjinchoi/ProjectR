using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChoiceDialogueInfo
{
    public string groupId;
    public string dialogueId;
    [TextArea(3, 10)] public string choiceText;
    public string nextNodeId;
}

[CreateAssetMenu(fileName = "ChoiceDialogue", menuName = "Dialogue/Choice")]
public class ChoiceDialogueSO : ScriptableObject
{
    public List<ChoiceDialogueInfo> choiceDialogueInfo;
}
