using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour, IEnemyStateProvider, IEnemyMovement, IEnemyCombat, IEnemyStateContext
{
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected float visionRange = 5f;
    [SerializeField] protected float attackCooldown = 2f;

    [Header("Leash / Home")]
    [SerializeField] protected float maxChaseDistance = 10f;
    [SerializeField] protected float loseTargetDelay = 0.5f;
    [SerializeField] protected float recoveryTimeout = 8f;
    [SerializeField] protected float homeTolerance = 0.3f;
    [SerializeField] protected float patrolHalfWidth = 2f;
    [SerializeField] protected float minReEngageDistance = 2f;

    protected Vector2 homePosition;
    protected float patrolMinX;
    protected float patrolMaxX;

    protected Transform player;
    protected CharacterStats characterStats;

    protected MovementManager movement;
    protected AnimationController animationCtrl;
    protected EnemyStateFactory stateFactory;

    protected IEnemyState currentState;
    protected Dictionary<string, IEnemyState> stateCache = new();

    protected float lastTimeAttack = -Mathf.Infinity;
    private float lastDisplacementCheckTime = -Mathf.Infinity;
    private const float DisplacementCheckInterval = 0.3f;
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

        homePosition = transform.position;
        patrolMinX = homePosition.x - patrolHalfWidth;
        patrolMaxX = homePosition.x + patrolHalfWidth;

        movement = new MovementManager(rb, sr, characterStats);
        movement.SetPatrolBounds(patrolMinX, patrolMaxX);
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
        stateCache["Patrol"] = GetPatrolState();
        stateCache["Pursuit"] = GetPursuitState();
        stateCache["Attack"] = GetAttackState();
        stateCache["Hurt"] = GetHurtState(null);
        stateCache["Die"] = GetDieState();
        stateCache["ReturnToPost"] = GetReturnToPostState();
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

    // --- Leash / Home ---
    public Vector2 HomePosition => homePosition;
    public float MaxChaseDistance => maxChaseDistance;
    public float LoseTargetDelay => loseTargetDelay;
    public float RecoveryTimeout => recoveryTimeout;
    public float HomeTolerance => homeTolerance;
    public float PatrolMinX => patrolMinX;
    public float PatrolMaxX => patrolMaxX;

    // maxChaseDistance - visionRange, clamp về sàn minReEngageDistance (mục 18)
    public float ReEngageDistance =>
        Mathf.Max(maxChaseDistance - visionRange, minReEngageDistance);

    // Mọi check leash phải dùng cái này, KHÔNG tự viết Vector2.Distance
    public float DistanceFromHomeX => Mathf.Abs(transform.position.x - homePosition.x);

    // Rule re-engage duy nhất (mục 18): ngoài dead zone + Player còn trong tầm nhìn.
    // Dùng chung bởi ReturnToPostState, và sau này HurtState / check định kỳ Patrol-Idle.
    public bool ShouldReengage()
    {
        return DistanceFromHomeX <= ReEngageDistance
            && GetDistanceToPlayer() <= GetVisionRange();
    }

    public bool ShouldReturnHome()
    {
        if (Time.time - lastDisplacementCheckTime < DisplacementCheckInterval)
            return false;

        lastDisplacementCheckTime = Time.time;
        return DistanceFromHomeX > MaxChaseDistance && IsGrounded();
    }

    // --- IEnemyMovement ---
    public virtual void Patrol() => movement.Patrol();
    public void LookAtPlayer() => movement.LookAtPlayer(player);
    public void MoveTowardPlayer() => movement.MoveTowardPlayer(player);
    public virtual void Pursue() { LookAtPlayer(); MoveTowardPlayer(); }
    public virtual void RetreatFromPlayer() => movement.RetreatFromPlayer(player);
    public void Stop() => movement.Stop();
    public void SetDirection(int dir) => movement.SetDirection(dir);
    public int GetDirection() => movement.GetDirection();
    public float GetDistanceToPlayer() => movement.GetDistanceToPlayer(player);
    public float GetVisionRange() => visionRange;
    public void MoveTowardsX(float targetX) => movement.MoveTowardsX(targetX);
    public bool IsGrounded() => movement.IsGrounded();
    public void SnapToY(float targetY) => movement.SnapToY(targetY);
    public void SnapToPosition(Vector2 targetPos) => movement.SnapToPosition(targetPos);
    public bool IsAtPlatformEdge() => movement.IsAtPlatformEdge();

    // --- IEnemyCombat ---
    public bool IsAttackReady() => Time.time - lastTimeAttack >= attackCooldown;
    public void RecordAttack() => lastTimeAttack = Time.time;
    public float GetAttackRange() => attackRange;
    public virtual void ExecuteAttack() => animationCtrl.PlayAttack();
    public void PlayAnimTrigger(string trigger) => animationCtrl.SetTrigger(trigger);
    public void PlayAnimBool(string name, bool value) => animationCtrl.SetBool(name, value);

    // --- IEnemyStateProvider ---
    public virtual IEnemyState GetIdleState() => stateFactory.CreateIdleState();
    public virtual IEnemyState GetPatrolState() => stateFactory.CreatePatrolState();
    public virtual IEnemyState GetPursuitState() => stateFactory.CreatePursuitState();
    public virtual IEnemyState GetAttackState() => stateFactory.CreateAttackState();
    public virtual IEnemyState GetAlertState() => stateFactory.CreateAlertState();
    public virtual IEnemyState GetHurtState(IEnemyState preState) => stateFactory.CreateHurtState(preState);
    public virtual IEnemyState GetDieState() => stateFactory.CreateDieState();
    public virtual IEnemyState GetKittingState() => stateFactory.CreateKittingState();
    public virtual IEnemyState GetReturnToPostState() => stateFactory.CreateReturnToPostState();

    // --- Animation events (called from Unity) ---
    // VoidBoss overrides ExecuteAttack() as no-op và không gọi method này qua Animation Event
    // hitbox damage của VoidBoss: Stomp/SpikePierce dùng OverlapCircleAll/OverlapBoxAll trực tiếp (SỬA 4)
    public virtual void DealNormalAttackDamage()
    {
        var selfHealth = GetComponent<Health>();
        if (selfHealth == null || selfHealth.CurrentHealth <= 0) return;

        if (player == null) return;

        var playerHealth = player.GetComponent<Health>();
        if (playerHealth == null || playerHealth.CurrentHealth <= 0) return;

        if (Vector2.Distance(transform.position, player.position) >= attackRange) return;
        
        playerHealth.TakeDamage((int)characterStats.Atk);
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

    // --- Gizmos kiểm chứng leash (chỉ vẽ khi chọn object trong Scene view) ---
    protected virtual void OnDrawGizmosSelected()
    {
        Vector2 home = Application.isPlaying ? homePosition : (Vector2)transform.position;
        float minX = Application.isPlaying ? patrolMinX : home.x - patrolHalfWidth;
        float maxX = Application.isPlaying ? patrolMaxX : home.x + patrolHalfWidth;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(home, 0.3f);
        Gizmos.DrawLine(new Vector3(minX, home.y - 0.5f), new Vector3(maxX, home.y - 0.5f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(home.x - maxChaseDistance, home.y),
                        new Vector3(home.x + maxChaseDistance, home.y));
    }
}
