using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private NormalDialogueSO normalDialogueSO;
    [SerializeField] private ChoiceDialogueSO choiceDialogueSO;
    public NormalDialogueSO NormalDialogue => normalDialogueSO;
    public ChoiceDialogueSO ChoiceDialogue => choiceDialogueSO;

    private DialogueGraph dialogueGraph;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

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
