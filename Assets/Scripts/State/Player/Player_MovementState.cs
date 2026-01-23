using UnityEngine;

public class Player_MovementState : PlayerBaseState
{

    public Player_MovementState(BaseCharacter owner, PlayerAIController aiController, StateMachine stateMachine, string animStateName) : base(owner, aiController, stateMachine, animStateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Update()
    {
        base.Update();

        if (!owner.IsGrounded)
            stateMachine.ChangeState(aiController.fallState);
        

        if (aiController.CanEnterAttackState())
        {
            stateMachine.ChangeState(aiController.attackState);
        }
        else
        {
           aiController.MovoToTarget();
        }
    }

    public override void Exit()
    {
        base.Exit();

    }
}
