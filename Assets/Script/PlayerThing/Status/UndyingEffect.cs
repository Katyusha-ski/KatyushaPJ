using UnityEngine;

public class UndyingEffect : StatusEffect
{
    public UndyingEffect(float dur, GameObject targetOJ) : base("Undying", dur, targetOJ)
    {
    }

    public override void OnApply()
    {
        var health = target.GetComponent<Health>();
        if (health != null)
        {
            health.SetUnDying(true);
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
            health.SetUnDying(false);
        }
    }
}