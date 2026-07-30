using UnityEngine;

public class ArenaHazardController
{
    public IEnvironmentSkill ccSkill;
    public IEnvironmentSkill dmgSkill;

    private bool enabled = true;

    public ArenaHazardController(IEnvironmentSkill ccSkill, IEnvironmentSkill dmgSkill)
    {
        this.ccSkill = ccSkill;
        this.dmgSkill = dmgSkill;
    }

    public void Tick(float dt)
    {
        if (!enabled) return;

        ccSkill?.Tick(dt);
        dmgSkill?.Tick(dt);
    }

    public void SetPhase(GolemController.GolemPhase phase)
    {
        ccSkill?.SetPhase(phase);
        dmgSkill?.SetPhase(phase);
    }

    public void SetEnabled(bool e)
    {
        enabled = e;
        ccSkill?.SetEnabled(e);
        dmgSkill?.SetEnabled(e);
    }
}
