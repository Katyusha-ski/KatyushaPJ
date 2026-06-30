using UnityEngine;

public class NecromancerE : EnemyController, IEnemyRanged
{
    [Header("Skill List")]
    public SkillManager skillManager;
    [SerializeField] private int skill1Index = 0;
    [SerializeField] private int skill2Index = 1;
    [SerializeField] private int healSkillIndex = 2;

    [SerializeField] private float closeDistance = 3f;
    [SerializeField] private float preferredDistance = 5f;

    private HealState healState;

    public float GetCloseDistance() => closeDistance;
    public float GetPreferredDistance() => preferredDistance;

    public void Kitting()
    {
        if (IsAttackReady())
        {
            ExecuteAttack();
            RecordAttack();
        }
        RetreatFromPlayer();
    }

    public override IEnemyState GetAttackState() => new RangedAttackState();
    public override IEnemyState GetKittingState() => new KittingState();

    protected override void CacheStates()
    {
        base.CacheStates();
        stateCache["Kitting"] = GetKittingState();
        stateCache["Attack"] = GetAttackState();
        healState = new HealState();
        stateCache["Heal"] = healState;
    }

    protected override void Start()
    {
        base.Start();
        ValidateSkillSetup();
    }

    protected override void Update()
    {
        if (ShouldHeal() && CanUseSkill(healSkillIndex) && !(currentState is HealState))
        {
            healState.SetPreviousState(currentState);
            SwitchTo("Heal");
            return;
        }

        base.Update();
    }

    public override void ExecuteAttack()
    {
        if (CanUseSkill(skill1Index))
        {
            PlayAnimTrigger("Skill1");
            return;
        }

        if (CanUseSkill(skill2Index))
        {
            PlayAnimTrigger("Skill2");
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
        if (CanUseSkill(skill1Index))
            skillManager.ActivateSkill(skill1Index, GetDirection());
    }

    public void CastSkill2()
    {
        if (CanUseSkill(skill2Index))
            skillManager.ActivateSkill(skill2Index, GetDirection());
    }

    public void CastHeal()
    {
        if (CanUseSkill(healSkillIndex))
            skillManager.ActivateSkill(healSkillIndex, 0);
    }

    private bool CanUseSkill(int index)
    {
        return skillManager != null
            && skillManager.skills != null
            && index >= 0
            && index < skillManager.skills.Count
            && skillManager.skills[index] != null
            && skillManager.skills[index].CanActivate;
    }

    private void ValidateSkillSetup()
    {
        if (skillManager == null)
        {
            Debug.LogError($"{name} missing SkillManager.", this);
            return;
        }

        ValidateSkillIndex(skill1Index, nameof(skill1Index));
        ValidateSkillIndex(skill2Index, nameof(skill2Index));
        ValidateSkillIndex(healSkillIndex, nameof(healSkillIndex));
    }

    private void ValidateSkillIndex(int index, string fieldName)
    {
        if (skillManager.skills == null)
        {
            Debug.LogError($"{name} SkillManager has a null skills list.", this);
            return;
        }

        if (index < 0 || index >= skillManager.skills.Count)
        {
            Debug.LogError($"{name} {fieldName} is out of range. Index: {index}, skill count: {skillManager.skills.Count}.", this);
            return;
        }

        if (skillManager.skills[index] == null)
        {
            Debug.LogError($"{name} {fieldName} points to an empty skill slot at index {index}.", this);
        }
    }
}
