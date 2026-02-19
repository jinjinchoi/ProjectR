using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject Prefab;
    public int Level = 1;
    public int Count = 1;
}

[System.Serializable]
public class BattleEventInfo
{
    public string eventId;
    public string sceneName;
    public List<EnemySpawnInfo> enemieInfos;
}


[CreateAssetMenu(fileName = "BattleInformation", menuName = "GameEvent/Battle")]
public class BattleInfoSO : ScriptableObject
{
    public List<BattleEventInfo> battleInfoList;

    private Dictionary<string, BattleEventInfo> battleInfoMap;

    public void Init()
    {
        battleInfoMap = battleInfoList.ToDictionary(e => e.eventId);
    }

    public BattleEventInfo GetBattleInfo(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            DebugHelper.LogWarning("GetBattleInfo called with null or empty id");
            return null;
        }

        battleInfoMap.TryGetValue(id, out var data);
        return data;
    }
}
