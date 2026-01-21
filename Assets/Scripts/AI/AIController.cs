using UnityEngine;

public abstract class AIController : MonoBehaviour
{
    protected BaseCharacter owner;
    protected StateMachine stateMachine;

    protected virtual void Awake()
    {
        owner = GetComponent<BaseCharacter>();
        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        stateMachine.UpdateActiveState();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine.FixedUpdateActiveState();
    }

    public void AnimationEnd()
    {

    }
}
