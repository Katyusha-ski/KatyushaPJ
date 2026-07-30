public interface IEnvironmentSkill
{
    void Tick(float dt);
    void SetPhase(GolemController.GolemPhase phase);
    void SetEnabled(bool enabled);
}
