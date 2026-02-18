using System.Collections;
using UnityEngine;

public abstract class AIController : MonoBehaviour
{
    protected BaseCharacter owner;
    protected StateMachine stateMachine;
    protected Transform target;

    [Header("Combat")]
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected Vector2 hostileDetectSize = new (50, 10);
    [SerializeField] protected LayerMask hostileLayerMask;
    [Range(0, 1)]
    [SerializeField] protected float skillProbability = 0.25f;
    [Range(0, 1)]
    [SerializeField] protected float retreatProbability = 0.4f;


    public BaseCharacter Owner => owner;
    public EAbilityId PendingAbilityId;

    private Coroutine skillStateDecisionCo;
    bool activate;


    protected virtual void Awake()
    {
        owner = GetComponent<BaseCharacter>();
        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {
        skillStateDecisionCo = StartCoroutine(AttackDecisionLoop());
    }

    protected virtual void Update()
    {
        stateMachine.UpdateActiveState();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine.FixedUpdateActiveState();
    }

    protected virtual void OnEnable()
    {
        owner.ASC.OnAbilityEnded += OnAbilityEnd;
        activate = true;
    }

    protected virtual void OnDisable()
    {
        owner.ASC.OnAbilityEnded -= OnAbilityEnd;
        StopCoroutine(skillStateDecisionCo);
        activate = false;
    }

    private IEnumerator AttackDecisionLoop()
    {
        while (activate)
        {
            if (PendingAbilityId == EAbilityId.None)
            {
                DecideAttackType();
            }
            yield return new WaitForSeconds(2f);
        }
    }

    private void DecideAttackType()
    {
        if (Random.value <= skillProbability)
        {
            PendingAbilityId = owner.ASC.GetRandomAbilityId();
        }
    }

    protected bool ShouldRetreat()
    {
        return Random.value < retreatProbability;
    }

    public void MovoToTarget()
    {
        owner.SetVelocity(owner.MoveSpeed * GetDirectionToTarget(), owner.Rb.linearVelocity.y);
    }

    public void Retreat()
    {
        int dir = -GetDirectionToTarget();
        owner.SetVelocity(owner.MoveSpeed * dir, owner.Rb.linearVelocity.y);
    }

    private int GetDirectionToTarget()
    {
        if (target == null) return 0;

        float deltaX = target.position.x - owner.transform.position.x;

        if (Mathf.Abs(deltaX) < 0.1f)
            return 0;

        return deltaX > 0 ? 1 : -1;
    }

    public void StopOwner()
    {
        owner.SetVelocity(0f, owner.Rb.linearVelocity.y);
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

    public bool CanEnterSkillState(EAbilityId abilityId)
    {
        if (target == null || PendingAbilityId == EAbilityId.None) return false;

        if (owner.ASC.GetDamageAbilityData(abilityId) is not DamageAbilityDataSO data)
            return false;

        return Vector2.Distance(owner.transform.position, target.position) <= data.minAttackRange;
    }

    protected virtual void OnAbilityEnd(EAbilityId abilityId)
    {
        
    }

}
