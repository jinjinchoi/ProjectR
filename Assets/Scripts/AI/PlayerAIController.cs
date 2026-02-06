using UnityEngine;


public class PlayerAIController : AIController
{
    public Player_MovementState movementState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_AttackState attackState { get; private set; }
    public Player_DeathState deathState { get; private set; }

    [Header("Anim")]
    [SerializeField] private string movementAnimName = "isMoving";
    [SerializeField] private string fallAnimName = "isFalling";
    [SerializeField] private string attackAnimName = "idle";
    [SerializeField] private string deathAnimName = "isDead";

    protected override void Awake()
    {
        base.Awake();

        movementState = new Player_MovementState(this, stateMachine, movementAnimName);
        fallState = new Player_FallState(this, stateMachine, fallAnimName);
        attackState = new Player_AttackState(this, stateMachine, attackAnimName);
        deathState = new Player_DeathState(this, stateMachine, deathAnimName);

    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(movementState);
    }


    protected override void Update()
    {
        base.Update();

        if (owner.IsDead)
            return;

        if (target == null)
            target = FindClosestTargetWithinBox();
    }

    public Transform FindClosestTargetWithinBox()
    {
        Collider2D[] detectedEnemy = Physics2D.OverlapBoxAll(transform.position, hostileDetectSize, 0f, hostileLayerMask);

        if (detectedEnemy.Length == 0)
        {
            //Debug.Log("detected Enemy is 0, FindClosestTarget");
            return null;
        }

        Transform closestTarget = null;
        float minSqrDistance = float.MaxValue;

        Vector2 myPos = transform.position;

        foreach (Collider2D enemy in detectedEnemy)
        {

            Vector2 enemyPos = enemy.transform.position;
            float sqrDist = (enemyPos - myPos).sqrMagnitude;

            if (sqrDist < minSqrDistance)
            {
                minSqrDistance = sqrDist;
                closestTarget = enemy.transform;
            }
        }

        return closestTarget;
    }

    protected override void OnAbilityEnd(EAbilityId abilityId)
    {
        base.OnAbilityEnd(abilityId);

        target = FindClosestTargetWithinBox();

        if (CanEnterAttackState())
        {
            TryActivateAbilityBy(abilityId);
        }
        else
        {
            stateMachine.ChangeState(movementState);
        }
    }
}