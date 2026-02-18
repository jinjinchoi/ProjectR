using System.Collections;
using UnityEngine;

public class EnemyAIController : AIController
{
    public Enemy_IdleState IdleState { get; private set; }
    public Enemy_DeathState DeathState { get; private set; }
    public Enemy_PatrolState PatrolState { get; private set; }
    public Enemy_AttackState AttackState { get; private set; }
    public Enemy_CombatState CombatState { get; private set; }
    public Enemy_RetreatState RetreatState { get; private set; }
    public Enemy_SkillState SkillState { get; private set; }
    public bool HasTarget => target != null;

    [Header("Combat")]
    [SerializeField] private float detectDistance;
    [SerializeField] private float targetDetectTime = 3f;

    [Header("State")]
    [SerializeField] private string idleAnimName = "idle";
    [SerializeField] private string deathAnimName = "isDead";
    [SerializeField] private string patrolAnimName = "isMoving";
    [SerializeField] private string attackAnimName = "comboAttack";
    [SerializeField] private string skillTriggerName = "skillActive";

    private Coroutine targetLostTimer;

    protected override void Awake()
    {
        base.Awake();

        DeathState = new Enemy_DeathState(this, stateMachine, deathAnimName);
        IdleState = new Enemy_IdleState(this, stateMachine, idleAnimName);
        PatrolState = new Enemy_PatrolState(this, stateMachine, patrolAnimName);
        CombatState = new Enemy_CombatState(this, stateMachine, patrolAnimName);
        AttackState = new Enemy_AttackState(this, stateMachine, attackAnimName);
        RetreatState = new Enemy_RetreatState(this, stateMachine, patrolAnimName);
        SkillState = new Enemy_SkillState(this, stateMachine, patrolAnimName, skillTriggerName);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(IdleState);
    }

    protected override void Update()
    {
        base.Update();

        if (owner.IsDead)
            return;
        
        UpdateTargetDetection();
    }

    private void UpdateTargetDetection()
    {
        TryDetectPlayer();
        UpdateTargetLostTimer();
    }
    private void TryDetectPlayer()
    {
        if (target != null)
            return;

        // TODO: 벽 감지 구현해야할 수 있음
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, hostileDetectSize, 0f, Vector2.right * owner.FacingDir, detectDistance, hostileLayerMask);
        if (hit.collider)
        {
            target = hit.transform;
        }

    }

    private void UpdateTargetLostTimer()
    {
        if (target == null && targetLostTimer == null)
        {
            targetLostTimer = StartCoroutine(LoseTargetAfterDelay());
        }
        
        if (target != null && targetLostTimer != null)
        {
            StopCoroutine(targetLostTimer);
            targetLostTimer = null;
        }
    }

    private IEnumerator LoseTargetAfterDelay()
    {
        yield return new WaitForSeconds(targetDetectTime);

        target = null;
        targetLostTimer = null;
    }

    public void MoveEnemy(int dir)
    {
        owner.SetVelocity(owner.MoveSpeed * dir, owner.Rb.linearVelocity.y);
    }

    protected override void OnAbilityEnd(EAbilityId abilityId)
    {
        base.OnAbilityEnd(abilityId);

        if (PendingAbilityId != EAbilityId.None)
        {
            stateMachine.ChangeState(SkillState);
        }
        else if (ShouldRetreat())
        {
            stateMachine.ChangeState(RetreatState);
        }
        else if (HasTarget)
        {
            stateMachine.ChangeState(CombatState);
        }
        else
        {
            stateMachine.ChangeState(IdleState);
        }
    }

    private void OnHit(FDamageInfo damageInfo)
    {
        if (!target)
            target = damageInfo.Instigator.Transform;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (Owner is EnemyCharacter enemy)
        {
            enemy.OnHit += OnHit;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (Owner is EnemyCharacter enemy)
        {
            enemy.OnHit -= OnHit;
        }
    }
}
