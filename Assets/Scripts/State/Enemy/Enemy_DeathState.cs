using UnityEngine;

public class Enemy_DeathState : EnemyBaseState
{
    public Enemy_DeathState(EnemyAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (!aiController.Owner.IsDead)
            stateMachine.ChangeState(aiController.IdleState);
    }
}
