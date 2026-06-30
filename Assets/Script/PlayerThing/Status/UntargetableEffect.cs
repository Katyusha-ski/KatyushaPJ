using UnityEngine;

public class UntargetableEffect : StatusEffect
{
    public override StatusEffectType Type => StatusEffectType.Buff;

    public UntargetableEffect(float dur, GameObject targetOJ)
        : base("Untargetable", dur, targetOJ) { }

    public override void OnApply()
    {
        var health = target.GetComponent<Health>();
        if (health != null)
            health.SetInvulnerable(true);
    }

    public override void OnTick() { }

    public override void OnRemove()
    {
        var health = target.GetComponent<Health>();
        if (health != null)
            health.SetInvulnerable(false);
    }
}
