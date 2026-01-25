public class PlayerBaseState : BaseState<PlayerAIController>
{
    public PlayerBaseState(PlayerAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }
}
