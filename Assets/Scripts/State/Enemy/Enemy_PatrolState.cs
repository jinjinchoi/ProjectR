using UnityEngine;

public class Enemy_PatrolState : EnemyBaseState
{
    private int patrolDirection;
    private float patrolTime;
    private float accumulatedTime;

    public Enemy_PatrolState(EnemyAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        patrolDirection = Random.value > 0.5f ? 1 : -1;
        patrolTime = Random.Range(2f, 4f);
        accumulatedTime = 0f;
    }

    public override void Exit()
    {
        base.Exit();
        accumulatedTime = 0f;
    }

    public override void Update()
    {
        base.Update();

        aiController.MoveEnemy(patrolDirection);

        accumulatedTime += Time.deltaTime;
        if (accumulatedTime > patrolTime)
        {
            stateMachine.ChangeState(aiController.idleState);
        }
    }

}
