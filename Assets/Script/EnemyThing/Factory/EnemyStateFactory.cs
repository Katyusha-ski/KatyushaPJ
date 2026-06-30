public class EnemyStateFactory
{
    public virtual IEnemyState CreateIdleState() => new IdleState();
    public virtual IEnemyState CreatePursuitState() => new BasePursuitState();
    public virtual IEnemyState CreateAttackState() => new BaseAttackState();
    public virtual IEnemyState CreateAlertState() => new AlertState();
    public virtual IEnemyState CreateHurtState(IEnemyState preState) => new HurtState(preState);
    public virtual IEnemyState CreateDieState() => new DieState();
    public virtual IEnemyState CreateKittingState() => new KittingState();
}
