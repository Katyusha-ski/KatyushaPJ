using UnityEngine;

public abstract class DirectDmgSkillBase : SkillBase
{
    public float baseDamage;

    protected float CalculateFinalDamage()
    {
        float finalDamage = baseDamage;

        if (characterStats != null)
        {
            float skillAmpMultiplier = 1 + Mathf.Clamp01(characterStats.SkillAmp / 100f);
            finalDamage = baseDamage * skillAmpMultiplier;
        }

        return finalDamage;
    }
}
