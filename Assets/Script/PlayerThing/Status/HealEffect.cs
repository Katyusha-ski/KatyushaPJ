using UnityEngine;

public class HealEffect : StatusEffect
{
    private int healAmount;
    private int tickAmount;
    private int tickInterval;
    private float timeSinceLastTick;
    public override StatusEffectType Type => StatusEffectType.Buff;

    public HealEffect(float dur, GameObject targetOJ, float healAmount, float tickInterval, float tickAmount) : base("Heal", dur, targetOJ)
    {
        this.healAmount = (int)healAmount;
        this.tickInterval = (int)tickInterval;
        this.tickAmount = (int)tickAmount;
        this.timeSinceLastTick = 0f;
    }

    public override void OnApply()
    {
        var health = target.GetComponent<Health>();
        if (health != null)
        {
            health.Heal(healAmount);
        }
    }

    public override void OnTick()
    {
        timeSinceLastTick += Time.deltaTime;
        if (timeSinceLastTick >= tickInterval)
        {
            var health = target.GetComponent<Health>();
            if (health != null)
            {
                health.Heal(tickAmount);
            }
            timeSinceLastTick = 0f;
        }
    }
    public override void OnRemove()
    {
    }
}