using UnityEngine;

public class StateMachine
{
    private IState currentState;
    public bool canChangeState = true;

    public void Initialize(IState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(IState newState)
    {
        if (!canChangeState) return;

        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void UpdateActiveState()
    {
        currentState.Update();
    }

    public void FixedUpdateActiveState()
    {
        currentState.FixedUpdate();
    }

    public void SwitchOnStateMachine() => canChangeState = true;
    public void SwitchOffStateMachine() => canChangeState = false;
}
