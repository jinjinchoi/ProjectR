using UnityEngine;

public class DialogueManager
{
    private DialogueGraph dialogueGraph;

    public void Init()
    {
        dialogueGraph = new DialogueGraph();
        dialogueGraph.Init();
    }

    public EDialogueType GetDialogueTypeBy(string id)
    {
        var node = dialogueGraph.GetNodeById(id);
        if (node != null)
        {
            return node.dialogueType;
        }

        return EDialogueType.None;
    }

    public T GetDialogueNodeBy<T>(string id) where T : DialogueNodeBase
    {
        return dialogueGraph.GetNodeById(id) as T;
    }
}
