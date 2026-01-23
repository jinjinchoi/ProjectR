using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerAIController : AIController
{
    public Player_MovementState movementState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_AttackState attackState { get; private set; }

    [Header("Anim")]
    [SerializeField] private string movementStateName = "isMoving";
    [SerializeField] private string fallStateName = "isFalling";
    [SerializeField] private string attackStateName = "idle";

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
    }

    protected override void OnAbilityEnd(EAbilityId abilityId)
    {
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