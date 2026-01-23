using UnityEngine;

public class Enemy_DeadState : EnemyBaseState
{
    public Enemy_DeadState(BaseCharacter owner, EnemyAIController aiController, StateMachine stateMachine, string animStateName) : base(owner, aiController, stateMachine, animStateName)
    {
    }
}
