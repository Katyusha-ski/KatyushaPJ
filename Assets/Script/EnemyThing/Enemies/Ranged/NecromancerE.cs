using UnityEngine;

public class NecromancerE : EnemyController, IRangedEnemy
{
    [Header("Skill List")]
    public SkillManager skillManager;

    private float closeDistance = 3f;
    private float preferredDistance = 5f;
    private HealState healState;

    public float GetCloseDistance() => closeDistance;
    public float GetPreferedDistance() => preferredDistance;

    public override IEnemyState GetAttackState() => new RangedAttackState();

    protected override void InitializeStates()
    {
        base.InitializeStates();
        stateCache["Kitting"] = GetKittingState();
        stateCache["Attack"] = GetAttackState();
        healState = new HealState(null); // Initialize once
        stateCache["Heal"] = healState;
    }

    void Update()
    {
        // Check heal priority from any state
        if (ShouldHeal() && skillManager.skills[2].CanActivate && !(currentState is HealState))
        {
            healState.SetPreviousState(GetCurrentState() ?? new IdleState());
            ChangeStateByName("Heal");
            return;
        }

        // Normal state update
        if (currentState != null)
        {
            currentState.OnUpdate(this);
        }
    }

    public override void ExecuteAttack()
    {
        // Try to cast skill 1
        if (skillManager.skills[0].CanActivate)
        {
            SetAnimatorTrigger("Skill1");
            return;
        }

        // Try to cast skill 2
        if (skillManager.skills[1].CanActivate)
        {
            SetAnimatorTrigger("Skill2");
            return;
        }
    }

    private bool ShouldHeal()
    {
        var health = GetComponent<Health>();
        if (health == null) return false;
        return health.CurrentHealth < health.MaxHealth * 0.5f;
    }

    public void CastSkill1()
    {
        skillManager.ActivateSkill(0, direction);
    }

    public void CastSkill2()
    {
        skillManager.ActivateSkill(1, direction);
    }

    public void CastHeal()
    {
        skillManager.ActivateSkill(2, 0);
    }

    public void Retreat()
    {
        LookAtPlayer();
        rb.linearVelocity = new Vector2(-characterStats.MovementSpeed * 0.8f * direction, rb.linearVelocity.y);
    }
    public void ExecuteKitting()
    {
        ExecuteAttack();
        Retreat();
    }
}