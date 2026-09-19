using System.Collections.Generic;
using UnityEngine;

public class TremorHailstormSkill : IEnvironmentSkill
{
    private const string HAILSTONE_POOL_TAG = "DuoGolem_Hailstone";
    private readonly GameObject hailstonePrefab;
    private readonly float groundY;
    /// <summary>Slow percentage per phase. Phase1: 10% light, Phase4: 60% heavy. Architect: chua chot so lieu.</summary>
    private float[] slowPercentByPhase = new float[] { 10f, 25f, 25f, 25f };

    /// <summary>Slow duration in seconds per phase. Architect: chua chot so lieu.</summary>
    private float[] slowDurationByPhase = new float[] { 1f, 1.5f, 2f, 3f };

    /// <summary>Chip damage per hailstone hit. Phase1-2: 0 (no hail). Phase3: Mức 2. Phase4: Mức 3. Architect: chua chot so lieu.</summary>
    private int[] chipDamageByPhase = new int[] { 0, 0, 3, 8 };

    /// <summary>Hailstone spawn interval per phase. 0 = disabled. Phase4: bullet-hell rapid.</summary>
    private readonly float[] hailIntervalByPhase;
    private readonly float hailSpawnHalfWidth;
    private readonly float hailSpawnHeight;

    /// <summary>Tremor (slow AoE) interval per phase. Architect: chua chot so lieu.</summary>
    private float[] tremorIntervalByPhase = new float[] { 5f, 4f, 3f, 2f };

    // --- Runtime ---
    private GolemController.GolemPhase currentPhase = GolemController.GolemPhase.Phase1;
    private bool enabled = true;
    private float tremorTimer;
    private float hailTimer;
    private const float INITIAL_TREMOR_DELAY = 10f;

    public TremorHailstormSkill(
        GameObject hailstonePrefab,
        float groundY,
        float[] hailSpawnIntervals,
        float hailSpawnHalfWidth,
        float hailSpawnHeight)
    {
        this.hailstonePrefab = hailstonePrefab;
        this.groundY = groundY;
        this.hailIntervalByPhase = hailSpawnIntervals != null && hailSpawnIntervals.Length >= 4
            ? (float[])hailSpawnIntervals.Clone()
            : new float[] { 0f, 0f, 1.5f, 0.8f };
        this.hailSpawnHalfWidth = Mathf.Max(0f, hailSpawnHalfWidth);
        this.hailSpawnHeight = Mathf.Max(0f, hailSpawnHeight);
    }

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
        Transform player = PlayerManager.Instance != null
            ? PlayerManager.Instance.PlayerTransform
            : null;
        if (player == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            player.position,
            2.5f,
            LayerMask.GetMask("Player"));

        foreach (Collider2D hit in hits)
        {
            GameObject target = hit.GetComponentInParent<PlayerController>()?.gameObject ?? hit.gameObject;
            StatusEffectController effects = target.GetComponent<StatusEffectController>();
            if (effects == null)
                effects = target.AddComponent<StatusEffectController>();

            effects.ApplyEffect(new StatModifierEffect(
                slowDur,
                target,
                new List<StatModifierConfig>
                {
                    new StatModifierConfig
                    {
                        statType = StatType.MovementSpeed,
                        value = -(slowPct / 100f),
                        modifierType = ModifierType.Multiplicative
                    }
                },
                true));
        }
    }

    private void SpawnHailstone()
    {
        int damage = chipDamageByPhase[(int)currentPhase];
        if (hailstonePrefab == null || ObjectPool.Instance == null)
            return;

        Transform player = PlayerManager.Instance != null
            ? PlayerManager.Instance.PlayerTransform
            : null;
        if (player == null) return;

        Vector3 spawnPosition = new Vector3(
            player.position.x + Random.Range(-hailSpawnHalfWidth, hailSpawnHalfWidth),
            groundY + hailSpawnHeight,
            player.position.z);
        GameObject hailstoneObject = ObjectPool.Instance.SpawnFromPool(
            HAILSTONE_POOL_TAG,
            spawnPosition,
            Quaternion.identity);
        if (hailstoneObject == null) return;

        CameraController camera = Object.FindFirstObjectByType<CameraController>();
        camera?.Shake();

        HailstoneInstance hailstone = hailstoneObject.GetComponent<HailstoneInstance>();
        if (hailstone != null)
            hailstone.Initialize(damage, (int)currentPhase >= 3 ? 10f : 8f, 4f);
    }

    public void SetPhase(GolemController.GolemPhase phase)
    {
        currentPhase = phase;
    }

    public void SetEnabled(bool e)
    {
        if (e && !enabled)
        {
            tremorTimer = INITIAL_TREMOR_DELAY;
            hailTimer = (int)currentPhase >= 2
                ? hailIntervalByPhase[(int)currentPhase]
                : 0f;
        }
        enabled = e;
    }

    public void Cleanup()
    {
        enabled = false;
    }
}
