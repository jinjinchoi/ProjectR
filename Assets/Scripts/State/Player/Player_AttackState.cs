using UnityEngine;

public class Player_AttackState : PlayerCombatState
{
    public EAbilityId abilityId = EAbilityId.NormalAttack;

    public Player_AttackState(PlayerAIController aiController, StateMachine stateMachine, string animStateName, string animTriggerName) : base(aiController, stateMachine, animStateName, animTriggerName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        aiController.StopOwner();
        aiController.TryActivateAbilityBy(abilityId);
    }

}
