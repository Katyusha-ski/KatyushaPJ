using System.Collections.Generic;
using UnityEngine;

public class VoidBossController : EnemyController
{
    [Header("Boss Settings")]
    [SerializeField] private float meleeRange = 2.5f;

    [Header("Attack Prefabs")]
    [SerializeField] private GameObject voidSpherePrefab;
    [SerializeField] private GameObject ambushTrapPrefab;
    [SerializeField] private GameObject bloodMoonTelegraphPrefab;

    [Header("Skill Cooldowns")]
    [SerializeField] private float skill1Cooldown = 4f;
    [SerializeField] private float skill2Cooldown = 10f;
    [SerializeField] private float bloodMoonCooldown = 45f;

    [Header("Blood Moon Config")]
    [SerializeField] private int bloodMoonWaves = 5;
    [SerializeField] private float bloodMoonWaveInterval = 0.8f;
    [SerializeField] private float bloodMoonSpread = 2.5f;
    [SerializeField] private int bloodMoonPerWave = 5;
    [SerializeField] private float bloodMoonMinSpacing = 1.5f;

    [Header("Hurt Effect")]
    [SerializeField] private SpriteRenderer bossSprite;
    [SerializeField] private Color hurtTint = Color.red;
    [SerializeField] private float hurtFlashDuration = 0.15f;

    [Header("Death")]
    [SerializeField] private GameObject deathVFX;
    [SerializeField] private float dieDuration = 2f;

    private BossHealthBarUI bossHealthBar;
    private Color originalColor;
    private bool isDead;
    private bool isAwake;
    private int cachedMaxHP;

    private const string BLOOD_MOON_POOL_TAG = "BloodMoonTelegraph";

    private float skill1ReadyTime = -Mathf.Infinity;
    private float skill2ReadyTime = -Mathf.Infinity;
    private float bloodMoonReadyTime = -Mathf.Infinity;

    private bool facingLocked;
    private List<GameObject> activeProjectiles = new();
    private List<GameObject> activeTelegraphs = new(); // Pool-managed BloodMoon telegraphs

    public event System.Action OnBossDefeated;

    public float MeleeRange => meleeRange;
    public int BloodMoonWaves => bloodMoonWaves;
    public float BloodMoonWaveInterval => bloodMoonWaveInterval;
    public float BloodMoonSpread => bloodMoonSpread;

    public bool IsSkill1Ready() => Time.time >= skill1ReadyTime;
    public bool IsSkill2Ready() => Time.time >= skill2ReadyTime;
    public bool IsBloodMoonReady() => Time.time >= bloodMoonReadyTime;

    public void UseSkill1()
    {
        skill1ReadyTime = Time.time + skill1Cooldown;
        animationCtrl.PlayRun(false);
        LockFacing();
    }

    public void UseSkill2()
    {
        skill2ReadyTime = Time.time + skill2Cooldown;
        animationCtrl.PlayRun(false);
        LockFacing();
    }

    public void UseBloodMoon()
    {
        bloodMoonReadyTime = Time.time + bloodMoonCooldown;
        animationCtrl.PlayRun(false);
        LockFacing();
    }

    public bool IsFacingLocked => facingLocked;
    public void LockFacing() => facingLocked = true;
    public void UnlockFacing() => facingLocked = false;

    protected override void Start()
    {
        player = PlayerManager.Instance != null ? PlayerManager.Instance.PlayerTransform : null;
        characterStats = GetComponent<CharacterStats>();
        if (characterStats == null)
        {
            Debug.LogError("VoidBoss missing CharacterStats!", this);
            return;
        }

        var rb = GetComponent<Rigidbody2D>();
        var sr = GetComponent<SpriteRenderer>();
        var animator = GetComponent<Animator>();

        movement = new MovementManager(rb, sr, characterStats);
        animationCtrl = new AnimationController(animator);
        stateFactory = null;

        cachedMaxHP = (int)characterStats.MaxHP;

        if (bossSprite != null)
            originalColor = bossSprite.color;

        CacheBossStates();

        isAwake = false;
        animator.Play("Void_Sleep", 0, 0f);
    }

    protected override void Update()
    {
        if (isDead || !isAwake) return;
        base.Update();
    }

    private void CacheBossStates()
    {
        stateCache["VoidIdle"] = new VoidIdleState();
        stateCache["Stomp"] = new GenericAttackState("Stomp", 1.2f, "VoidIdle");
        stateCache["SpikePierce"] = new GenericAttackState("SpikePierce", 1.2f, "VoidIdle");
        stateCache["VoidSphere"] = new GenericAttackState("VoidSphere", 1.0f, "VoidIdle");
        stateCache["AmbushSummon"] = new GenericAttackState("AmbushSummon", 1.0f, "VoidIdle");
        stateCache["Pursuit"] = new VoidPursuitState();
        stateCache["BloodMoon"] = new BloodMoonState();
        stateCache["Hurt"] = new HurtState("VoidIdle", false);
        stateCache["Die"] = new DieState(dieDuration, () => HandleEnemyDeath());
    }

    // --- Super Armor ---
    public override IEnemyState GetHurtState(IEnemyState currentState) => currentState;
    public override IEnemyState GetDieState() => stateCache["Die"];

    // --- Movement no-ops ---
    public override void Patrol() { }
    public override void Pursue() { }
    public override void RetreatFromPlayer() { }
    public override void ExecuteAttack() { }

    // ============================================================
    // AI Helpers
    // ============================================================

    public void PickMeleeAttack()
    {
        animationCtrl.PlayRun(false);
        LockFacing();
        float roll = Random.value;
        if (roll < 0.5f)
            SwitchTo("Stomp");
        else
            SwitchTo("SpikePierce");
    }

    // ============================================================
    // Animation Event Callbacks
    // ============================================================

    public void SpawnVoidSphere()
    {
        if (voidSpherePrefab == null || player == null) return;
        GameObject sphere = Instantiate(voidSpherePrefab, transform.position, Quaternion.identity);
        activeProjectiles.Add(sphere);
    }

    public void SpawnAmbushTrap()
    {
        if (ambushTrapPrefab == null || player == null) return;
        GameObject trap = Instantiate(ambushTrapPrefab, player.position, Quaternion.identity);
        activeProjectiles.Add(trap);
    }

    public void SpawnBloodMoonWave()
    {
        if (bloodMoonTelegraphPrefab == null || player == null) return;

        float a = bloodMoonSpread;
        float b = bloodMoonSpread;
        float minSpacing = bloodMoonMinSpacing;
        List<Vector3> chosen = new();

        for (int attempt = 0; attempt < bloodMoonPerWave * 5; attempt++)
        {
            if (chosen.Count >= bloodMoonPerWave) break;

            Vector3 candidate = new(
                player.position.x + Random.Range(-a, a),
                player.position.y + Random.Range(-b, b),
                0f
            );

            bool tooClose = false;
            foreach (Vector3 p in chosen)
            {
                if (Vector3.Distance(candidate, p) < minSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                chosen.Add(candidate);

                ObjectPool pool = ObjectPool.Instance;
                GameObject t;

                if (pool != null)
                {
                    t = pool.SpawnFromPool(BLOOD_MOON_POOL_TAG, candidate, Quaternion.identity);
                    if (t == null)
                        t = Object.Instantiate(bloodMoonTelegraphPrefab, candidate, Quaternion.identity);
                }
                else
                {
                    t = Object.Instantiate(bloodMoonTelegraphPrefab, candidate, Quaternion.identity);
                }

                activeProjectiles.Add(t);
                activeTelegraphs.Add(t);
            }
        }
    }

    public void OnAttackAnimEnd()
    {
        if (currentState is GenericAttackState)
            SwitchTo("VoidIdle");
    }

    public void OnWakeUpComplete()
    {
        isAwake = true;
        UnlockFacing();
        ChangeState(stateCache["VoidIdle"]);
    }

    // ============================================================
    // Hurt Flash
    // ============================================================

    public void FlashHurt()
    {
        if (bossSprite == null) return;
        StopAllCoroutines();
        StartCoroutine(HurtFlashRoutine());
    }

    private System.Collections.IEnumerator HurtFlashRoutine()
    {
        bossSprite.color = hurtTint;
        yield return new WaitForSeconds(hurtFlashDuration);
        bossSprite.color = originalColor;
    }

    // ============================================================
    // Death
    // ============================================================

    public override void HandleEnemyDeath()
    {
        isDead = true;

        ObjectPool pool = ObjectPool.Instance;

        for (int i = activeTelegraphs.Count - 1; i >= 0; i--)
        {
            if (activeTelegraphs[i] != null)
            {
                if (pool != null)
                    pool.ReturnToPool(activeTelegraphs[i]);
                else
                    Destroy(activeTelegraphs[i]);
            }
        }
        activeTelegraphs.Clear();

        for (int i = activeProjectiles.Count - 1; i >= 0; i--)
        {
            if (activeProjectiles[i] != null)
                Destroy(activeProjectiles[i]);
        }
        activeProjectiles.Clear();

        if (deathVFX != null)
            Instantiate(deathVFX, transform.position, Quaternion.identity);
        bossHealthBar?.Hide();
        OnBossDefeated?.Invoke();

        Health h = GetComponent<Health>();
        if (h != null && h.lootManager != null)
            h.lootManager.SpawnLoot();
    }

    public void SetHealthBar(BossHealthBarUI bar) => bossHealthBar = bar;
}
