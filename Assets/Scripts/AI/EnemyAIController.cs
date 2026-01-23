using Unity.IO.LowLevel.Unsafe;

public class EnemyAIController : AIController
{
    public Enemy_IdleState idleState { get; private set; }
    public Enemy_DeadState deadState { get; private set; }

    protected override void Awake()
    {
        base.Awake();



        idleState = new Enemy_IdleState(owner, this, stateMachine, "idle");
        deadState = new Enemy_DeadState(owner, this, stateMachine, "isDead");
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
        else
        {
            stateMachine.ChangeState(idleState);
        }
    }
}
