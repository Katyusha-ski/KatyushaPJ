using UnityEngine;

public class StoneSpikeStabSkill : IEnvironmentSkill
{
    /// <summary>Delay between telegraph appearing and damage. Phase1: long (reactable), Phase4: extremely short. Architect: chua chot so lieu.</summary>
    private float[] delayByPhase = new float[] { 1.5f, 1.2f, 0.8f, 0.4f };

    /// <summary>Damage radius per phase. Phase1-2: small AoE. Phase3-4: huge AoE. Architect: chua chot so lieu.</summary>
    private float[] aoeRadiusByPhase = new float[] { 1.5f, 2f, 3f, 4f };

    /// <summary>Damage per phase. Phase1-2: Mức 1-2. Phase3-4: Mức 3→Max 4. Architect: chua chot so lieu.</summary>
    private int[] damageByPhase = new int[] { 5, 10, 20, 30 };

    /// <summary>Cooldown between activations per phase. Phase4: relentless, forces constant dashing. Architect: chua chot so lieu.</summary>
    private float[] cooldownByPhase = new float[] { 6f, 5f, 3f, 1.5f };

    // --- Runtime ---
    private GolemController.GolemPhase currentPhase = GolemController.GolemPhase.Phase1;
    private bool enabled = true;
    private float cooldownTimer;

    public void Tick(float dt)
    {
        if (!enabled) return;

        cooldownTimer -= dt;
        if (cooldownTimer > 0f) return;

        cooldownTimer = cooldownByPhase[(int)currentPhase];
        TelegraphSpikeStab();
    }

    private void TelegraphSpikeStab()
    {
        float delay = delayByPhase[(int)currentPhase];
        float radius = aoeRadiusByPhase[(int)currentPhase];
        int damage = damageByPhase[(int)currentPhase];

        // Position is locked at telegraph start — NO tracking/homing
        // TODO: chờ prefab — spawn telegraph indicator at Player position at this moment
        // TODO: after delay → deal AoE damage at locked position, then destroy
        // Phase1-2: small AoE, long delay
        // Phase3-4: huge AoE, very short delay, relentless cooldown
        // Absolutely NO tracking after telegraph appears.
    }

    public void SetPhase(GolemController.GolemPhase phase)
    {
        currentPhase = phase;
    }

    public void SetEnabled(bool e)
    {
        enabled = e;
    }
}
