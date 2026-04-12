using UnityEngine;

/// <summary>
/// Base class for skills that spawn prefabs to deal damage (Projectiles, Spawns, etc.)
/// Handles SkillAmp calculation and final damage computation
/// </summary>
public abstract class SpawnDamageSkillBase : SkillBase
{
    public float baseDamage = 10f;
    public GameObject spawnPrefab;

    /// <summary>
    /// Calculate final damage with SkillAmp multiplier
    /// </summary>
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
