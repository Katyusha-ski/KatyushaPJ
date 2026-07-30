using UnityEngine;

public class TremorHailstormSkill : IEnvironmentSkill
{
    /// <summary>Slow percentage per phase. Phase1: 10% light, Phase4: 60% heavy. Architect: chua chot so lieu.</summary>
    private float[] slowPercentByPhase = new float[] { 10f, 25f, 40f, 60f };

    /// <summary>Slow duration in seconds per phase. Architect: chua chot so lieu.</summary>
    private float[] slowDurationByPhase = new float[] { 1f, 1.5f, 2f, 3f };

    /// <summary>Chip damage per hailstone hit. Phase1-2: 0 (no hail). Phase3: Mức 2. Phase4: Mức 3. Architect: chua chot so lieu.</summary>
    private int[] chipDamageByPhase = new int[] { 0, 0, 3, 8 };

    /// <summary>Hailstone spawn interval per phase. 0 = disabled. Phase4: bullet-hell rapid. Architect: chua chot so lieu.</summary>
    private float[] hailIntervalByPhase = new float[] { 0f, 0f, 2f, 0.8f };

    /// <summary>Tremor (slow AoE) interval per phase. Architect: chua chot so lieu.</summary>
    private float[] tremorIntervalByPhase = new float[] { 5f, 4f, 3f, 2f };

    // --- Runtime ---
    private GolemController.GolemPhase currentPhase = GolemController.GolemPhase.Phase1;
    private bool enabled = true;
    private float tremorTimer;
    private float hailTimer;

    public void Tick(float dt)
    {
        if (!enabled) return;

        tremorTimer -= dt;
        if (tremorTimer <= 0f)
        {
            tremorTimer = tremorIntervalByPhase[(int)currentPhase];
            EmitTremor();
        }

        if ((int)currentPhase >= 2)
        {
            hailTimer -= dt;
            if (hailTimer <= 0f)
            {
                hailTimer = hailIntervalByPhase[(int)currentPhase];
                SpawnHailstone();
            }
        }
    }

    private void EmitTremor()
    {
        float slowPct = slowPercentByPhase[(int)currentPhase];
        float slowDur = slowDurationByPhase[(int)currentPhase];
        // TODO: chờ prefab — spawn tremor AoE indicator, apply StatModifierEffect(Slow) on enter
        // Phase1: 0 dmg, light slow
        // Phase2: 0 dmg, stronger + longer slow
        // Phase3-4: same slow, but also spawns hailstones
    }

    private void SpawnHailstone()
    {
        int damage = chipDamageByPhase[(int)currentPhase];
        // TODO: chờ prefab — spawn falling debris at random positions above player
        // Phase3: small debris, chip damage
        // Phase4: larger debris, bullet-hell density, combined with heavy slow
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
