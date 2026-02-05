using System;
using UnityEngine;

public static class EventHub
{
    public static event Action<string> DialogueRequested;
    public static void RaiseDialogueRequested(string dialgoueId)
    {
        DialogueRequested?.Invoke(dialgoueId);
    }

    public static event Action DialogueFinished;
    public static void RaiseDialogueFinished()
    {
        DialogueFinished?.Invoke();
    }

    public static event Action PlayerDied;
    public static void RaisePlayerDied()
    {
        PlayerDied?.Invoke();
    }
}
