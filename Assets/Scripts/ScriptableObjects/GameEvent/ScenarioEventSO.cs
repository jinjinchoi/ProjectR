using System.Collections.Generic;
using UnityEngine;

public enum EScenarioEventType
{
    Dialogue,
    Battle
}

[System.Serializable]
public class ScenarioEventInfo
{
    public int day;
    public EScenarioEventType type;
    public string eventId;
    public string dialogueId;
    public string battleInfoId;
    //reward
}

[CreateAssetMenu(fileName = "ScenarioEvent", menuName = "GameEvent/Scenario")]
public class ScenarioEventSO : ScriptableObject
{
    public List<ScenarioEventInfo> scenarioEvent;
    public int lastDay = 30;
}
