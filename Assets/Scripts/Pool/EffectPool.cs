using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum EEffectType
{
    None,
    Healing,
    Thunder,
    HolySword
}

[System.Serializable]
public class EffectClipEntry
{
    public EEffectType type;
    public AnimationClip clip;
}

public class EffectPool : MonoBehaviour
{
    [SerializeField] GameObject effectPrefab;
    [SerializeField] List<EffectClipEntry> clipList;
    
    Dictionary<EEffectType, AnimationClip> clipMap;

    IObjectPool<GameObject> pool;

    private void Awake()
    {
        pool = new ObjectPool<GameObject>(Create, OnGet, OnRelease, OnDestroyEffect, collectionCheck: false, defaultCapacity: 6, maxSize: 50);

        clipMap = new Dictionary<EEffectType, AnimationClip>();
        foreach (var entry in clipList)
        {
            clipMap[entry.type] = entry.clip;
        }
    }

    GameObject Create()
    {
        GameObject go = Instantiate(effectPrefab);
        go.GetComponent<AnimatorPooledEffect>().Init(pool);
        return go;
    }

    void OnGet(GameObject go)
    {
        go.SetActive(true);
    }

    void OnRelease(GameObject go)
    {
        go.SetActive(false);
    }

    void OnDestroyEffect(GameObject go)
    {
        Destroy(go);
    }

    public void Play(EEffectType type, Vector3 pos)
    {
        if (!clipMap.ContainsKey(type)) return;

        GameObject effect = pool.Get();
        effect.transform.position = pos;
        effect.GetComponent<AnimatorPooledEffect>().Play(clipMap[type]);
    }

    public void ActivateAttackableEffect(EEffectType type, Vector3 pos, FAttackData attackData)
    {
        if (!clipMap.ContainsKey(type)) return;

        GameObject effect = pool.Get();
        effect.transform.position = pos;
        effect.GetComponent<ExplosionEffect>().SetDamageData(attackData);
        effect.GetComponent<AnimatorPooledEffect>().Play(clipMap[type]);
    }

  
}
