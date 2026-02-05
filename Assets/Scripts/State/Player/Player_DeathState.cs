using UnityEngine;

public class Player_DeathState : PlayerBaseState
{
    public Player_DeathState(PlayerAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (!aiController.Owner.IsDead)
            stateMachine.ChangeState(aiController.movementState);
    }
}
