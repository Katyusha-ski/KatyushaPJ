using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBossController : EnemyController
{
    [Header("Boss Settings")]
    public SkillManager skillManager;
    [SerializeField] private float hoverHeight = 4f;
    [SerializeField] private float hoverSpeed = 0.8f;
    [SerializeField] private float hoverAmplitude = 1.5f;
    [Header("Spawn Prefabs")]
    [SerializeField] private GameObject batSpherePrefab;
    [SerializeField] private GameObject pillarPrefab;
    [SerializeField] private GameObject hazardZonePrefab;
    [SerializeField] private Transform[] sphereSpawnPoints;
    [SerializeField] private Transform[] pillarSpawnPoints;

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
    private int cachedMaxHP;

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
        ChangeState(stateCache["Hover"]);
    }

    protected override void Update()
    {
        if (isDead) return;
        base.Update();
    }

    private void CacheBossStates()
    {
        stateCache["Hover"] = new BatHoverState();
        stateCache["DropSphere"] = new BatAttackAnimState("Attack1", 1.2f, "Hover");
        stateCache["SpawnDoT"] = new BatAttackAnimState("Attack2", 1.2f, "Hover");
        stateCache["SpawnPillar"] = new BatAttackAnimState("Attack3", 1.5f, "Hover");
        stateCache["Hurt"] = new BatHurtState();
        stateCache["Die"] = new BatDieState();
    }

    // --- Movement (no-ops — boss flies) ---
    public override void Patrol() { }
    public override void Pursue() { }
    public override void RetreatFromPlayer() { }
    public override void ExecuteAttack() { }
    public override void DealNormalAttackDamage() { }

    public override IEnemyState GetHurtState(IEnemyState preState) => stateCache["Hurt"];
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
        if (roll < 0.4f)
            SwitchTo("DropSphere");
        else if (roll < 0.7f)
            SwitchTo("SpawnDoT");
        else
            SwitchTo("SpawnPillar");
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
        if (pillarPrefab == null || pillarSpawnPoints == null || pillarSpawnPoints.Length == 0)
            return;

        Transform pt = pillarSpawnPoints[Random.Range(0, pillarSpawnPoints.Length)];
        GameObject pillar = Instantiate(pillarPrefab, pt.position, Quaternion.identity);
        pillar.GetComponent<Pillar>()?.Init(this);
    }

    public void SpawnDoTZone()
    {
        if (hazardZonePrefab == null || player == null)
            return;

        Vector3 spawnPos = player.position;
        spawnPos.y = transform.position.y - hoverHeight;
        GameObject zone = Instantiate(hazardZonePrefab, spawnPos, Quaternion.identity);
        HazardZone haz = zone.GetComponent<HazardZone>();
        if (haz != null)
            haz.Init(3, 1f, 3f, 5f);
        else
            Destroy(zone, 5f);
    }

    /// <summary>
    /// Optional: gọi từ animation event cuối clip để kết thúc state sớm.
    /// Nếu không dùng, state tự kết thúc theo timer.
    /// </summary>
    public void OnAttackAnimEnd()
    {
        if (currentState is BatAttackAnimState)
            SwitchTo("Hover");
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
