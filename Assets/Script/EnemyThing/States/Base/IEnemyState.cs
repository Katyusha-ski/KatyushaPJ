public interface IEnemyState
{
    void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx);
    void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx);
    void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx);
}

public interface IEnemyStateProvider
{
    IEnemyState GetIdleState();
    IEnemyState GetPursuitState();
    IEnemyState GetAttackState();
    IEnemyState GetAlertState();
    IEnemyState GetHurtState(IEnemyState preState);
    IEnemyState GetDieState();
    IEnemyState GetKittingState();
}
