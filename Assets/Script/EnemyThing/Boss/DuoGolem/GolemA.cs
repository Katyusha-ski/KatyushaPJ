using UnityEngine;

public class GolemA : GolemController
{
    [SerializeField] private GameObject snapTrapPrefab;
    [SerializeField] private GameObject rollingStonePrefab;

    protected override IEnvironmentSkill CreateCCSkill()
    {
        return new SnapTrapSkill(snapTrapPrefab, GroundY);
    }

    protected override IEnvironmentSkill CreateDmgSkill()
    {
        return new RollingStoneSkill(rollingStonePrefab, GroundY);
    }
}
