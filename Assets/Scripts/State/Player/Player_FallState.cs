using UnityEngine;

public class Player_FallState : PlayerBaseState
{
    public Player_FallState(PlayerAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (aiController.Owner.IsGrounded)
        {
            stateMachine.ChangeState(aiController.movementState);
        }
    }
}
