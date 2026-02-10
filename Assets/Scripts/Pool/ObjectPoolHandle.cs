using UnityEngine;
using UnityEngine.Pool;

public interface IPoolableObject
{
    void SetObjectPool(IObjectPool<GameObject> pool);
}

public class ObjectPoolHandle
{
    public IObjectPool<GameObject> Pool { get; private set; }
    private readonly ObjectPoolConfig poolConfig;

    public ObjectPoolHandle(ObjectPoolConfig config)
    {
        poolConfig = config;

        Pool = new ObjectPool<GameObject>
            (Create, OnGet, OnRelease, OnDestroy, collectionCheck: false, defaultCapacity: poolConfig.defaultCapacity, maxSize: poolConfig.maxSize);
    }

    private GameObject Create()
    {
        GameObject go = GameObject.Instantiate(poolConfig.prefab);
        go.GetComponent<IPoolableObject>().SetObjectPool(Pool);
        return go;
    }

    private void OnGet(GameObject go)
    {
        go.SetActive(true);
    }

    private void OnRelease(GameObject go)
    {
        go.SetActive(false);
    }

    private void OnDestroy(GameObject go)
    {
        GameObject.Destroy(go);
    }

}


