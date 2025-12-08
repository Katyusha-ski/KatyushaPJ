public interface IEnemyState
{
    void OnEnter(EnemyController enemy);
    void OnUpdate(EnemyController enemy);
    void OnExit(EnemyController enemy);
}

public interface IEnemyStateProvider
{
    IEnemyState GetIdleState();
    IEnemyState GetPursuitState();
    IEnemyState GetAttackState();
    IEnemyState GetHurtState(IEnemyState preState);
    IEnemyState GetDieState();
}
