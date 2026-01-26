using UnityEngine;

public class Enemy_CombatState : EnemyBaseState
{
    public Enemy_CombatState(EnemyAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        aiController.StopOwner();
    }

    public override void Update()
    {
        base.Update();

        if (!aiController.HasTarget)
        {
            stateMachine.ChangeState(aiController.idleState);
            return;
        }

        if (aiController.CanEnterAttackState())
        {
            stateMachine.ChangeState(aiController.attackState);
            return;
        }

        aiController.MovoToTarget();
    }
}
