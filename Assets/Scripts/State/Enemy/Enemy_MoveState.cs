using UnityEngine;

public class Enemy_MoveState : EnemyBaseState
{
    public Enemy_MoveState(BaseCharacter owner, EnemyAIController aiController, StateMachine stateMachine, string animStateName) : base(owner, aiController, stateMachine, animStateName)
    {
    }
}
