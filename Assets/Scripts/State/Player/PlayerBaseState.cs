public class PlayerBaseState : BaseState<PlayerAIController>
{
    public PlayerBaseState(BaseCharacter owner, PlayerAIController aiController, StateMachine stateMachine, string animStateName) : base(owner, aiController, stateMachine, animStateName)
    {
    }
}
