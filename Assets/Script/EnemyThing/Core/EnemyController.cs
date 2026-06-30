using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour, IEnemyStateProvider, IEnemyMovement, IEnemyCombat, IEnemyStateContext
{
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected float visionRange = 5f;
    [SerializeField] protected float attackCooldown = 2f;

    protected Transform player;
    protected CharacterStats characterStats;

    protected MovementManager movement;
    protected AnimationController animationCtrl;
    protected EnemyStateFactory stateFactory;

    protected IEnemyState currentState;
    protected Dictionary<string, IEnemyState> stateCache = new();

    protected float lastTimeAttack = -Mathf.Infinity;

    protected virtual void Start()
    {
        var rb = GetComponent<Rigidbody2D>();
        var sr = GetComponent<SpriteRenderer>();
        var animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        if (characterStats == null)
        {
            Debug.LogWarning($"{gameObject.name} missing CharacterStats!", gameObject);
            return;
        }

        if (player == null && PlayerManager.Instance != null)
            player = PlayerManager.Instance.PlayerTransform;

        movement = new MovementManager(rb, sr, characterStats);
        animationCtrl = new AnimationController(animator);
        stateFactory = CreateStateFactory();

        CacheStates();
        ChangeState(GetIdleState());
    }

    protected virtual void Update()
    {
        currentState?.OnUpdate(this, this, this);
    }

    protected virtual EnemyStateFactory CreateStateFactory()
    {
        return new EnemyStateFactory();
    }

    protected virtual void CacheStates()
    {
        stateCache["Idle"] = GetIdleState();
        stateCache["Pursuit"] = GetPursuitState();
        stateCache["Attack"] = GetAttackState();
        stateCache["Hurt"] = GetHurtState(null);
        stateCache["Die"] = GetDieState();
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState?.OnExit(this, this, this);
        currentState = newState;
        currentState?.OnEnter(this, this, this);
    }

    public void SwitchTo(string stateName)
    {
        if (stateCache.TryGetValue(stateName, out var state) && state != null)
        {
            ChangeState(state);
        }
        else
        {
            Debug.LogWarning($"State {stateName} not found in cache.", this);
        }
    }

    public IEnemyState GetCurrentState() => currentState;

    // --- IEnemyMovement ---
    public virtual void Patrol() => movement.Patrol();
    public void LookAtPlayer() => movement.LookAtPlayer(player);
    public void MoveTowardPlayer() => movement.MoveTowardPlayer(player);
    public virtual void Pursue() { LookAtPlayer(); MoveTowardPlayer(); }
    public virtual void RetreatFromPlayer() => movement.RetreatFromPlayer(player);
    public void SetDirection(int dir) => movement.SetDirection(dir);
    public int GetDirection() => movement.GetDirection();
    public float GetDistanceToPlayer() => movement.GetDistanceToPlayer(player);
    public float GetVisionRange() => visionRange;

    // --- IEnemyCombat ---
    public bool IsAttackReady() => Time.time - lastTimeAttack >= attackCooldown;
    public void RecordAttack() => lastTimeAttack = Time.time;
    public float GetAttackRange() => attackRange;
    public virtual void ExecuteAttack() => animationCtrl.PlayAttack();
    public void PlayAnimTrigger(string trigger) => animationCtrl.SetTrigger(trigger);
    public void PlayAnimBool(string name, bool value) => animationCtrl.SetBool(name, value);

    // --- IEnemyStateProvider ---
    public virtual IEnemyState GetIdleState() => stateFactory.CreateIdleState();
    public virtual IEnemyState GetPursuitState() => stateFactory.CreatePursuitState();
    public virtual IEnemyState GetAttackState() => stateFactory.CreateAttackState();
    public virtual IEnemyState GetAlertState() => stateFactory.CreateAlertState();
    public virtual IEnemyState GetHurtState(IEnemyState preState) => stateFactory.CreateHurtState(preState);
    public virtual IEnemyState GetDieState() => stateFactory.CreateDieState();
    public virtual IEnemyState GetKittingState() => stateFactory.CreateKittingState();

    // --- Animation events (called from Unity) ---
    public virtual void DealNormalAttackDamage()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) < attackRange)
        {
            var playerHealth = player.GetComponent<Health>();
            if (playerHealth != null && characterStats != null)
            {
                playerHealth.TakeDamage((int)characterStats.Atk);
            }
        }
    }

    public virtual void HandleEnemyDeath()
    {
        Health health = GetComponent<Health>();
        if (health != null && health.lootManager != null)
        {
            health.lootManager.SpawnLoot();
        }
    }

    // --- Collision ---
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
            movement.OnHitObstacle();
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
            movement.OnHitObstacle();
    }
}
