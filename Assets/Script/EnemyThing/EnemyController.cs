using UnityEngine;

public class EnemyController : MonoBehaviour, IEnemyStateProvider
{
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator animator;

    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected int attackDamage = 1;
    [SerializeField] protected float visionRange = 5f;
    [SerializeField] protected float attackCooldown = 2f;
    [SerializeField] protected float lastTimeAttack = -Mathf.Infinity;
    [SerializeField] protected Transform player;

    protected int direction = 1; // 1 for right, -1 for left
    protected int lastPatrolDirection = 1; // 1 for right, -1 for left
    protected IEnemyState currentState;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        lastPatrolDirection = direction;

        ChangeState(GetIdleState()); 
    }

    private void Update()
    {
        if (player == null) return;
        currentState?.OnUpdate(this);
    }

    // Enemy behavior methods
    public void Patrol()
    {
        direction = lastPatrolDirection;
        // Move enemy horizontally
        rb.linearVelocity = new Vector2(speed * direction, rb.linearVelocity.y);

        // Flip sprite based on direction
        sr.flipX = direction < 0;
    }

    public void NormalAttack()
    {
        animator.SetTrigger("Attack");
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        // Flip direction on collision with ground or obstacle
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            direction *= -1;
            lastPatrolDirection = direction;
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            direction *= -1;
            lastPatrolDirection = direction;
        }
    }

    public virtual void DealNormalAttackDamage()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) < attackRange)
        {
            var playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
    
    public void LookAtPlayer()
    {
        if (player == null) return;
        direction = player.position.x > transform.position.x ? 1 : -1;
        sr.flipX = direction < 0;
        rb.linearVelocity = Vector2.zero;
    }

    public void MoveTowardPlayer()
    {
        if (player == null) return;
        float moveDirection = player.position.x > transform.position.x ? 1 : -1;
        direction = (int)moveDirection;
        rb.linearVelocity = new Vector2(speed * 1.5f * direction, rb.linearVelocity.y);
        sr.flipX = direction < 0;
    }

    public virtual void ExecuteAttack()
    {
        if (Time.time - lastTimeAttack >= attackCooldown)
        {
            NormalAttack();
            lastTimeAttack = Time.time;
        }
    }

    // State management methods
    public void ChangeState(IEnemyState newState)
    {
        currentState?.OnExit(this);
        currentState = newState;
        currentState.OnEnter(this);
    }

    public float GetDistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, player.position);
    }

    public float GetAttackRange() => attackRange;

    public float GetVisionRange() => visionRange;

    public void SetAnimatorBool(string parameter, bool value)
    {
        animator.SetBool(parameter, value);
    }

    public void SetAnimatorTrigger(string parameter)
    {
        animator.SetTrigger(parameter);
    }

    // ? Getter/Setter cho attack timing
    public float GetLastTimeAttack() => lastTimeAttack;

    public void SetLastTimeAttack(float time)
    {
        lastTimeAttack = time;
    }

    public float GetAttackCooldown() => attackCooldown;

    /// <summary>
    /// Factory method - override in subclass for custom idle state
    /// </summary>
    public virtual IEnemyState GetIdleState()
    {
        return new IdleState();
    }

    /// <summary>
    /// Factory method - override in subclass for custom pursuit state
    /// </summary>
    public virtual IEnemyState GetPursuitState()
    {
        return new PursuitState();
    }

    /// <summary>
    /// Factory method - override in subclass for custom attack state
    /// </summary>
    public virtual IEnemyState GetAttackState()
    {
        return new AttackState();
    }
}
