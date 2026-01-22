using UnityEngine;

public class Player_MovementState : PlayerBaseState
{
    Transform target;

    public Player_MovementState(BaseCharacter owner, PlayerAIController aiController, StateMachine stateMachine, string animStateName) : base(owner, aiController, stateMachine, animStateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        target = owner.FindClosestTargetWithinBox();
    }

    public override void Update()
    {
        base.Update();

        if (!owner.isGrounded)
        {
            stateMachine.ChangeState(aiController.fallState);
        }

        if (target == null) return;

        if (aiController.CanAttackTarget(target))
        {
            stateMachine.ChangeState(aiController.attackState);
        }
        else
        {
            owner.SetVelocity(owner.MoveSpeed * aiController.GetDirectionToTarget(target), owner.Rb.linearVelocity.y);
        }
    }

    public override void Exit()
    {
        base.Exit();

        target = null;
    }
}
