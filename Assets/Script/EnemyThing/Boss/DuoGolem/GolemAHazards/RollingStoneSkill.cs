using UnityEngine;

public class RollingStoneSkill : IEnvironmentSkill
{
    private readonly GameObject stonePrefab;
    /// <summary>Cooldown between boulder spawns per phase. Phase1: slow, Phase4: bullet-hell rapid. Architect: chua chot so lieu.</summary>
    private float[] cooldownByPhase = new float[] { 8f, 6f, 3f, 1.5f };

    /// <summary>Damage per phase. Phase1-2: Mức 1-2 (chip). Phase3-4: Mức 3→Max 4. Architect: chua chot so lieu.</summary>
    private int[] damageByPhase = new int[] { 5, 10, 20, 30 };

    /// <summary>Hitbox scale multiplier per phase. Higher = larger boulder. Architect: chua chot so lieu.</summary>
    private float[] hitboxScaleByPhase = new float[] { 1f, 1f, 1.5f, 2f };

    // --- Runtime ---
    private GolemController.GolemPhase currentPhase = GolemController.GolemPhase.Phase1;
    private bool enabled = true;
    private float cooldownTimer;

    public RollingStoneSkill(GameObject stonePrefab)
    {
        this.stonePrefab = stonePrefab;
    }

    public void Tick(float dt)
    {
        if (!enabled) return;

        cooldownTimer -= dt;
        if (cooldownTimer > 0f) return;

        cooldownTimer = cooldownByPhase[(int)currentPhase];
        SpawnRollingStone();
    }

    private void SpawnRollingStone()
    {
        int damage = damageByPhase[(int)currentPhase];
        float scale = hitboxScaleByPhase[(int)currentPhase];
        Transform player = PlayerManager.Instance != null
            ? PlayerManager.Instance.PlayerTransform
            : null;
        if (stonePrefab == null || player == null)
            return;

        const float spawnOffset = 8f;
        Vector3 spawnPosition = player.position + Vector3.left * spawnOffset;
        GameObject stoneObject = Object.Instantiate(stonePrefab, spawnPosition, Quaternion.identity);
        stoneObject.transform.localScale *= scale;
        IProjectilePref projectile = stoneObject.GetComponent<IProjectilePref>();
        if (projectile != null)
        {
            projectile.SetDamage(damage);
            projectile.SetDirection(1);
            projectile.SetProjectileConfig(new ProjectileConfig
            {
                speed = 7f,
                pierceCount = 0
            });
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
