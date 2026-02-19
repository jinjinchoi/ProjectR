using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour, IAbilityOwner, IDamageable
{
    public event Action CharacterDied;

    // ability나 ai controller 등에서 상호 참조를 막기 위해 actor에 직접 접근하는 것이 아닌 인터페이스 사용
    #region IAbilityOwner
    public Animator Anim => anim;
    public Transform Transform => transform;
    public AnimationTrigger AnimationTrigger => animationTrigger;
    public Transform AttackPoint => attackPoint;
    public EFaction Faction => faction;
    public int FacingDir => facingDir;
    #endregion

    [Header("Debug")]
    [SerializeField] protected bool showDebug = false;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float groundCheckRadius = 0.5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayerMask;

    [Header("Combat")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private string deadLayerName = "Dead";
    [SerializeField] protected EFaction faction = EFaction.Neutral;


    [Header("ASC")]
    [SerializeField] private AttributeSO attributeInfoSO;
    [SerializeField] private List<BaseAbilityDataSO> defaultAbilities; // 캐릭터에게 기본으로 부여할 ability

    private Rigidbody2D rb;
    private AbilitySystemComponent abilitySystemComponent;
    private Animator anim;
    private AnimationTrigger animationTrigger;
    private VFXComponent vfxComponent;
    private LayerMask originalLayerMask;
    private bool isGrounded = false;
    private bool isFacingRight = true;
    private bool isKnockback = false;
    private Coroutine knockbackCo;
    private bool isDead = false;
    private int facingDir = 1;
    private DamageDealer damageDealer;

    #region Getter
    public Rigidbody2D Rb => rb;
    public DamageDealer DamageDealer => damageDealer;
    public AbilitySystemComponent ASC => abilitySystemComponent ? abilitySystemComponent : GetComponent<AbilitySystemComponent>();
    public float MoveSpeed => moveSpeed;
    public bool IsGrounded => isGrounded;
    public bool IsDead => isDead;
    #endregion

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        vfxComponent = GetComponent<VFXComponent>();

        animationTrigger = GetComponentInChildren<AnimationTrigger>();
        anim = GetComponentInChildren<Animator>();
        damageDealer = GetComponentInChildren<DamageDealer>();


        originalLayerMask = gameObject.layer;

        abilitySystemComponent = GetComponent<AbilitySystemComponent>();
        abilitySystemComponent.Init(this);
        ApplyDefualtAttribute();
        GiveDefaultAbility();
    }

    void ApplyDefualtAttribute()
    {
        if (attributeInfoSO == null)
        {
            DebugHelper.LogWarning($"attributeInfoSO is not set on [{gameObject.name}]");
            return;
        }

        foreach (AttributeInitInfo info in attributeInfoSO.Attributes)
        {
            FAttributeModifier modifier = new()
            {
                attributeType = info.attributeType,
                value = info.baseValue,
                policy = EModifierPolicy.Instant,
                operation = EModifierOp.Override
            };
            abilitySystemComponent.ApplyModifier(modifier);
        }

        float maxHealth = abilitySystemComponent.AttributeSet.GetAttributeValue(EAttributeType.MaxHealth);
        float maxMana = abilitySystemComponent.AttributeSet.GetAttributeValue(EAttributeType.MaxMana);

        FAttributeModifier healthModifier = new()
        {
            attributeType = EAttributeType.CurrentHealth,
            value = maxHealth,
            policy = EModifierPolicy.Instant,
            operation = EModifierOp.Override
        };
        abilitySystemComponent.ApplyModifier(healthModifier);

        FAttributeModifier manaModifier = new()
        {
            attributeType = EAttributeType.CurrentMana,
            value = maxMana,
            policy = EModifierPolicy.Instant,
            operation = EModifierOp.Override
        };
        abilitySystemComponent.ApplyModifier(manaModifier);
    }

    void GiveDefaultAbility()
    {
        if (defaultAbilities.Count == 0)
        {
            DebugHelper.LogWarning($"defaultAbilities is not set on [{gameObject.name}]");
            return;
        }

        foreach (BaseAbilityDataSO attributeInfoSO in defaultAbilities)
        {
            abilitySystemComponent.GiveAbility(attributeInfoSO);
        }
        
    }

    protected virtual void Start()
    {
        abilitySystemComponent.AttributeSet.OnDaed += OnDead;
    }

    protected virtual void Update()
    {
        CheckGrounded();
    }

    public virtual void TakeDamage(FDamageInfo damageInfo)
    {
        if (!abilitySystemComponent || isDead) return;

        FAttributeModifier damageModifier = new()
        {
            attributeType = EAttributeType.IncommingDamage,
            value = DamageCalculator.CalculateIncomingDamage(abilitySystemComponent.AttributeSet, damageInfo),
            policy = EModifierPolicy.Instant,
            operation = EModifierOp.Add
        };

        if (vfxComponent) vfxComponent.PlayOnDamageVfx();
        abilitySystemComponent.ApplyModifier(damageModifier);

        if (damageInfo.KnockbackPower != Vector2.zero)
        {
            if (knockbackCo != null)
                StopCoroutine(knockbackCo);

            int dir = transform.position.x > damageInfo.DamageSource.position.x ? 1 : -1;
            Vector2 kncokback = damageInfo.KnockbackPower;
            kncokback.x *= dir;
            knockbackCo = StartCoroutine(ExcuteKnockback(kncokback, damageInfo.KnockbackDuration));
        }
    }

    private IEnumerator ExcuteKnockback(Vector2 knockback, float duration)
    {
        isKnockback = true;
        rb.linearVelocity = knockback;
        
        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector2.zero;
        isKnockback = false;

    }

    protected virtual void OnDead()
    {
        isDead = true;
        int deadLayer = LayerMask.NameToLayer(deadLayerName);
        gameObject.layer = deadLayer;
        CharacterDied?.Invoke();
    }

    private void Revive()
    {
        isDead = false;
        gameObject.layer = originalLayerMask;
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnockback || isDead) return;

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        anim.SetFloat("xVelocity", Mathf.Abs(xVelocity));
        HandleFlip(xVelocity);
    }


    public void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (xVelocity < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        if (isDead) return;

        transform.Rotate(0f, 180f, 0f);
        isFacingRight = !isFacingRight;
        facingDir *= -1;
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

    protected virtual void OnDrawGizmos()
    {
        if (!showDebug) return;

        if (!groundCheck) return;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        abilitySystemComponent.AttributeSet.OnDaed -= OnDead;
    }
}
