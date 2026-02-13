using UnityEngine;

public class StunEffect : StatusEffect
{
    public StunEffect(float dur, GameObject targetOJ) : base("Stun", dur, targetOJ)
    {

    }

    public override void OnApply()
    {
        var movement = target.GetComponent<PlayerMovementController>();
        if (movement != null)
        {
            movement.Stop();
        }
        var skillInput = target.GetComponent<PlayerSkillInput>();
        if (skillInput != null) {
            skillInput.ClearInputBuffer();
        }

        // thêm những logic khác nếu cần(như animation, particle, sound...)
    }
    public override void OnTick()
    {

    }

    public override void OnRemove()
    {
        
    }
}