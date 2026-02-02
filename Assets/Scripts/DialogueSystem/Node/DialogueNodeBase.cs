using UnityEngine;

public enum EDialogueType
{
    None,
    Text,
    Choice,
    End
}

public class DialogueNodeBase 
{
    public string nodeId;
    public EDialogueType dialogueType;
    
}
