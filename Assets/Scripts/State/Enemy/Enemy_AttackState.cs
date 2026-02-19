using UnityEngine;

public class Enemy_AttackState : EnemyBaseState
{
    public EAbilityId abilityId = EAbilityId.NormalAttack;

    public Enemy_AttackState(EnemyAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        aiController.StopOwner();
        aiController.TryActivateAbilityBy(abilityId);
    }

}
