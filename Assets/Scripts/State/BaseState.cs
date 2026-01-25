using UnityEngine;

public interface IState
{
    void Enter();
    void Update();
    void FixedUpdate();
    void Exit();
}

public abstract class BaseState <T> : IState where T : AIController
{
    protected T aiController;
    protected StateMachine stateMachine;
    protected string animBoolName;

    protected BaseState(T aiController, StateMachine stateMachine, string animStateName)
    {
        this.aiController = aiController;
        this.stateMachine = stateMachine;
        this.animBoolName = animStateName;
    }


    public virtual void Enter()
    {
        aiController.Owner.Anim.SetBool(animBoolName, true);
    }

    public virtual void Update()
    {

    }
    public virtual void FixedUpdate()
    {
    }

    public virtual void Exit()
    {
        aiController.Owner.Anim.SetBool(animBoolName, false);
    }
}