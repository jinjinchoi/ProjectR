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
        owner.anim.SetBool(animBoolName, false);

        owner.abilitySystemComponent.OnAbilityEnded += OnAbilityEnd;
        owner.abilitySystemComponent.TryActivateAbilityById(abilityId);
    }

    public override void Exit()
    {
        base.Exit();
        owner.abilitySystemComponent.OnAbilityEnded -= OnAbilityEnd;

    }

    private void OnAbilityEnd()
    {
        Transform target = owner.FindClosestTargetWithinBox();
        if (aiController.CanAttackTarget(target))
        {
            int dir = aiController.GetDirectionToTarget(target);
            owner.HandleFlip(dir);
            owner.abilitySystemComponent.TryActivateAbilityById(abilityId);
        }
        else
        {
            stateMachine.ChangeState(aiController.movementState);
        }

    }

}
