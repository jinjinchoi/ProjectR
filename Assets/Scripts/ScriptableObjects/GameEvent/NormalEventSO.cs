using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class NormalEventInfo
{
    public string eventId;
    public string dialogueId;
    public bool isOnce = true;
    // ...reward
}

[CreateAssetMenu(fileName = "Scenario", menuName = "GameEvent/Normal")]
public class NormalEventSO : ScriptableObject
{
    public List<NormalEventInfo> NormalEvent;

}
