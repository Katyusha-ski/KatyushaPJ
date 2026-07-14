using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBossController : EnemyController
{
    [Header("Boss Settings")]
    [SerializeField] private float hoverHeight = 4f;
    [SerializeField] private float hoverSpeed = 0.8f;
    [SerializeField] private float hoverAmplitude = 1.5f;
    [Header("Spawn Prefabs")]
    [SerializeField] private GameObject batSpherePrefab;
    [SerializeField] private GameObject pillarPrefab;
    [SerializeField] private GameObject holePrefab;
    [SerializeField] private Transform[] sphereSpawnPoints;
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
    private float hoverPhase;
    private Color originalColor;
    private bool isDead;
    private bool isAwake;
    private int cachedMaxHP;

    private List<Pillar> activePillars = new List<Pillar>();
    private float pillarSpawnTimer;

    public event System.Action OnBossDefeated;
    public int PillarBurstDamage => Mathf.RoundToInt(cachedMaxHP * 0.25f);
    public int CachedMaxHP => cachedMaxHP;

    protected override void Start()
    {
        player = PlayerManager.Instance != null ? PlayerManager.Instance.PlayerTransform : null;
        characterStats = GetComponent<CharacterStats>();
        if (characterStats == null)
        {
            Debug.LogError("BatBoss missing CharacterStats!", this);
            return;
        }

        var rb = GetComponent<Rigidbody2D>();
        var sr = GetComponent<SpriteRenderer>();
        var animator = GetComponent<Animator>();

        movement = new MovementManager(rb, sr, characterStats);
        animationCtrl = new AnimationController(animator);
        stateFactory = null;

        cachedMaxHP = (int)characterStats.MaxHP;

        hoverOrigin = transform.position;
        hoverPhase = 0f;
        if (bossSprite != null)
            originalColor = bossSprite.color;

        CacheBossStates();

        isAwake = false;
        animator.Play("Bat_Sleep", 0, 0f);
    }

    protected override void Update()
    {
        if (isDead || !isAwake) return;
        base.Update();
        UpdatePillarSpawning(Time.deltaTime);
    }

    private void CacheBossStates()
    {
        stateCache["Hover"] = new BatHoverState();
        stateCache["DropSphere"] = new GenericAttackState("Atk1", 1.2f, "Hover");
        stateCache["SpawnDoT"] = new GenericAttackState("Atk2", 1.2f, "Hover");
        stateCache["Hurt"] = new HurtState("Hover", false);
        stateCache["Die"] = new DieState(2f, () => HandleEnemyDeath());
    }

    // --- Movement (no-ops — boss flies) ---
    public override void Patrol() { }
    public override void Pursue() { }
    public override void RetreatFromPlayer() { }
    public override void ExecuteAttack() { }
    public override void DealNormalAttackDamage() { }
    public override IEnemyState GetHurtState(IEnemyState currentState) => currentState;
    public override IEnemyState GetDieState() => stateCache["Die"];

    // --- Hover ---
    public void UpdateHover(float dt)
    {
        hoverPhase += dt * hoverSpeed;
        float xOff = Mathf.Sin(hoverPhase) * hoverAmplitude;
        float yOff = Mathf.Sin(hoverPhase * 0.7f) * 0.5f;
        Vector3 target = hoverOrigin + new Vector3(xOff, yOff + hoverHeight, 0f);
        target.x += hoverOrigin.x;
        transform.position = Vector3.Lerp(transform.position, target, dt * 2f);
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
        if (batSpherePrefab == null || sphereSpawnPoints == null || sphereSpawnPoints.Length == 0)
            return;

        Transform pt = sphereSpawnPoints[Random.Range(0, sphereSpawnPoints.Length)];
        GameObject sphere = Instantiate(batSpherePrefab, pt.position, Quaternion.identity);
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
        spawnPos.y = transform.position.y - hoverHeight;
        Instantiate(holePrefab, spawnPos, Quaternion.identity);
    }

    public void OnAttackAnimEnd()
    {
        if (currentState is GenericAttackState)
            SwitchTo("Hover");
    }

    public void OnWakeUpComplete()
    {
        isAwake = true;
        ChangeState(stateCache["Hover"]);
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
    }

    public void SetHealthBar(BossHealthBarUI bar) => bossHealthBar = bar;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        if (sphereSpawnPoints != null)
            foreach (var pt in sphereSpawnPoints)
                if (pt != null) Gizmos.DrawWireSphere(pt.position, 0.3f);
        if (pillarSpawnPoints != null)
            foreach (var pt in pillarSpawnPoints)
                if (pt != null) Gizmos.DrawWireCube(pt.position, Vector3.one * 0.5f);
    }
}
