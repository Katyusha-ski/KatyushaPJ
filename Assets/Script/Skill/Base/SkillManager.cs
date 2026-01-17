using UnityEngine;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    public List<SkillBase> skills = new List<SkillBase>();

    public List<SkillBase> GetSkills()
    {
        return skills;
    }

    void Update()
    {
        foreach (var skill in skills)
        {
            if (skill != null)
                skill.UpdateCooldown();
        }
    }

    public void ActivateSkill(int skillIndex, int direction)
    {
        if (skillIndex >= 0 && skillIndex < skills.Count)
            skills[skillIndex].Activate(gameObject, direction);
    }
}
