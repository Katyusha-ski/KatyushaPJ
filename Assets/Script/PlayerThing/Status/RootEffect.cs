using UnityEngine;

public class RootEffect : StatusEffect
{
    public RootEffect(float dur, GameObject targetOJ) : base("Root", dur, targetOJ)
    {

    }

    public override void OnApply()
    {
        var movement = target.GetComponent<PlayerMovementController>();

        if (movement != null)
        {
            movement.Stop();
        }

    }
    public override void OnTick()
    {

    }

    public override void OnRemove()
    {

    }
}