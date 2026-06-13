using UnityEngine;

public class CleanseEffect : StatusEffect
{
    
    public override StatusEffectType Type => StatusEffectType.Buff;

    public CleanseEffect(float dur, GameObject targetOJ) : base("Cleanse", dur, targetOJ)
    {
    }
    public override void OnApply()
    {
        var sec = target.GetComponent<StatusEffectController>();
        sec.CleanseCrowdControl();
    }
    public override void OnTick()
    {
    }
    public override void OnRemove()
    {

    }
}
