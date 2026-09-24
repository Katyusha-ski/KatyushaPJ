using UnityEngine;

public class StoneSpikeStabSkill : IEnvironmentSkill
{
    private readonly GameObject spikePrefab;
    private readonly float groundY;
    /// <summary>Delay between telegraph appearing and damage. Phase1: long (reactable), Phase4: extremely short. Architect: chua chot so lieu.</summary>
    private float[] delayByPhase = new float[] { 1.5f, 1.25f, 1.0f, 0.75f };

    /// <summary>Damage radius per phase. Phase1-2: small AoE. Phase3-4: huge AoE. Architect: chua chot so lieu.</summary>
    private float[] aoeRadiusByPhase = new float[] { 1.5f, 1.875f, 1.875f, 2.25f };

    /// <summary>Damage per phase. Phase1-2: Mức 1-2. Phase3-4: Mức 3→Max 4. Architect: chua chot so lieu.</summary>
    private int[] damageByPhase = new int[] { 5, 10, 15, 20 };

    /// <summary>Cooldown between activations per phase. Phase4: relentless, forces constant dashing. Architect: chua chot so lieu.</summary>
    private float[] cooldownByPhase = new float[] { 8f, 6f, 4f, 3f };

    // --- Runtime ---
    private GolemController.GolemPhase currentPhase = GolemController.GolemPhase.Phase1;
    private bool enabled = true;
    private float cooldownTimer;

    public StoneSpikeStabSkill(GameObject spikePrefab, float groundY)
    {
        this.spikePrefab = spikePrefab;
        this.groundY = groundY;
    }

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

        Transform player = PlayerManager.Instance != null
            ? PlayerManager.Instance.PlayerTransform
            : null;
        if (spikePrefab == null || player == null)
            return;

        GameObject spikeObject = Object.Instantiate(
            spikePrefab,
            new Vector3(player.position.x, groundY, player.position.z),
            Quaternion.identity);
        StoneSpikeInstance spike = spikeObject.GetComponent<StoneSpikeInstance>();
        if (spike != null)
        {
            spike.AlignBottomToGround(groundY);
            spike.Initialize(currentPhase, damage, radius, delay);
        }
    }

    public void SetPhase(GolemController.GolemPhase phase)
    {
        currentPhase = phase;
    }

    public void SetEnabled(bool e)
    {
        if (e && !enabled)
            cooldownTimer = cooldownByPhase[(int)currentPhase];
        enabled = e;
    }

    public void Cleanup()
    {
        enabled = false;
    }
}
