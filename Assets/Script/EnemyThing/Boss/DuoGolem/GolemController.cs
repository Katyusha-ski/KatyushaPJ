using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class GolemController : EnemyController
{
    [System.Serializable]
    public enum GolemPhase
    {
        Phase1 = 0,
        Phase2 = 1,
        Phase3 = 2,
        Phase4 = 3
    }

    [Header("Duo Golem Settings")]
    [SerializeField] protected GolemController partnerGolem;
    [SerializeField] protected float groundY = 0f;

    protected ArenaHazardController myHazards;
    private BossHealthBarUI bossHealthBar;

    /// <summary>HP% thresholds that trigger phase transitions. Index order: Phase1 (>[0]), Phase2 (>[1]), Phase3 (>[2]), Phase4 (<=[2]). Architect: adjust for difficulty tuning.</summary>
    [Header("Phase Thresholds (%)")]
    [SerializeField] protected float[] phaseThresholds = new float[] { 75f, 50f, 25f };

    /// <summary>Movement speed multiplier per phase. Index = (int)GolemPhase. Architect: chua chot so lieu.</summary>
    [Header("Phase Speed Multipliers")]
    [SerializeField] protected float[] moveSpeedMultipliers = new float[] { 1f, 1.2f, 1.5f, 2f };

    /// <summary>Attack animation speed multiplier per phase (higher = faster punch). Inversely affects GolemAttackState.animDuration. Architect: chua chot so lieu.</summary>
    // Base punch animation is reduced by 25% because the source clip is already fast.
    // Phase scaling is preserved on top of that reduced baseline.
    [SerializeField] protected float[] attackSpeedMultipliers = new float[] { 0.75f, 0.8625f, 1.0125f, 1.2f };

    /// <summary>Punch damage — locked at Mức 3 across ALL phases per GDD. Only speed scales with phase.</summary>
    [Header("Punch Damage (Muc 3, constant across all phases)")]
    [SerializeField] protected int punchDamage = 25;

    [Header("Normal Attack Hitbox")]
    [Tooltip("Local-space offset from the Golem pivot to the center of the punch hitbox. X is mirrored with the facing direction.")]
    [SerializeField] private Vector2 attackHitboxOffset = new Vector2(1.5f, 0f);
    [SerializeField] private Vector2 attackHitboxSize = new Vector2(3f, 2f);
    [SerializeField] private LayerMask playerLayer;

    protected GolemPhase currentPhase = GolemPhase.Phase1;
    protected Health health;
    protected bool pendingPhaseRecalc;
    protected bool isChanneling;

    public GolemPhase CurrentPhase => currentPhase;
    public bool IsChanneling => isChanneling;
    public GolemController PartnerGolem => partnerGolem;
    public ArenaHazardController MyHazards => myHazards;
    public float GroundY => groundY;
    public float AttackAnimationSpeed => attackSpeedMultipliers[(int)currentPhase];
    public Vector2 AttackHitboxOffset => attackHitboxOffset;
    private bool encounterStarted;

    public Vector2 GetAttackHitboxCenter()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        bool isFlippedX = spriteRenderer != null && spriteRenderer.flipX;
        Vector2 offset = isFlippedX
            ? attackHitboxOffset * -1f
            : attackHitboxOffset;

        return (Vector2)transform.position + offset;
    }


    public void ShowStateEffect(Color color)
    {
        Transform effect = transform.Find("GolemStateEffect");
        if (effect == null) return;

        MeshRenderer renderer = effect.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Material material = renderer.material;
            material.SetColor("_ColorA", color);
            material.SetColor("_ColorB", color);
        }

        effect.gameObject.SetActive(true);
    }

    public void HideStateEffect()
    {
        Transform effect = transform.Find("GolemStateEffect");
        if (effect != null)
            effect.gameObject.SetActive(false);
    }

    public event System.Action OnDuoBossDefeated;
    private bool duoBossDefeatHandled;
    protected virtual bool IsVictoryOwner => false;

    protected override void Start()
    {
        // Prepare subclass dependencies before caching states. Calling base.Start()
        // first would invoke GetDieState() while the Golem-specific states are
        // still missing from the cache.
        characterStats = GetComponent<CharacterStats>();
        health = GetComponent<Health>();
        bossHealthBar = GetComponentInChildren<BossHealthBarUI>(true);
        bossHealthBar?.Hide();
        if (health != null)
        {
            health.OnDamaged += OnHealthDamaged;
        }

        IEnvironmentSkill ccSkill = CreateCCSkill();
        IEnvironmentSkill dmgSkill = CreateDmgSkill();
        myHazards = new ArenaHazardController(ccSkill, dmgSkill);
        myHazards.SetEnabled(false);

        pendingPhaseRecalc = false;
        isChanneling = false;

        InitializeEnemyController();

        if (characterStats != null)
        {
            characterStats.SetBaseMovementSpeed(moveSpeedMultipliers[0]);
            characterStats.SetBaseAttack(punchDamage);
        }
    }

    protected virtual IEnvironmentSkill CreateCCSkill() { return null; }
    protected virtual IEnvironmentSkill CreateDmgSkill() { return null; }

    public override void BeginEncounter()
    {
        base.BeginEncounter();
        encounterStarted = true;
        bossHealthBar?.SetBoss(health);
        myHazards?.SetEnabled(true);
        SwitchTo("Pursuit");
    }

    private void OnHealthDamaged(int damageAmount)
    {
        pendingPhaseRecalc = true;
    }

    protected override void Update()
    {
        if (pendingPhaseRecalc && health != null)
        {
            pendingPhaseRecalc = false;
            RecalculatePhase();
        }

        if (!encounterStarted)
        {
            movement?.Stop();
            return;
        }

        if (myHazards != null)
        {
            myHazards.Tick(Time.deltaTime);
        }

        base.Update();
    }

    public virtual void RecalculatePhase()
    {
        if (health == null) return;

        float hpPercent = (float)health.CurrentHealth / health.MaxHealth * 100f;
        GolemPhase newPhase;

        if (hpPercent > phaseThresholds[0])
            newPhase = GolemPhase.Phase1;
        else if (hpPercent > phaseThresholds[1])
            newPhase = GolemPhase.Phase2;
        else if (hpPercent > phaseThresholds[2])
            newPhase = GolemPhase.Phase3;
        else
            newPhase = GolemPhase.Phase4;

        if (newPhase != currentPhase)
        {
            currentPhase = newPhase;
            OnPhaseChanged(currentPhase);
        }
    }

    protected virtual void OnPhaseChanged(GolemPhase newPhase)
    {
        int phaseIndex = (int)newPhase;

        if (characterStats != null)
        {
            characterStats.SetBaseMovementSpeed(moveSpeedMultipliers[phaseIndex]);
            characterStats.SetBaseAttack(punchDamage);
        }

        stateCache["Attack"] = GetAttackState();

        if (myHazards != null && !(currentState is RevivalChannelingState))
        {
            myHazards.SetPhase(newPhase);
        }
    }

    public virtual void HandleSelfDown()
    {
        if (GetCurrentState() is ParalyzedState || GetCurrentState() is RevivalChannelingState)
            return;

        if (partnerGolem == null)
        {
            Debug.LogWarning($"{name} has no partner Golem.", this);
            return;
        }

        IEnemyState partnerState = partnerGolem.GetCurrentState();

        if (partnerState is ParalyzedState || partnerState is RevivalChannelingState)
            return;

        partnerGolem.ForceEnterChanneling();
    }

    public virtual void ForceEnterChanneling()
    {
        IEnemyState current = GetCurrentState();
        if (current is RevivalChannelingState || current is ParalyzedState)
            return;
        isChanneling = true;
        SwitchTo("RevivalChanneling");
    }

    public void EndChanneling()
    {
        isChanneling = false;
    }

    public virtual void ReviveWithHP(float hp)
    {
        if (health != null)
        {
            health.Revive((int)hp);
        }

        // Re-enable ALL colliders (body + hurtbox) after revival
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            if (col != null)
                col.enabled = true;
        }

        isChanneling = false;
        RecalculatePhase();
        OnPhaseChanged(currentPhase);

        PlayAnimTrigger("Revive");

        if (myHazards != null)
        {
            myHazards.SetEnabled(true);
            myHazards.SetPhase(currentPhase);
        }

        SwitchTo("Idle");
    }

    public virtual void HandleDuoBossDefeated()
    {
        GolemController victoryOwner = IsVictoryOwner ? this : partnerGolem;

        if (victoryOwner == null) return;

        if (victoryOwner.duoBossDefeatHandled) return;

        if (partnerGolem == null || !(partnerGolem.GetCurrentState() is ParalyzedState))
        {
            return;
        }

        victoryOwner.duoBossDefeatHandled = true;

        myHazards?.Cleanup();
        partnerGolem.myHazards?.Cleanup();

        partnerGolem.SwitchTo("RealDie");
        victoryOwner.OnDuoBossDefeated?.Invoke();
    }

    // --- IEnemyState override ---
    public override IEnemyState GetHurtState(IEnemyState currentState)
    {
        return currentState;
    }

    public override IEnemyState GetDieState()
    {
        IEnemyState partnerState = partnerGolem != null
            ? partnerGolem.GetCurrentState()
            : null;

        // A golem can only enter the temporary downed state if its partner is
        // still able to complete the revival. If the partner is already down,
        // channeling, permanently dead, or missing, there is no revival path.
        bool partnerCannotRevive = partnerGolem == null
            || partnerState is ParalyzedState
            || partnerState is RevivalChannelingState
            || partnerState is GolemFinalDeathState;

        if (isChanneling || partnerCannotRevive)
        {
            if (partnerState is ParalyzedState)
            {
                HandleDuoBossDefeated();
            }

            return stateCache["RealDie"];
        }

        HandleSelfDown();
        return stateCache["Paralyzed"];
    }

    protected override void CacheStates()
    {
        // Do not call base.CacheStates(): it calls virtual GetDieState(), which
        // is unsafe before the Golem-specific states have been created.
        stateCache["Idle"] = GetIdleState();
        stateCache["Patrol"] = GetPatrolState();
        stateCache["Pursuit"] = GetPursuitState();
        stateCache["ReturnToPost"] = GetReturnToPostState();
        stateCache["Attack"] = GetAttackState();
        stateCache["Paralyzed"] = new ParalyzedState(this);
        stateCache["RevivalChanneling"] = new RevivalChannelingState(this);
        stateCache["RealDie"] = new GolemFinalDeathState();
        stateCache["Die"] = stateCache["RealDie"];
    }

    public override IEnemyState GetPursuitState()
    {
        return new GolemPursuitState();
    }

    public override IEnemyState GetAttackState()
    {
        return new GolemAttackState(punchDamage, 1f / attackSpeedMultipliers[(int)currentPhase]);
    }

    public override void HandleEnemyDeath()
    {
        myHazards?.Cleanup();
        base.HandleEnemyDeath();
    }

    // --- Animation Event Stubs (Called from Animator clips) ---
    /// <summary>
    /// Called via Animation Event at the punch impact frame.
    /// Forwards to DealNormalAttackDamage() for damage calculation.
    /// </summary>
    public void Animation_OnPunchHit()
    {
        DealNormalAttackDamage();
    }

    /// <summary>
    /// Called via Animation Event at the end of any attack clip.
    /// Provides a safety net: if the FSM timer fails, force-returns to Pursuit.
    /// </summary>
    public void Animation_OnAttackEnd()
    {
        if (GetCurrentState() is GolemAttackState)
            SwitchTo("Pursuit");
    }

    // --- Animation Event ---
    public override void DealNormalAttackDamage()
    {
        if (playerLayer.value == 0)
            playerLayer = LayerMask.GetMask("Player");

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            GetAttackHitboxCenter(),
            attackHitboxSize,
            0f,
            playerLayer);

        HashSet<Health> damagedTargets = new HashSet<Health>();
        foreach (Collider2D hit in hits)
        {
            Health playerHealth = hit.GetComponentInParent<Health>();
            if (playerHealth == null || !damagedTargets.Add(playerHealth))
                continue;

            if (characterStats != null)
                playerHealth.TakeDamage((int)characterStats.Atk, gameObject);
        }
    }

    private void OnDestroy()
    {
        if (health != null)
            health.OnDamaged -= OnHealthDamaged;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the rectangular punch hitbox around the configurable center.
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(GetAttackHitboxCenter(), attackHitboxSize);

        // Keep the pursuit/attack decision boundary visible separately.
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
