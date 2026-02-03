using UnityEngine;

public enum EDialogueType
{
    None,
    Text,
    Choice,
    Reward,
    End
}

public class DialogueNodeBase 
{
    public string nodeId;
    public EDialogueType dialogueType;
}
