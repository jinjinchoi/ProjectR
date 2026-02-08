using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }
    private EffectPool effectPool;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        effectPool = GetComponent<EffectPool>();
    }

    public void ActivateEffect(EEffectType effectType, Vector3 pos)
    {
        effectPool.Play(effectType, pos);
    }
}
