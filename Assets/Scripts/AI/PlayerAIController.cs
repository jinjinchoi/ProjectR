using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerAIController : AIController
{
    public Player_MovementState movementState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_AttackState attackState { get; private set; }

    [Header("Anim")]
    [SerializeField] private string movementStateName = "isMoving";
    [SerializeField] private string fallStateName = "isFalling";
    [SerializeField] private string attackStateName = "idle";

    [Header("Combat")]
    [SerializeField] private float attackRange = 1f;
    public float AttackRange => attackRange;

    protected override void Awake()
    {
        base.Awake();

        movementState = new Player_MovementState(owner, this, stateMachine, movementStateName);
        fallState = new Player_FallState(owner, this, stateMachine, fallStateName);

        attackState = new Player_AttackState(owner, this, stateMachine, attackStateName);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(movementState);
    }

    protected override void Update()
    {
        base.Update();
        //DecideAIBehavior();
    }

    // TODO: 값 리턴해서 state에서 쓰게 변경해야함. 아니면 switch off 사용.
    private void DecideAIBehavior()
    {
        
    }

    public bool CanAttackTarget(Transform target)
    {
        return Vector2.Distance(owner.transform.position, target.position) <= attackRange;
    }

    public int GetDirectionToTarget(Transform target)
    {
        if (target == null) return 0;

        float deltaX = target.position.x - owner.transform.position.x;

        if (Mathf.Abs(deltaX) < 0.1f)
            return 0;

        return deltaX > 0 ? 1 : -1;
    }

}