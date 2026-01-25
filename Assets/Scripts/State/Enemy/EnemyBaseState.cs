using UnityEngine;

public class EnemyBaseState : BaseState<EnemyAIController>
{
    public EnemyBaseState(EnemyAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }
}
