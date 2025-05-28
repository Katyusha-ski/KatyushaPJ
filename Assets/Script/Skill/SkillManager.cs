using UnityEngine;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    public List<SkillBase> skills = new List<SkillBase>();

    void Update()
    {
        foreach (var skill in skills)
            skill.UpdateSkill();
    }

    public void ActivateSkill(int skillIndex, int direction)
    {
        if (skillIndex >= 0 && skillIndex < skills.Count)
            skills[skillIndex].Activate(direction);
    }
}
