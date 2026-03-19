using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    public Sprite icon;
    public float cooldown;
    protected float cooldownTimer = 0f;
    public float CooldownTimer => cooldownTimer;
    public bool CanActivate => cooldownTimer <= 0f;

    public void UpdateCooldown()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public virtual void Activate(GameObject user, int direction)
    {
        cooldownTimer = cooldown;
    }
}
