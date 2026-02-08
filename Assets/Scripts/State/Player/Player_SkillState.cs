using UnityEngine;

public class Player_SkillState : PlayerBaseState
{
    public Player_SkillState(PlayerAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        aiController.StopOwner();
        aiController.Owner.Anim.SetBool(animBoolName, false);
       
        Debug.Log($"Activate : {aiController.PendingAbilityId}");
        aiController.TryActivateAbilityBy(aiController.PendingAbilityId);
        aiController.PendingAbilityId = EAbilityId.None;

    }


}
