using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NormalDialogueInfo
{
    public string dialogueId;
    public string speakerName;
    public string text;
    public string nextDialogueId;
}

public class NormalDialogueSO : ScriptableObject
{
    public List<NormalDialogueInfo> dialogueInfo;
}
