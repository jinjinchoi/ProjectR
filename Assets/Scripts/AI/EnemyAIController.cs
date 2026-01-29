using System.Collections;
using UnityEngine;

public class EnemyAIController : AIController
{
    public Enemy_IdleState idleState { get; private set; }
    public Enemy_DeadState deadState { get; private set; }
    public Enemy_PatrolState patrolState { get; private set; }
    public Enemy_AttackState attackState { get; private set; }
    public Enemy_CombatState combatState { get; private set; }
    public bool HasTarget => target != null;

    [Header("Combat")]
    [SerializeField] private float detectDistance;
    [SerializeField] private float targetDetectTime = 3f;

    [Header("State")]
    [SerializeField] private string idleAnimName = "idle";
    [SerializeField] private string deadAnimName = "isDead";
    [SerializeField] private string patrolAnimName = "isMoving";
    [SerializeField] private string attackAnimName = "comboAttack";

    private Coroutine targetLostTimer;

    protected override void Awake()
    {
        base.Awake();

        deadState = new Enemy_DeadState(this, stateMachine, deadAnimName);
        idleState = new Enemy_IdleState(this, stateMachine, idleAnimName);
        patrolState = new Enemy_PatrolState(this, stateMachine, patrolAnimName);
        combatState = new Enemy_CombatState(this, stateMachine, patrolAnimName);
        attackState = new Enemy_AttackState(this, stateMachine, attackAnimName);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        if (owner.IsDead)
        {
            stateMachine.ChangeState(deadState);
            return;
        }

        if (stateMachine.CurrentState == deadState)
        {
            stateMachine.ChangeState(idleState);
        }
      
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

        if (HasTarget)
            stateMachine.ChangeState(combatState);
        else
            stateMachine.ChangeState(idleState);
    }

    private void OnHit(FDamageInfo damageInfo)
    {
        if (!target)
            target = damageInfo.Instigator.OwnerTransform;
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
