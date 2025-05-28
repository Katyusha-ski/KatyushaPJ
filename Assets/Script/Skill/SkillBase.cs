using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    public float cooldown = 1f;
    protected float cooldownTimer = 0f;

    public bool CanActivate => cooldownTimer <= 0f;

    public virtual void UpdateSkill()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    public abstract void Activate(int direction);
}
