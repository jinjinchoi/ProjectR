using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager Instance { get; private set; }
    private EffectPool effectPool;

    private Dictionary<EObjectId, ObjectPoolHandle> poolHandles = new();

    [SerializeField] private PoolConfigSO poolConfigs;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        effectPool = GetComponent<EffectPool>();
        CreateObjectPool();
    }

    public void ActivateEffect(EEffectType effectType, Vector3 pos)
    {
        effectPool.Play(effectType, pos);
    }

    public void ActivateAttackableEffect(EEffectType type, Vector3 pos, FAttackData attackData)
    {
        effectPool.ActivateAttackableEffect(type, pos, attackData);
    }

    public GameObject GetPooledObject(EObjectId id)
    {
        poolHandles.TryGetValue(id, out var poolHandle);
        return poolHandle.Pool.Get();
    }

    private void CreateObjectPool()
    {
        if (poolConfigs == null)
        {
            DebugHelper.LogWarning("poolConfigs is null");
            return;
        }

        foreach (var config in poolConfigs.PoolInfoList)
        {
            if (poolHandles.ContainsKey(config.objectId)) continue;

            ObjectPoolHandle pool = new(config);
            poolHandles.Add(config.objectId, pool);
        }
    }
}
