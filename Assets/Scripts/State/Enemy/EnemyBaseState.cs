using UnityEngine;

public class EnemyBaseState : BaseState<EnemyAIController>
{
    public EnemyBaseState(BaseCharacter owner, EnemyAIController aiController, StateMachine stateMachine, string animStateName) : base(owner, aiController, stateMachine, animStateName)
    {
    }
}
