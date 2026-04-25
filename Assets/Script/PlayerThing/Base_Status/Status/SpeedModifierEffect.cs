using UnityEngine;

public class SpeedModifierEffect : StatusEffect
{
    private float slowAmount;
    private CharacterStats stats;   
    private StatsModifier appliedModifier;

    public override StatusEffectType Type => StatusEffectType.CrowdControl;

    public SpeedModifierEffect(float dur, GameObject targetOJ, float slowAmount) : base("Slow", dur, targetOJ)
    {
        this.slowAmount = slowAmount;
    }

    public override void OnApply()
    {
        stats = target.GetComponent<CharacterStats>();
        if (stats == null)
        {
            Debug.LogWarning("SpeedModifierEffect: CharacterStats not found on target", target);
            return;
        }

        // Calculate actual slow with CC Resistance
        float actualSlowAmount = CalculateActualSlow(slowAmount, stats.CCRes);

        // Apply multiplicative modifier to movement speed
        appliedModifier = new StatsModifier(-actualSlowAmount, ModifierType.Multiplicative, "SpeedModifierEffect");
        stats.AddMovementSpeedModifier(appliedModifier);
    }

    public override void OnTick()
    {
        // Optionally: Update slow effect every tick (for stacking logic)
    }

    public override void OnRemove()
    {
        if (stats != null && appliedModifier != null)
        {
            stats.RemoveMovementSpeedModifier(appliedModifier);
        }
    }

    private float CalculateActualSlow(float slowAmount, float ccResistance)
    {
        // CC Resistance reduces slow effect
        // Example: 50% slow with 30% CC Res = 50% * (1 - 0.3) = 35% actual slow
        return slowAmount * (1f - ccResistance / 100f);
    }
}