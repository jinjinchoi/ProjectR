using UnityEngine;

public class Enemy_IdleState : EnemyBaseState
{
    private float idleTime;
    private float accumulatedTime;
    
    public Enemy_IdleState(EnemyAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        idleTime = Random.Range(2f, 5f);
        accumulatedTime = 0f;
        aiController.StopOwner();
    }

    public override void Exit()
    {
        base.Exit();

        accumulatedTime = 0f;
    }

    public override void Update()
    {
        base.Update();

        if (aiController.HasTarget)
        {
            stateMachine.ChangeState(aiController.combatState);
        }

        accumulatedTime += Time.deltaTime;
        if (accumulatedTime > idleTime)
        {
            stateMachine.ChangeState(aiController.patrolState);
        }
    }
}
