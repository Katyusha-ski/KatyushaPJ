using UnityEngine;

public abstract class BaseAttackState : IEnemyState
{
    public virtual void OnEnter(EnemyController enemy)
    {
        enemy.SetAnimatorBool("Run", false);
        enemy.LookAtPlayer();
    }

    public void OnUpdate(EnemyController enemy)
    {
        float distanceToPlayer = enemy.GetDistanceToPlayer();

        if (distanceToPlayer > enemy.GetAttackRange())
        {
            enemy.ChangeState(enemy.GetPursuitState());
            return;
        }

        if (distanceToPlayer > enemy.GetVisionRange())
        {
            enemy.ChangeState(enemy.GetIdleState());
            return;
        }

        ExecuteAttackPattern(enemy);
    }

    public virtual void OnExit(EnemyController enemy)
    {
    }

    protected abstract void ExecuteAttackPattern(EnemyController enemy);

    protected bool IsAttackReady(EnemyController enemy)
    {
        return Time.time - enemy.GetLastTimeAttack() >= enemy.GetAttackCooldown();
    }

    protected void RecordAttack(EnemyController enemy)
    {
        enemy.SetLastTimeAttack(Time.time);
    }
}
