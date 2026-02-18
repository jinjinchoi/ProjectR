using UnityEngine;

public class Enemy_SkillState : EnemyBaseState
{
    public string animTriggerName;

    public Enemy_SkillState(EnemyAIController aiController, StateMachine stateMachine, string animStateName, string animTriggerName) : base(aiController, stateMachine, animStateName)
    {
        this.animTriggerName = animTriggerName;
    }

    public override void Enter()
    {
        base.Enter();

        aiController.StopOwner();

        aiController.Owner.Anim.SetBool(animBoolName, false);
        aiController.Owner.Anim.SetTrigger(animTriggerName);

        Debug.Log($"{aiController.Owner.name} : {aiController.PendingAbilityId}");
        aiController.TryActivateAbilityBy(aiController.PendingAbilityId);
        aiController.PendingAbilityId = EAbilityId.None;
    }
}
