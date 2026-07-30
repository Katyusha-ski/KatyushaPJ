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

    protected ArenaHazardController myHazards;

    /// <summary>HP% thresholds that trigger phase transitions. Index order: Phase1 (>[0]), Phase2 (>[1]), Phase3 (>[2]), Phase4 (<=[2]). Architect: adjust for difficulty tuning.</summary>
    [Header("Phase Thresholds (%)")]
    [SerializeField] protected float[] phaseThresholds = new float[] { 75f, 50f, 25f };

    /// <summary>Movement speed multiplier per phase. Index = (int)GolemPhase. Architect: chua chot so lieu.</summary>
    [Header("Phase Speed Multipliers")]
    [SerializeField] protected float[] moveSpeedMultipliers = new float[] { 1f, 1.2f, 1.5f, 2f };

    /// <summary>Attack animation speed multiplier per phase (higher = faster punch). Inversely affects GolemAttackState.animDuration. Architect: chua chot so lieu.</summary>
    [SerializeField] protected float[] attackSpeedMultipliers = new float[] { 1f, 1.15f, 1.35f, 1.6f };

    /// <summary>Punch damage — locked at Mức 3 across ALL phases per GDD. Only speed scales with phase.</summary>
    [Header("Punch Damage (Muc 3, constant across all phases)")]
    [SerializeField] protected int punchDamage = 25;

    protected GolemPhase currentPhase = GolemPhase.Phase1;
    protected Health health;
    protected bool pendingPhaseRecalc;
    protected bool isChanneling;

    public GolemPhase CurrentPhase => currentPhase;
    public bool IsChanneling => isChanneling;
    public GolemController PartnerGolem => partnerGolem;
    public ArenaHazardController MyHazards => myHazards;

    protected override void Start()
    {
        base.Start();

        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDamaged += OnHealthDamaged;
        }

        if (characterStats != null)
        {
            characterStats.SetBaseMovementSpeed(moveSpeedMultipliers[0]);
            characterStats.SetBaseAttack(punchDamage);
        }

        IEnvironmentSkill ccSkill = CreateCCSkill();
        IEnvironmentSkill dmgSkill = CreateDmgSkill();
        myHazards = new ArenaHazardController(ccSkill, dmgSkill);

        pendingPhaseRecalc = false;
        isChanneling = false;
    }

    protected virtual IEnvironmentSkill CreateCCSkill() { return null; }
    protected virtual IEnvironmentSkill CreateDmgSkill() { return null; }

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
        if (health != null && health.CurrentHealth > 0)
        {
            health.SetHealth(0);
        }

        if (partnerGolem != null)
        {
            IEnemyState partnerState = partnerGolem.GetCurrentState();
            if (partnerState is ParalyzedState || partnerState is RevivalChannelingState)
                return;
            partnerGolem.ForceEnterChanneling();
        }
    }

    public virtual void ForceEnterChanneling()
    {
        IEnemyState current = GetCurrentState();
        if (current is RevivalChannelingState || current is ParalyzedState)
            return;
        isChanneling = true;
        SwitchTo("RevivalChanneling");
    }

    public virtual void ReviveWithHP(float hp)
    {
        if (health != null)
        {
            health.SetHealth((int)hp);
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

        if (myHazards != null)
        {
            myHazards.SetEnabled(true);
            myHazards.SetPhase(currentPhase);
        }

        SwitchTo("Idle");
    }

    public virtual void HandleDuoBossDefeated()
    {
        // Called when one golem dies permanently during channeling
        // and partner is already Paralyzed (0 HP).
        // Self state transition is handled by GetDieState() caller (Health.Die flow).
        // Force partner to permanent death too.
        if (partnerGolem != null && partnerGolem.GetCurrentState() is ParalyzedState)
        {
            partnerGolem.SwitchTo("RealDie");
        }
    }

    // --- IEnemyState override ---
    public override IEnemyState GetHurtState(IEnemyState currentState)
    {
        return currentState;
    }

    public override IEnemyState GetDieState()
    {
        if (isChanneling)
        {
            if (partnerGolem != null && partnerGolem.GetCurrentState() is ParalyzedState)
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
        base.CacheStates();
        stateCache["Attack"] = GetAttackState();
        stateCache["Paralyzed"] = new ParalyzedState(this);
        stateCache["RevivalChanneling"] = new RevivalChannelingState(this);
        stateCache["RealDie"] = new DieState(2f, () => HandleEnemyDeath());
        stateCache["Die"] = stateCache["RealDie"];
    }

    public override IEnemyState GetAttackState()
    {
        return new GolemAttackState(punchDamage, 1f / attackSpeedMultipliers[(int)currentPhase]);
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
        if (player != null && Vector2.Distance(transform.position, player.position) < attackRange)
        {
            var playerHealth = player.GetComponent<Health>();
            if (playerHealth != null && characterStats != null)
            {
                playerHealth.TakeDamage((int)characterStats.Atk, gameObject);
            }
        }
    }
}
