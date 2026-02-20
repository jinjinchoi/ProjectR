public class Player_MovementState : PlayerBaseState
{

    public Player_MovementState(PlayerAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (!aiController.Owner.IsGrounded)
        {
            stateMachine.ChangeState(aiController.FallState);
            return;
        }

        if (aiController.CanEnterSkillState(aiController.PendingAbilityId))
        {
            stateMachine.ChangeState(aiController.SkillState);
            return;
        }

        if (aiController.CanEnterAttackState())
        {
            stateMachine.ChangeState(aiController.AttackState);
            return;
        }

        aiController.MovoToTarget();

    }

    public override void Exit()
    {
        base.Exit();

    }

}
