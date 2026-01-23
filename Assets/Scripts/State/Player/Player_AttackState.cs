using UnityEngine;

public class Player_AttackState : PlayerBaseState
{
    public EAbilityId abilityId = EAbilityId.Player_NormalAttack;

    public Player_AttackState(BaseCharacter owner, PlayerAIController aiController, StateMachine stateMachine, string animStateName) : base(owner, aiController, stateMachine, animStateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        owner.SetVelocity(0, owner.Rb.linearVelocity.y);
        owner.Anim.SetBool(animBoolName, false);

        aiController.TryActivateAbilityBy(abilityId);
    }

    public override void Exit()
    {
        base.Exit();

    }

}
