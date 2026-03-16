using UnityEngine;

public class IronBodyEffect : StatusEffect // Can't be CCed
{
    public override StatusEffectType Type => StatusEffectType.Buff;

    public IronBodyEffect(float dur, GameObject targetOJ) : base("Iron Body", dur, targetOJ)
    {
    }
    public override void OnApply()
    {

    }
    public override void OnTick()
    {
    }
    public override void OnRemove()
    {

    }

}