using System.Collections.Generic;
using UnityEngine;

public enum ScenarioEventType
{
    Dialogue,
    Battle
}

[System.Serializable]
public class ScenarioEventInfo
{
    public int day;
    public ScenarioEventType type;
    public string eventId;
    public string dialogueId;
    public string battleInfoId;
    //reward
}

[CreateAssetMenu(fileName = "Scenario", menuName = "GameEvent/Scenario")]
public class ScenarioEventSO : ScriptableObject
{
    public List<ScenarioEventInfo> scenarioEvent;
    public int lastDay;
}
