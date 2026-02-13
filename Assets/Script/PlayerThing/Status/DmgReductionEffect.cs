using UnityEngine;
public class DmgReductionEffect : StatusEffect
{
    private float reductionAmount;
    public DmgReductionEffect(float dur, GameObject targetOJ, float reductionAmount) : base("Damage Reduction", dur, targetOJ)
    {
        this.reductionAmount = reductionAmount;
    }
    public override void OnApply()
    {
        var health = target.GetComponent<Health>();
        if (health != null)
        {
            health.SetDamageReduction(reductionAmount);
        }
    }
    public override void OnTick()
    {
    }
    public override void OnRemove()
    {   
        var health = target.GetComponent<Health>();
        if (health != null)
        {
            health.SetDamageReduction(0f);
        }
    }
}