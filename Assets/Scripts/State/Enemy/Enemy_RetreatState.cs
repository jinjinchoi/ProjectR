using UnityEngine;

public class Enemy_RetreatState : EnemyBaseState
{
    private float retreatTime;
    private float accumulatedTime;

    public Enemy_RetreatState(EnemyAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        retreatTime = Random.Range(1f, 3f);
        accumulatedTime = 0f;
    }

    public override void Update()
    {
        base.Update();

        if (!aiController.HasTarget)
        {
            stateMachine.ChangeState(aiController.IdleState);
            return;
        }

        accumulatedTime += Time.deltaTime;
        if (accumulatedTime > retreatTime)
        {
            stateMachine.ChangeState(aiController.CombatState);
        }

        aiController.Retreat();

    }
}
