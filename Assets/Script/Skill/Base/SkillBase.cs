using UnityEngine;

// This file also includes Base for Attack skill and Buff skill
public abstract class SkillBase : ScriptableObject
{
    public Sprite icon;
    public float cooldown;
    protected float cooldownTimer = 0f;
    protected CharacterStats characterStats;

    public float CooldownTimer => cooldownTimer;
    public bool CanActivate => cooldownTimer <= 0f;

    public virtual void Initialize(CharacterStats stats)
    {
        characterStats = stats;
    }

    public void UpdateCooldown()
    {
        if (cooldownTimer > 0f && characterStats != null)
        {
            float cdrMultiplier = 1 + Mathf.Clamp01(characterStats.CDR / 100f);
            cooldownTimer -= Time.deltaTime * cdrMultiplier;
        }
    }

    public virtual void Activate(GameObject user, int direction)
    {
        cooldownTimer = cooldown;
    }
}
