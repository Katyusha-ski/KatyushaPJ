using UnityEngine;

public class GolemB : GolemController
{
    protected override IEnvironmentSkill CreateCCSkill()
    {
        return new TremorHailstormSkill();
    }

    protected override IEnvironmentSkill CreateDmgSkill()
    {
        return new StoneSpikeStabSkill();
    }
}
