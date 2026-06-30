using UnityEngine;

public class GolemE : EnemyController
{
    private const int GolemMagicSkillIndex = 0;
    private const int StoneSpikeSkillIndex = 1;

    [Header("Skill List")]
    public SkillManager skillManager;

    private bool checkAttack1 = false;

    protected override void Start()
    {
        base.Start();
        ValidateSkillSetup();
    }

    public override void Pursue()
    {
        if (CanUseSkill(GolemMagicSkillIndex))
        {
            PlayAnimBool("Run", false);
            PlayAnimTrigger("Attack 2");
            return;
        }
        base.Pursue();
    }

    public override void ExecuteAttack()
    {
        if (CanUseSkill(StoneSpikeSkillIndex))
        {
            PlayAnimTrigger("Attack 3");
            return;
        }

        if (!checkAttack1)
        {
            PlayAnimTrigger("Attack");
            checkAttack1 = true;
        }
        else
        {
            PlayAnimTrigger("Attack 1");
            checkAttack1 = false;
        }
    }

    public void CastGolemMagic()
    {
        if (CanUseSkill(GolemMagicSkillIndex))
            skillManager.ActivateSkill(GolemMagicSkillIndex, GetDirection());
        animationCtrl.ResetTrigger("Attack 2");
    }

    public void CastStoneSpike()
    {
        if (CanUseSkill(StoneSpikeSkillIndex))
            skillManager.ActivateSkill(StoneSpikeSkillIndex, 0);
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

        ValidateSkillIndex(GolemMagicSkillIndex, nameof(GolemMagicSkillIndex));
        ValidateSkillIndex(StoneSpikeSkillIndex, nameof(StoneSpikeSkillIndex));
    }

    private void ValidateSkillIndex(int index, string skillName)
    {
        if (skillManager.skills == null)
        {
            Debug.LogError($"{name} SkillManager has a null skills list.", this);
            return;
        }

        if (index < 0 || index >= skillManager.skills.Count)
        {
            Debug.LogError($"{name} {skillName} is out of range. Index: {index}, skill count: {skillManager.skills.Count}.", this);
            return;
        }

        if (skillManager.skills[index] == null)
        {
            Debug.LogError($"{name} {skillName} points to an empty skill slot at index {index}.", this);
        }
    }
}
