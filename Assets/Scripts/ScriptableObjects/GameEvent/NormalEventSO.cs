using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class NormalEventInfo
{
    public string eventId;
    public string dialogueId;
    public bool isOnce = true;
}

[CreateAssetMenu(fileName = "NormalEvent", menuName = "GameEvent/Normal")]
public class NormalEventSO : ScriptableObject
{
    public List<NormalEventInfo> NormalEvent;

}
