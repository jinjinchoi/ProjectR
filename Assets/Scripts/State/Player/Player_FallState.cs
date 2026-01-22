using UnityEngine;

public class Player_FallState : PlayerBaseState
{
    public Player_FallState(BaseCharacter owner, PlayerAIController aiController, StateMachine stateMachine, string animStateName) : base(owner, aiController, stateMachine, animStateName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (owner.IsGrounded)
        {
            stateMachine.ChangeState(aiController.movementState);
        }
    }
}
