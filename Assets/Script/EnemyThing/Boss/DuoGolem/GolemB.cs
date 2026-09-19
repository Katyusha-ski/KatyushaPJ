using UnityEngine;

public class GolemB : GolemController
{
    [SerializeField] private GameObject stoneSpikePrefab;
    [SerializeField] private GameObject hailstonePrefab;
    [Header("Hailstorm Settings")]
    [Tooltip("Hailstone spawn interval per phase. 0 disables hailstone spawning for that phase.")]
    [SerializeField] private float[] hailSpawnIntervalByPhase = new float[] { 0f, 0f, 2f, 0.8f };
    [SerializeField] private float hailSpawnHalfWidth = 3f;
    [SerializeField] private float hailSpawnHeight = 5f;

    protected override bool IsVictoryOwner => true;

    protected override IEnvironmentSkill CreateCCSkill()
    {
        return new TremorHailstormSkill(
            hailstonePrefab,
            GroundY,
            hailSpawnIntervalByPhase,
            hailSpawnHalfWidth,
            hailSpawnHeight);
    }

    protected override IEnvironmentSkill CreateDmgSkill()
    {
        return new StoneSpikeStabSkill(stoneSpikePrefab, GroundY);
    }
}
