using UnityEngine;
using UnityEngine.Rendering;


public class PlayerAIController : AIController
{
    public Player_MovementState MovementState { get; private set; }
    public Player_FallState FallState { get; private set; }
    public Player_DeathState DeathState { get; private set; }
    public Player_AttackState AttackState { get; private set; }
    public Player_SkillState SkillState { get; private set; }

    [Header("Anim")]
    [SerializeField] private string movementAnimName = "isMoving";
    [SerializeField] private string fallAnimName = "isFalling";
    [SerializeField] private string deathAnimName = "isDead";
    [SerializeField] private string attackTriggerName = "comboActive";
    [SerializeField] private string skillTriggerName = "skillActive";

    protected override void Awake()
    {
        base.Awake();

        MovementState = new Player_MovementState(this, stateMachine, movementAnimName);
        FallState = new Player_FallState(this, stateMachine, fallAnimName);
        DeathState = new Player_DeathState(this, stateMachine, deathAnimName);
        AttackState = new Player_AttackState(this, stateMachine, movementAnimName, attackTriggerName);
        SkillState = new Player_SkillState(this, stateMachine, movementAnimName, skillTriggerName);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(MovementState);
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

        if (owner.IsDead && stateMachine.CurrentState != DeathState)
        {
            stateMachine.ChangeState(DeathState);
            return;
        }

        target = FindClosestTargetWithinBox();

        if (PendingAbilityId != EAbilityId.None)
        {
            stateMachine.ChangeState(SkillState);
        }
        else if (CanEnterAttackState())
        {
            stateMachine.ChangeState(AttackState);
        }
        else
        {
            stateMachine.ChangeState(MovementState);
        }
    }

    public EAbilityId GetRandomAbilityId()
    {
        return owner.ASC.GetRandomAbilityId();
    }
}