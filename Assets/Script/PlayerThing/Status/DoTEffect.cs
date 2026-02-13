using UnityEngine;

public class DoTEffect : StatusEffect
{
    private int damagePerTick;
    private float tickInterval;
    private float timeSinceLastTick;
    public DoTEffect(float dur, GameObject targetOJ, int damagePerTick, float tickInterval) : base("Damage Over Time", dur, targetOJ)
    {
        this.damagePerTick = damagePerTick;
        this.tickInterval = tickInterval;
        this.timeSinceLastTick = 0f;
    }
    public override void OnApply()
    {
        // Initial application logic if needed
    }
    public override void OnTick()
    {
        timeSinceLastTick += Time.deltaTime;
        if (timeSinceLastTick >= tickInterval)
        {
            var health = target.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damagePerTick);
            }
            timeSinceLastTick = 0f;
        }
    }
    public override void OnRemove()
    {
        // Cleanup logic if needed
    }
}