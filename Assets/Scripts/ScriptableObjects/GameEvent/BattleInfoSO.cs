using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BattleEventInfo
{
    string eventId;
    // TODO: 에너미 레벨에 따른 능력치 설정 방법 생각해야함.
    // 배열을 사용해서 설정하는 방식이 유효할 것으로 생각됨.
    Dictionary<string /* enemy id */, int /* enemy level */> enemyInfo;
}

public class BattleInfoSO : ScriptableObject
{
    public List<BattleEventInfo> battleInfo;
}
