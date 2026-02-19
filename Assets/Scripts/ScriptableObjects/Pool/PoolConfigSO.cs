
using System;
using System.Collections.Generic;
using UnityEngine;

public enum EObjectId
{
    None,
    SwordAura,
    Healing,
    Thunder,
    IceBall
}

[Serializable]
public class ObjectPoolConfig
{
    public EObjectId objectId;
    public GameObject prefab;
    public int defaultCapacity = 5;
    public int maxSize = 50;
}

[CreateAssetMenu(menuName = "Pooling/PoolConfig")]
public class PoolConfigSO : ScriptableObject
{
    public List<ObjectPoolConfig> PoolInfoList;
}
