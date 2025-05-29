using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public float cooldown;
    protected float cooldownTimer = 0f;

    public bool CanActivate => cooldownTimer <= 0f;

    public void UpdateCooldown()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public abstract void Activate(GameObject user, int direction);
}
