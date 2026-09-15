using UnityEngine;

public class RollingStoneSkill : IEnvironmentSkill
{
    private readonly GameObject stonePrefab;
    private readonly float groundY;
    /// <summary>Cooldown between boulder spawns per phase. Phase1: slow, Phase4: bullet-hell rapid. Architect: chua chot so lieu.</summary>
    private float[] cooldownByPhase = new float[] { 8f, 6f, 4f, 3f };

    /// <summary>Damage per phase. Phase1-2: Mức 1-2 (chip). Phase3-4: Mức 3→Max 4. Architect: chua chot so lieu.</summary>
    private int[] damageByPhase = new int[] { 5, 10, 20, 30 };

    /// <summary>Hitbox scale multiplier per phase. Higher = larger boulder. Architect: chua chot so lieu.</summary>
    private float[] hitboxScaleByPhase = new float[] { 1f, 1f, 1.5f, 2f };

    // --- Runtime ---
    private GolemController.GolemPhase currentPhase = GolemController.GolemPhase.Phase1;
    private bool enabled = true;
    private float cooldownTimer;

    public RollingStoneSkill(GameObject stonePrefab, float groundY)
    {
        this.stonePrefab = stonePrefab;
        this.groundY = groundY;
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
        Vector3 spawnPosition = new Vector3(player.position.x - spawnOffset, groundY, player.position.z);
        GameObject stoneObject = Object.Instantiate(stonePrefab, spawnPosition, Quaternion.identity);
        stoneObject.transform.localScale *= scale;

        // The collider bounds can still contain the prefab's original scale
        // during this frame. Sync transforms before calculating the scaled
        // bottom offset, otherwise larger stones spawn too low.
        Physics2D.SyncTransforms();

        Collider2D collider = stoneObject.GetComponent<Collider2D>();
        if (collider != null)
            stoneObject.transform.position += Vector3.up * (groundY - collider.bounds.min.y);
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
        if (e && !enabled)
            cooldownTimer = cooldownByPhase[(int)currentPhase];
        enabled = e;
    }

    public void Cleanup()
    {
        enabled = false;
    }
}
