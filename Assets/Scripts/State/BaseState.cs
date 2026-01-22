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
    protected BaseCharacter owner;
    protected T aiController;
    protected StateMachine stateMachine;
    protected string animBoolName;

    protected BaseState(BaseCharacter owner, T aiController, StateMachine stateMachine, string animStateName)
    {
        this.owner = owner;
        this.aiController = aiController;
        this.stateMachine = stateMachine;
        this.animBoolName = animStateName;
    }


    public virtual void Enter()
    {
        owner.Anim.SetBool(animBoolName, true);
    }

    public virtual void Update()
    {

    }
    public virtual void FixedUpdate()
    {
    }

    public virtual void Exit()
    {
        owner.Anim.SetBool(animBoolName, false);
    }
}