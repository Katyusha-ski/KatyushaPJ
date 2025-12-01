using UnityEngine;

public class GolemE : EnemyController
{
    [Header("Skill List")]
    public SkillManager skillManager;

    private bool checkAttack1 = false;

    public override IEnemyState GetAttackState()
    {
        return new GolemAttackState();
    }

    public override IEnemyState GetPursuitState()
    {
        return new GolemPursuitState();
    }

    public override void ExecuteAttack()
    {
        if (skillManager.skills[1].CanActivate)
        {
            animator.SetTrigger("Attack 3");
            return;
        }

        if (!checkAttack1)
        {
            NormalAttack();
            checkAttack1 = true;
        }
        else
        {
            Attack1();
            checkAttack1 = false;
        }
    }


    public void Attack1()
    {
        animator.SetTrigger("Attack 1");
    }

    public void CastGolemMagic()
    {
        skillManager.ActivateSkill(0, direction);
        animator.ResetTrigger("Attack 2");
    }

    public void CastStoneSpike()
    {
        skillManager.ActivateSkill(1, 0);
    }
}
