using UnityEngine;

public class VirtualShieldEffect : StatusEffect
{
    public override StatusEffectType Type => StatusEffectType.Buff;
    private int shieldAmount;
    public VirtualShieldEffect(float dur, GameObject targetOJ, int shieldAmt)
        : base("Virtual Shield", dur, targetOJ)
    {
        shieldAmount = shieldAmt;
    }
    public override void OnApply()
    {
        var health = target.GetComponent<Health>();
        if (health != null)
            health.AddShield(shieldAmount);
    }
    public override void OnTick() { }
    public override void OnRemove()
    {
        var health = target.GetComponent<Health>();
        if (health != null)
            health.SetShield(0);
    }
}