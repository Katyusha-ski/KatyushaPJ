using UnityEngine;

public class SnapTrapSkill : IEnvironmentSkill
{
    private readonly GameObject trapPrefab;

    // --- Constants ---
    /// <summary>WaitTime before walls slam — FIXED at 1.5s across all phases per GDD to build muscle memory.</summary>
    private const float WAIT_TIME = 1.5f;

    /// <summary>Wall slam speed per phase. Higher = faster trap closure. Architect: chua chot so lieu.</summary>
    private float[] slamSpeedByPhase = new float[] { 2f, 3f, 5f, 8f };

    /// <summary>Damage per phase. Phase1-2: 0 (CC only). Phase3: Mức 2. Phase4: Mức 3. Architect: chua chot so lieu.</summary>
    private int[] damageByPhase = new int[] { 0, 0, 15, 25 };

    /// <summary>Cooldown between trap activations per phase. Architect: chua chot so lieu.</summary>
    private float[] cooldownByPhase = new float[] { 10f, 8f, 6f, 4f };
    private float[] animationSpeedByPhase = new float[] { 1f, 1f, 1f, 1.5f };

    // --- Runtime ---
    private GolemController.GolemPhase currentPhase = GolemController.GolemPhase.Phase1;
    private bool enabled = true;
    private float cooldownTimer;

    public SnapTrapSkill(GameObject trapPrefab)
    {
        this.trapPrefab = trapPrefab;
    }

    public void Tick(float dt)
    {
        if (!enabled) return;

        cooldownTimer -= dt;
        if (cooldownTimer > 0f) return;

        cooldownTimer = cooldownByPhase[(int)currentPhase];
        SpawnSnapTrap();
    }

    // IMPORTANT — SnapTrap vs DashSkill Collision Matrix:
    // The player's DashSkill runs a coroutine with layer pass-through + Untargetable.
    // For SnapTrap to correctly block the player:
    //   1. TrapWall prefab MUST be assigned to a dedicated physics layer (e.g. "TrapWall").
    //   2. In Unity Physics2D Collision Matrix, this layer must be set to collide with Player
    //      and must NOT be included in DashSkill's pass-through layer mask.
    //   3. If misconfigured, the player can dash through walls, breaking the mechanic.
    // Design intent (GDD): "Bắt buộc nhảy né (Dash vô hiệu)" — player CAN dash but cannot
    // pass through the wall. Only jump clears it.
    private void SpawnSnapTrap()
    {
        Transform player = PlayerManager.Instance != null
            ? PlayerManager.Instance.PlayerTransform
            : null;
        if (trapPrefab == null || player == null)
            return;

        int damage = damageByPhase[(int)currentPhase];
        GameObject trapObject = Object.Instantiate(
            trapPrefab,
            player.position,
            Quaternion.identity);

        SnapTrapInstance trap = trapObject.GetComponent<SnapTrapInstance>();
        if (trap != null)
        {
            trap.Initialize(
                currentPhase,
                damage,
                animationSpeedByPhase[(int)currentPhase],
                WAIT_TIME);
        }
    }

    public void SetPhase(GolemController.GolemPhase phase)
    {
        currentPhase = phase;
    }

    public void SetEnabled(bool e)
    {
        enabled = e;
    }

    public void Cleanup()
    {
        enabled = false;
    }
}
