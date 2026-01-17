using System.Collections.Generic;
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

    protected int direction = 1;
    protected int lastPatrolDirection = 1;
    protected IEnemyState currentState;
    protected Dictionary<string, IEnemyState> stateCache = new();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        lastPatrolDirection = direction;

        if(player == null && PlayerManager.Instance != null)
        {
            player = PlayerManager.Instance.PlayerTransform;
        }
        InitializeStates();
        ChangeState(GetIdleState());
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate(this);
        }
    }

    #region Getters
    public float GetDistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, player.position);
    }

    public float GetAttackRange() => attackRange;

    public float GetVisionRange() => visionRange;

    public float GetAttackCooldown() => attackCooldown;

    public float GetLastTimeAttack() => lastTimeAttack;

    public int GetDirection() => direction;
    #endregion

    #region Logic Control
    public void Patrol()
    {
        direction = lastPatrolDirection;
        rb.linearVelocity = new Vector2(speed * direction, rb.linearVelocity.y);
        sr.flipX = direction < 0;
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

    /// <summary>
    /// Pursue behavior: Look at player and move toward them
    /// Can be overridden by subclasses for custom pursuit logic
    /// </summary>
    public virtual void Pursue()
    {
        LookAtPlayer();
        MoveTowardPlayer();
    }

    public void NormalAttack()
    {
        animator.SetTrigger("Attack");
    }

    public virtual void ExecuteAttack()
    {
        NormalAttack();
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

    public void HandleEnemyDeath()
    {
        Health health = GetComponent<Health>();
        if (health != null && health.lootManager != null)
        {
            health.lootManager.SpawnLoot();
        }

        Debug.Log($"{gameObject.name} has died.");
    }
    public void SetAnimatorBool(string parameter, bool value)
    {
        animator.SetBool(parameter, value);
    }

    public void SetAnimatorTrigger(string parameter)
    {
        animator.SetTrigger(parameter);
    }

    public void SetLastTimeAttack(float time)
    {
        lastTimeAttack = time;
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
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
    #endregion

    #region State Management

    protected virtual void InitializeStates()
    {
        stateCache["Idle"] = GetIdleState();
        stateCache["Pursuit"] = GetPursuitState();
        stateCache["Attack"] = GetAttackState();
        stateCache["Hurt"] = GetHurtState(null);
        stateCache["Die"] = GetDieState();
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState?.OnExit(this);
        currentState = newState;
        currentState.OnEnter(this);
    }

    public void ChangeStateByName(string stateName)
    {
        if (stateCache.TryGetValue(stateName, out var state) && state != null)
        {
            ChangeState(state);
        }
        else
        {
            Debug.LogWarning($"State {stateName} not found in cache.");
        }
    }

    public IEnemyState GetCurrentState()
    {
        return currentState;
    }

    public virtual IEnemyState GetIdleState()
    {
        return new IdleState();
    }

    public virtual IEnemyState GetPursuitState()
    {
        return new BasePursuitState();
    }

    public virtual IEnemyState GetAttackState()
    {
        return new BaseAttackState();
    }

    public virtual IEnemyState GetKittingState()
    {
        return new KittingState();
    }

    public virtual IEnemyState GetAlertState()
    {
        return new AlertState();
    }
    public virtual IEnemyState GetHurtState(IEnemyState preState)
    {
        return new HurtState(preState);
    }

    public virtual IEnemyState GetDieState()
    {
        return new DieState();
    }
    #endregion
}
