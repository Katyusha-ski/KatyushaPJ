public interface IEnemyCombat
{
    bool IsAttackReady();
    void ExecuteAttack();
    void RecordAttack();
    float GetAttackRange();
    void PlayAnimTrigger(string trigger);
    void PlayAnimBool(string name, bool value);
}
