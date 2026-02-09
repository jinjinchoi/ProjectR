using UnityEngine;

public class Player_SkillState : PlayerCombatState
{
    public Player_SkillState(PlayerAIController aiController, StateMachine stateMachine, string animStateName, string animTriggerName) : base(aiController, stateMachine, animStateName, animTriggerName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        aiController.StopOwner();
       
        Debug.Log($"Activate : {aiController.PendingAbilityId}");
        aiController.TryActivateAbilityBy(aiController.PendingAbilityId);
        aiController.PendingAbilityId = EAbilityId.None;

    }

}
