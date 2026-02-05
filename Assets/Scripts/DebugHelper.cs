using UnityEngine;

public static class DebugHelper
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(string message)
    {
        Debug.Log(message);
    }
}
