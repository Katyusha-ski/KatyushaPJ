using UnityEngine;
using System.Collections.Generic;

public class SpeedModifierEffect : StatusEffect
{
    private float slowAmount;
    public SpeedModifierEffect(float dur, GameObject targetOJ, float slowAmount) : base("Slow", dur, targetOJ)
    {
        this.slowAmount = slowAmount;
    }
    public override void OnApply()
    {
        var movement = target.GetComponent<PlayerMovementController>();

        if (movement != null)
        {
            movement.SpeedMultiplier(1f - slowAmount);
        }
    }
    public override void OnTick()
    {
    }

    public override void OnRemove()
    {
        var movement = target.GetComponent<PlayerMovementController>();
        if (movement != null)
        {
            movement.ResetSpeedMultiplier();  
        }
    }
}