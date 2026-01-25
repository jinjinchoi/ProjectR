using UnityEngine;

public class EnemyAIController : AIController
{
    public Enemy_IdleState idleState { get; private set; }
    public Enemy_DeadState deadState { get; private set; }
    public Enemy_PatrolState patrolState { get; private set; }

    [Header("Combat")]
    [SerializeField] private float detectDistance;

    [Header("State")]
    [SerializeField] private string idleAnimName = "idle";
    [SerializeField] private string deadAnimName = "isDead";
    [SerializeField] private string patrolStateName = "isMoving";

    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, idleAnimName);
        deadState = new Enemy_DeadState(this, stateMachine, deadAnimName);
        patrolState = new Enemy_PatrolState(this, stateMachine, patrolStateName);
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
        else if (stateMachine.CurrentState == deadState)
        {
            stateMachine.ChangeState(idleState);
        }
  
    }

    public bool IsPlayerDetected()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, hostileDetectSize, 0f, Vector2.right * owner.FacingDir, detectDistance, hostileLayerMask);
        // TODO: 벽감지도 구현해야할 수 있음.
        return hit.collider != null;
    }

    public void MoveEnemy(int dir)
    {
        owner.SetVelocity(owner.MoveSpeed * dir, owner.Rb.linearVelocity.y);
    }
}
