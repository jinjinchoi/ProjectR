using UnityEngine;

public abstract class AIController : MonoBehaviour
{
    protected BaseCharacter owner;
    protected StateMachine stateMachine;
    protected Transform target;

    [Header("Combat")]
    [SerializeField] private float attackRange = 1.5f;

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

    public void MovoToTarget()
    {
        owner.SetVelocity(owner.MoveSpeed * GetDirectionToTarget(), owner.Rb.linearVelocity.y);
    }

    private int GetDirectionToTarget()
    {
        if (target == null) return 0;

        float deltaX = target.position.x - owner.transform.position.x;

        if (Mathf.Abs(deltaX) < 0.1f)
            return 0;

        return deltaX > 0 ? 1 : -1;
    }

    public bool CanEnterAttackState()
    {
        if (target == null) return false;

        return Vector2.Distance(owner.transform.position, target.position) <= attackRange;
    }

    public void TryActivateAbilityBy(EAbilityId abilityId)
    {
        int dir = GetDirectionToTarget();
        owner.HandleFlip(dir);
        owner.ASC.TryActivateAbilityById(abilityId);
    }

    private void OnEnable()
    {
        owner.ASC.OnAbilityEnded += OnAbilityEnd;
    }

    private void OnDisable()
    {
        owner.ASC.OnAbilityEnded -= OnAbilityEnd;
    }

    protected virtual void OnAbilityEnd(EAbilityId abilityId)
    {
        
    }

}
