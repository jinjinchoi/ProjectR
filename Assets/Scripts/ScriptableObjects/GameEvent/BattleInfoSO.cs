using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class EnemySpawnInfo
{
    public string enemyId;
    public int level;
    public int count = 1;
}

[System.Serializable]
public class BattleEventInfo
{
    public string eventId;
    public string sceneName;
    public List<EnemySpawnInfo> enemieInfos;
}


[CreateAssetMenu(fileName = "BattleEvent", menuName = "GameEvent/Battle")]
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
        battleInfoMap.TryGetValue(id, out var data);
        return data;
    }
}
