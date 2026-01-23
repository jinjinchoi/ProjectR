using UnityEngine;

public class Enemy_IdleState : EnemyBaseState
{
    public Enemy_IdleState(BaseCharacter owner, EnemyAIController aiController, StateMachine stateMachine, string animStateName) : base(owner, aiController, stateMachine, animStateName)
    {
    }
}
