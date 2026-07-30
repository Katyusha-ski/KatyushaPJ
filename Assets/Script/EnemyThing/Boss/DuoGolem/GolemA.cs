using UnityEngine;

public class GolemA : GolemController
{
    protected override IEnvironmentSkill CreateCCSkill()
    {
        return new SnapTrapSkill();
    }

    protected override IEnvironmentSkill CreateDmgSkill()
    {
        return new RollingStoneSkill();
    }
}
