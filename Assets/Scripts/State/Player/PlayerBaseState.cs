public class PlayerBaseState : BaseState<PlayerAIController>
{
    public PlayerBaseState(PlayerAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (aiController.Owner.IsDead && stateMachine.CurrentState != aiController.DeathState)
            stateMachine.ChangeState(aiController.DeathState);
    }
}
