using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/HealSkill")]
public class HealSkill : SkillBase
{
    public int HealAmount = 5;

    public override void Activate(GameObject user, int direction)
    {
        var health = user.GetComponent<Health>();
        if (health != null)
        {
            health.Heal(HealAmount);
        }
    }

}
