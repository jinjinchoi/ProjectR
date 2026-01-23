using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour, IAbilityOwner, IDamageable
{

    #region IAbilityActor
    public Animator Anim => anim;
    public Transform OwnerTransform => transform;
    public AnimationTrigger AnimationTrigger => animationTrigger;
    public Transform AttackPoint => attackPoint;
    #endregion
    public Rigidbody2D Rb => rb;
    public AbilitySystemComponent ASC => abilitySystemComponent;
    public float MoveSpeed => moveSpeed;
    public bool IsGrounded => isGrounded;

    [Header("Debug")]
    [SerializeField] protected bool showDebug = false;

    [Header("Hostile Target Detect")]
    [SerializeField] protected Vector2 hostileDetectSize = new Vector2(50, 10);
    [SerializeField] private LayerMask hostileLayerMask;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float groundCheckRadius = 0.5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayerMask;

    [Header("Ability System")]
    // 캐릭터에게 부여할 어빌리티 목록
    [SerializeField] private List<BaseAbilityDataSO> defaultAbilities;
    [SerializeField] private Transform attackPoint;

    private Rigidbody2D rb;
    private AbilitySystemComponent abilitySystemComponent;
    private Animator anim;
    private AnimationTrigger animationTrigger;
    private bool isGrounded = false;
    private bool facingRight = true;
    private int facingDirection = 1;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        abilitySystemComponent = GetComponent<AbilitySystemComponent>();

        animationTrigger = GetComponentInChildren<AnimationTrigger>();
        anim = GetComponentInChildren<Animator>();
        
    }

    protected virtual void Start()
    {
        if (abilitySystemComponent == null) return;

        abilitySystemComponent.Initialize(this);
        foreach (BaseAbilityDataSO ability in defaultAbilities)
        {
            abilitySystemComponent.GiveAbility(ability);
        }
    }

    protected virtual void Update()
    {
        CheckGrounded();
    }

    public void TakeDamage(FDamageInfo damageInfo)
    {
        Debug.Log(damageInfo.Damage);
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        anim.SetFloat("xVelocity", Mathf.Abs(xVelocity));
        HandleFlip(xVelocity);
    }


    public void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && !facingRight)
        {
            Flip();
        }
        else if (xVelocity < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
        facingDirection *= -1;
    }

    private void CheckGrounded()
    {
        if (!groundCheck) return;

        Collider2D result = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        if (result != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public Transform FindClosestTargetWithinBox()
    {
        Collider2D[] detectedEnemy = Physics2D.OverlapBoxAll(transform.position, hostileDetectSize, 0f, hostileLayerMask);

        if (detectedEnemy.Length == 0)
        {
            Debug.Log("detected Enemy is 0, FindClosestTarget");
            return null;
        }

        Transform closestTarget = null;
        float minSqrDistance = float.MaxValue;

        Vector2 myPos = transform.position;

        foreach (Collider2D enemy in detectedEnemy)
        {
            // TODO: 에너미 death 여부 확인해야함.

            Vector2 enemyPos = enemy.transform.position;
            float sqrDist = (enemyPos - myPos).sqrMagnitude;

            if (sqrDist < minSqrDistance)
            {
                minSqrDistance = sqrDist;
                closestTarget = enemy.transform;
            }
        }

        return closestTarget;
    }

    protected virtual void OnDrawGizmos()
    {
        if (!showDebug) return;

        if (!groundCheck) return;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }


}
