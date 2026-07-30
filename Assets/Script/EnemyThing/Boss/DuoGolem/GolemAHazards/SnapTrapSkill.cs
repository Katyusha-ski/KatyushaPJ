using UnityEngine;

public class SnapTrapSkill : IEnvironmentSkill
{
    // --- Constants ---
    /// <summary>WaitTime before walls slam — FIXED at 1.5s across all phases per GDD to build muscle memory.</summary>
    private const float WAIT_TIME = 1.5f;

    /// <summary>Wall slam speed per phase. Higher = faster trap closure. Architect: chua chot so lieu.</summary>
    private float[] slamSpeedByPhase = new float[] { 2f, 3f, 5f, 8f };

    /// <summary>Damage per phase. Phase1-2: 0 (CC only). Phase3: Mức 2. Phase4: Mức 3. Architect: chua chot so lieu.</summary>
    private int[] damageByPhase = new int[] { 0, 0, 15, 25 };

    /// <summary>Cooldown between trap activations per phase. Architect: chua chot so lieu.</summary>
    private float[] cooldownByPhase = new float[] { 10f, 8f, 6f, 4f };

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
        // TODO: chờ prefab — instantiate wall pair at random/player position
        // Phase1-2: 0 dmg, thin walls block path randomly
        // Phase3: dmg Mức 2, two walls create dead end, SlamSpeed applies
        //   — walls must collide with Player layer, not bypassed by DashSkill layer mask
        //   — Player MUST jump (see IMPORTANT block above)
        // Phase4: dmg Mức 3, SlamSpeed max, hit applies RootEffect
        //   — if player airborne → set Rigidbody2D.linearVelocity.y = -strongPull
        //     (see Architect note: direct velocity, NOT AddForce)
        float slamSpeed = slamSpeedByPhase[(int)currentPhase];
        int damage = damageByPhase[(int)currentPhase];

        if (currentPhase >= GolemController.GolemPhase.Phase3)
        {
            // Timer-based: walls ALWAYS slam after WAIT_TIME even if player already escaped
            // TODO: instantiate wall prefab pair (collider blocks Player layer),
            //   set slamSpeed, set damage
            // TODO: on trigger enter → apply RootEffect (Phase4)
            // TODO: if player airborne → set Rigidbody2D.linearVelocity.y = -strongPull
            //   (GDD: kéo giật xuống đất). Dùng direct velocity set, KHÔNG dùng AddForce
            //   — AddForce bị trượt timing do quán tính nhảy lơ lửng của player.
            //   Architect confirmed: ưu tiên linearVelocity set cứng trục Y.
        }
        else
        {
            // TODO: instantiate thin blocking walls at random positions (no damage)
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
}
