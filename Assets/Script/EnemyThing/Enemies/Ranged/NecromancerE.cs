using UnityEngine;

public class NecromancerE : EnemyController, IRangedEnemy
{
    [Header("Skill List")]
    public SkillManager skillManager;
    
    private float closeDistance = 3f;
    private float preferredDistance = 5f;

    public float GetCloseDistance() => closeDistance;
    public float GetPreferedDistance() => preferredDistance;

    public override IEnemyState GetAttackState() => new RangedAttackState();

    protected override void InitializeStates()
    {
        base.InitializeStates();
        stateCache["Kitting"] = GetKittingState();
        stateCache["Attack"] = GetAttackState();
    }
    public override void ExecuteAttack()
    {
        if (skillManager.skills[0].CanActivate)
        {
            SetAnimatorTrigger("Skill1");
            return;
        }
        if (skillManager.skills[1].CanActivate)
        {
            SetAnimatorTrigger("Skill2");
            return;
        }
    }

    public void CastSkill1()
    {
        skillManager.ActivateSkill(0, direction);
    }

    public void CastSkill2()
    {
        skillManager.ActivateSkill(1, direction);
    }

    public void Heal()
    {
        skillManager.ActivateSkill(2, 0);
    }

    public void Retreat()
    {
        LookAtPlayer();
        rb.linearVelocity = new Vector2(-speed * 0.8f * direction, rb.linearVelocity.y);
    }
    public void ExecuteKitting()
    {
        ExecuteAttack();
        Retreat();
    }
}