using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy_RetreatState : EnemyBaseState
{
    private float retreatDuration;
    private float accumulatedTime;

    // 벽 반대편 방향을 저장하는 변수로 최초로 벽에 도달할때 설정됨.
    private int wallOppositeDir;

    public Enemy_RetreatState(EnemyAIController aiController, StateMachine stateMachine, string animStateName) : base(aiController, stateMachine, animStateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        retreatDuration = Random.Range(1f, 3f);
        accumulatedTime = 0f;
        wallOppositeDir = 0;
    }

    public override void Update()
    {
        base.Update();

        if (!aiController.HasTarget)
        {
            stateMachine.ChangeState(aiController.IdleState);
            return;
        }

        accumulatedTime += Time.deltaTime;
        if (accumulatedTime > retreatDuration)
        {
            stateMachine.ChangeState(aiController.CombatState);
            return;
        }

        Retreat();

    }

    // 후퇴 중 벽 감지하면 벽 반대방향으로 이동시키는 함수
    private void Retreat()
    {
        // dir가 0이라는 건 벽에 도달한 적이 없다는 뜻.
        if (wallOppositeDir == 0 && aiController.IsWallDetected)
        {
            // 벽에 최초로 도달하면 벽 반대방향으로 위치 설정.
            wallOppositeDir = -aiController.Owner.FacingDir;
        }

        // 벽에 도달하였으면 벽 반대 방향으로 이동 아니면 계속 후퇴로직 발동.
        if (wallOppositeDir != 0)
            aiController.MoveInDirection(wallOppositeDir);
        else
            aiController.Retreat();
    }
}
