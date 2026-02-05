using UnityEngine;

public class EnemyBaseState : BaseState<EnemyAIController>
{
    public EnemyBaseState(EnemyAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (aiController.Owner.IsDead && stateMachine.CurrentState != aiController.deathState)
            stateMachine.ChangeState(aiController.deathState);
    }

}
