using UnityEngine;

public class Player_AttackState : PlayerBaseState
{
    public EAbilityId abilityId = EAbilityId.Common_NormalAttack;

    public Player_AttackState(PlayerAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        aiController.StopOwner();
        aiController.Owner.Anim.SetBool(animBoolName, false);

        aiController.TryActivateAbilityBy(abilityId);
    }

    public override void Exit()
    {
        base.Exit();

    }

}
