using UnityEngine;

public class PlayerCombatState : PlayerBaseState
{
    public string animTriggerName;

    public PlayerCombatState(PlayerAIController aiController, StateMachine stateMachine, string animStateName, string animTriggerName) : base(aiController, stateMachine, animStateName)
    {
        this.animTriggerName = animTriggerName;
    }

    public override void Enter()
    {
        base.Enter();

        aiController.Owner.Anim.SetBool(animBoolName, false);
        aiController.Owner.Anim.SetTrigger(animTriggerName);
    }
}
