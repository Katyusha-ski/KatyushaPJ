public class BasePursuitState : IEnemyState
{
    public virtual void OnEnter(EnemyController enemy)
    {
        enemy.SetAnimatorBool("Run", true);
    }

    public virtual void OnUpdate(EnemyController enemy)
    {
        float distanceToPlayer = enemy.GetDistanceToPlayer();

        if (distanceToPlayer > enemy.GetVisionRange())
        {
            enemy.ChangeState(enemy.GetIdleState());
            return;
        }
        
        if (distanceToPlayer <= enemy.GetAttackRange())
        {
            enemy.ChangeState(enemy.GetAttackState());
            return;
        }

        ExecutePursuit(enemy);
    }

    public virtual void OnExit(EnemyController enemy)
    {
        enemy.SetAnimatorBool("Run", false);
    }

    protected virtual void ExecutePursuit(EnemyController enemy)
    {
        enemy.LookAtPlayer();
        enemy.MoveTowardPlayer();
    }
}
