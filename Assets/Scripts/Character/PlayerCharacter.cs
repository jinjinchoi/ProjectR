using UnityEngine;

public class PlayerCharacter : BaseCharacter
{

    protected override void Awake()
    {
        base.Awake();

    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (!showDebug) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, hostileDetectSize);
    }
}
