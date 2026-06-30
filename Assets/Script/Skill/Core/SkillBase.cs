using UnityEngine;

// This file also includes Base for Attack skill and Buff skill
public abstract class SkillBase : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public SkillType skillType;
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
        if (cooldownTimer > 0f)
        {
            float speedMultiplier = 1f;
            if (characterStats != null)
            {
                float cdr = Mathf.Clamp01(characterStats.CDR / 100f);
                speedMultiplier = 1f / (1f - cdr);
            }
            cooldownTimer -= Time.deltaTime * speedMultiplier;
        }
    }

    public virtual void Activate(GameObject user, int direction)
    {
        cooldownTimer = cooldown;
    }
}
