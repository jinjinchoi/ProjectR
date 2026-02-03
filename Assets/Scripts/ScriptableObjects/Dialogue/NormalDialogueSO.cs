using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NormalDialogueInfo
{
    public string dialogueId;
    public string speakerName;
    [TextArea(3, 10)] public string text;
    public string nextDialogueId;
}

[CreateAssetMenu(fileName = "NormalDialogue", menuName = "Dialogue/Normal")]
public class NormalDialogueSO : ScriptableObject
{
    public List<NormalDialogueInfo> dialogueInfo;
}
