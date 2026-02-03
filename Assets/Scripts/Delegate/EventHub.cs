using System;
using UnityEngine;

public static class EventHub
{
    public static event Action<string> DialogueRequested;
    public static event Action DialogueFinished;

    public static void RaiseDialogueRequested(string dialgoueId)
    {
        DialogueRequested?.Invoke(dialgoueId);
    }

    public static void RaiseDialogueFinished()
    {
        DialogueFinished?.Invoke();
    }
}
