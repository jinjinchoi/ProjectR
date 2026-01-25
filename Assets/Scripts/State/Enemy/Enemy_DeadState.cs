using UnityEngine;

public class Enemy_DeadState : EnemyBaseState
{
    public Enemy_DeadState(EnemyAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }
}
