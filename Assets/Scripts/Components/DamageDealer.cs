using UnityEngine;

public interface IHitHandler
{
    void OnHit(IDamageable target, Collider2D hitCollider);
}

public class DamageDealer : MonoBehaviour
{
    private IHitHandler hitHandler;

    [SerializeField] private bool canDebug = false;

    public void Init(IHitHandler handler)
    {
        hitHandler = handler;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out var target))
        {
            if (canDebug) DebugHelper.Log($"[AttackHitBox] IDamageable detected: {other.name}");
            hitHandler?.OnHit(target, other);
        }
        else
        {
            if (canDebug) DebugHelper.Log($"[AttackHitBox] is not IDamageable: {other.name}");
        }
    }
}
