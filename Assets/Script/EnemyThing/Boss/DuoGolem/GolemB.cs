using UnityEngine;

public class GolemB : GolemController
{
    [SerializeField] private GameObject stoneSpikePrefab;
    [SerializeField] private GameObject hailstonePrefab;

    protected override bool IsVictoryOwner => true;

    protected override IEnvironmentSkill CreateCCSkill()
    {
        return new TremorHailstormSkill(hailstonePrefab, GroundY);
    }

    protected override IEnvironmentSkill CreateDmgSkill()
    {
        return new StoneSpikeStabSkill(stoneSpikePrefab, GroundY);
    }
}
