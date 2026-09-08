using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBossController : EnemyController
{
    [Header("Fixed Hover")]
    [Tooltip("Giữ boss tại vị trí lúc bắt đầu scene. Tắt để dùng Fixed Hover Position.")]
    [SerializeField] private bool useInitialPosition = true;
    [Tooltip("Tọa độ cố định của boss khi Use Initial Position tắt.")]
    [SerializeField] private Vector3 fixedHoverPosition;
    [Tooltip("Tọa độ Y cố định để spawn BatHole.")]
    [SerializeField] private float holeSpawnY = -7f;
    [Header("Spawn Prefabs")]
    [SerializeField] private GameObject batSpherePrefab;
    [Tooltip("Độ cao spawn fireball tính từ vị trí cố định của boss.")]
    [SerializeField] private float dropHeight = 4f;
    [SerializeField] private GameObject pillarPrefab;
    [SerializeField] private GameObject holePrefab;
    [SerializeField] private Transform[] pillarSpawnPoints;

    [Header("Pillar Spawn Config")]
    [SerializeField] private int maxActivePillars = 3;
    [SerializeField] private float pillarSpawnCooldown = 7f;
    [SerializeField] private float maxPlayerDistance = 12f;
    [SerializeField] private float minPillarDistance = 5f;

    [Header("Hurt Effect")]
    [SerializeField] private SpriteRenderer bossSprite;
    [SerializeField] private Color hurtTint = Color.red;
    [SerializeField] private float hurtFlashDuration = 0.15f;

    [Header("Death")]
    [SerializeField] private GameObject deathVFX;

    private BossHealthBarUI bossHealthBar;
    private Vector3 hoverOrigin;
    private Color originalColor;
    private bool isDead;
    private bool isAwake;
    private bool isInitialized;
    private int cachedMaxHP;
    private Rigidbody2D bossRigidbody;
    private Animator bossAnimator;

    private List<Pillar> activePillars = new List<Pillar>();
    private float pillarSpawnTimer;

    public event System.Action OnBossDefeated;
    public int PillarBurstDamage => Mathf.RoundToInt(cachedMaxHP * 0.25f);
    public int CachedMaxHP => cachedMaxHP;

    protected override void Start()
    {
        InitializeBoss();
    }

    private void InitializeBoss()
    {
        if (isInitialized)
            return;

        player = PlayerManager.Instance != null ? PlayerManager.Instance.PlayerTransform : null;
        characterStats = GetComponent<CharacterStats>();
        if (characterStats == null)
        {
            Debug.LogError("BatBoss missing CharacterStats!", this);
            return;
        }

        bossRigidbody = GetComponent<Rigidbody2D>();
        var sr = GetComponent<SpriteRenderer>();
        bossAnimator = GetComponent<Animator>();

        movement = new MovementManager(bossRigidbody, sr, characterStats, spriteBaseFlipX);
        animationCtrl = new AnimationController(bossAnimator);
        stateFactory = null;

        cachedMaxHP = (int)characterStats.MaxHP;

        hoverOrigin = useInitialPosition ? transform.position : fixedHoverPosition;
        transform.position = hoverOrigin;

        if (bossRigidbody != null)
        {
            bossRigidbody.bodyType = RigidbodyType2D.Kinematic;
            bossRigidbody.gravityScale = 0f;
            bossRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            bossRigidbody.linearVelocity = Vector2.zero;
            bossRigidbody.angularVelocity = 0f;
        }

        if (bossSprite != null)
            originalColor = bossSprite.color;

        CacheBossStates();

        bossHealthBar = GetComponentInChildren<BossHealthBarUI>(true);
        bossHealthBar?.Hide();

        isAwake = false;
        isInitialized = true;

        if (bossAnimator != null)
            bossAnimator.Play("Bat_Sleep", 0, 0f);
    }

    protected override void Update()
    {
        if (isDead) return;
        base.Update();

        // Death must keep ticking even if the boss has not been awakened yet.
        if (!isAwake) return;
        UpdatePillarSpawning(Time.deltaTime);
    }

    private void CacheBossStates()
    {
        stateCache["Combat"] = new BatCombatState();
        stateCache["DropSphere"] = new GenericAttackState("Atk1", 1.2f, "Combat");
        stateCache["SpawnDoT"] = new GenericAttackState("Atk2", 1.2f, "Combat");
        stateCache["Hurt"] = new HurtState("Combat", false);
        stateCache["Die"] = new DieState(2f, () => HandleEnemyDeath());
    }

    // --- Movement (no-ops — boss flies) ---
    public override void Patrol() { }
    public override void Pursue() { }
    public override void RetreatFromPlayer() { }
    public override void ExecuteAttack() { }
    public override void DealNormalAttackDamage() => base.DealNormalAttackDamage();
    public override IEnemyState GetHurtState(IEnemyState currentState) => currentState;
    public override IEnemyState GetDieState() => stateCache["Die"];

    // --- Combat movement ---
    // Kept as a compatibility wrapper for the legacy BatHoverState.
    public void UpdateHover(float dt)
    {
        transform.position = hoverOrigin;

        if (bossRigidbody != null)
        {
            bossRigidbody.position = hoverOrigin;
            bossRigidbody.linearVelocity = Vector2.zero;
            bossRigidbody.angularVelocity = 0f;
        }
    }

    public void FacePlayer()
    {
        if (player != null)
            movement?.LookAtPlayer(player);
    }

    public void ChasePlayer(float dt)
    {
        if (player == null || characterStats == null)
            return;

        FacePlayer();

        Vector3 target = transform.position;
        target.x = player.position.x;
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            characterStats.MovementSpeed * 1.5f * dt);

        if (bossRigidbody != null)
        {
            bossRigidbody.position = transform.position;
            bossRigidbody.linearVelocity = Vector2.zero;
            bossRigidbody.angularVelocity = 0f;
        }
    }

    public void PickNextAttack()
    {
        float roll = Random.value;
        if (roll < 0.5f)
            SwitchTo("DropSphere");
        else
            SwitchTo("SpawnDoT");
    }

    public void ForceHurtState()
    {
        FlashHurt();
        ChangeState(stateCache["Hurt"]);
    }

    // ============================================================
    // Animation Event Callbacks (public, gọi từ Animation clip)
    // ============================================================

    public void DropSphere()
    {
        if (batSpherePrefab == null || player == null)
            return;

        Vector3 spawnPos = new Vector3(player.position.x, transform.position.y + dropHeight, 0f);
        GameObject sphere = Instantiate(batSpherePrefab, spawnPos, Quaternion.identity);
        sphere.GetComponent<BatSphere>()?.Init(player);
    }

    public void SpawnPillar()
    {
        TrySpawnPillar();
    }

    private void UpdatePillarSpawning(float dt)
    {
        activePillars.RemoveAll(p => p == null);

        if (activePillars.Count >= maxActivePillars)
        {
            pillarSpawnTimer = pillarSpawnCooldown;
            return;
        }

        pillarSpawnTimer -= dt;
        if (pillarSpawnTimer <= 0f)
        {
            if (TrySpawnPillar())
                pillarSpawnTimer = pillarSpawnCooldown;
        }
    }

    private bool TrySpawnPillar()
    {
        if (pillarPrefab == null || pillarSpawnPoints == null || pillarSpawnPoints.Length == 0 || player == null)
            return false;

        activePillars.RemoveAll(p => p == null);

        if (activePillars.Count >= maxActivePillars)
            return false;

        var validPoints = new List<Transform>();
        foreach (var pt in pillarSpawnPoints)
        {
            if (pt == null) continue;

            if (Vector2.Distance(pt.position, player.position) > maxPlayerDistance)
                continue;

            bool tooClose = false;
            foreach (var _pillar in activePillars)
            {
                if (_pillar != null && Vector2.Distance(pt.position, _pillar.transform.position) < minPillarDistance)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue;

            validPoints.Add(pt);
        }

        if (validPoints.Count == 0)
            return false;

        Transform chosen = validPoints[Random.Range(0, validPoints.Count)];
        GameObject obj = Instantiate(pillarPrefab, chosen.position, Quaternion.identity);
        Pillar pillar = obj.GetComponent<Pillar>();
        if (pillar != null)
        {
            pillar.Init(this);
            activePillars.Add(pillar);
        }
        return true;
    }

    public void SpawnAoECircle()
    {
        if (holePrefab == null || player == null)
            return;

        Vector3 spawnPos = player.position;
        spawnPos.y = holeSpawnY;
        Instantiate(holePrefab, spawnPos, Quaternion.identity);
    }

    public void OnAttackAnimEnd()
    {
        if (currentState is GenericAttackState)
            SwitchTo("Combat");
    }

    public void OnWakeUpComplete()
    {
        if (!isInitialized || isDead)
            return;

        isAwake = true;
        if (bossRigidbody != null)
            bossRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        ChangeState(stateCache["Combat"]);
    }

    public void BeginEncounter()
    {
        InitializeBoss();

        if (!isInitialized || isDead || isAwake || bossAnimator == null)
            return;

        bossHealthBar?.SetBoss(GetComponent<Health>());
        bossAnimator.Play("Bat_WakeUp", 0, 0f);
    }

    // --- Hurt ---
    public void FlashHurt()
    {
        if (bossSprite == null) return;
        StopAllCoroutines();
        StartCoroutine(HurtFlashRoutine());
    }

    private IEnumerator HurtFlashRoutine()
    {
        bossSprite.color = hurtTint;
        yield return new WaitForSeconds(hurtFlashDuration);
        bossSprite.color = originalColor;
    }

    // --- Death ---
    public override void HandleEnemyDeath()
    {
        isDead = true;
        if (deathVFX != null)
            Instantiate(deathVFX, transform.position, Quaternion.identity);
        bossHealthBar?.Hide();
        OnBossDefeated?.Invoke();

        Health h = GetComponent<Health>();
        if (h != null && h.lootManager != null)
            h.lootManager.SpawnLoot();

        // Keep boss cleanup explicit. DieState also destroys after this callback,
        // but this makes the boss lifecycle safe if the state is interrupted.
        Destroy(gameObject);
    }

    public void SetHealthBar(BossHealthBarUI bar) => bossHealthBar = bar;

    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
