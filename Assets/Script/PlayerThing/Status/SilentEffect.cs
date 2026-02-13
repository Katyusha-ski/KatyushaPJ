using UnityEngine;

public class SilentEffect : StatusEffect
{
    public SilentEffect(float dur, GameObject targetOJ) : base("Silent", dur, targetOJ)
    {

    }
    public override void OnApply()
    {
        var skillInput = target.GetComponent<PlayerSkillInput>();
        if (skillInput != null)
        {
            skillInput.ClearInputBuffer();
        }
    }
    public override void OnTick()
    {
    }
    public override void OnRemove()
    {
    }
}