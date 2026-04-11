using UnityEngine;
public class DmgReductionEffect : StatusEffect
{
    private float reductionAmount;
    private StatsModifier dmgRMod;
    public override StatusEffectType Type => StatusEffectType.Buff;
    public DmgReductionEffect(float dur, GameObject targetOJ, float reductionAmount) : base("Damage Reduction", dur, targetOJ)
    {
        this.reductionAmount = reductionAmount;
    }
    public override void OnApply()
    {
        var characterStats = target.GetComponent<CharacterStats>();
        if (characterStats != null)
        {
            dmgRMod = new StatsModifier(reductionAmount, ModifierType.Additive, "DmgReductionEffect");
            characterStats.AddDmgRModifier(dmgRMod);
        }
    }
    public override void OnTick()
    {
    }
    public override void OnRemove()
    {   
        var characterStats = target.GetComponent<CharacterStats>();
        if (characterStats != null && dmgRMod != null)
        {
            characterStats.RemoveDmgRModifier(dmgRMod);
            dmgRMod = null;
        }
    }
}