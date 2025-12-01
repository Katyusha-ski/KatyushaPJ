using UnityEngine;

public class BaseAttackState : IEnemyState
{
    public virtual void OnEnter(EnemyController enemy)
    {
        enemy.SetAnimatorBool("Run", false);
        enemy.LookAtPlayer();
    }

    public virtual void OnUpdate(EnemyController enemy)
    {
        float distanceToPlayer = enemy.GetDistanceToPlayer();

        if (distanceToPlayer > enemy.GetAttackRange())
        {
            enemy.ChangeState(enemy.GetPursuitState());
            return;
        }

        ExecuteAttackPattern(enemy);
    }

    public virtual void OnExit(EnemyController enemy){}

    protected virtual void ExecuteAttackPattern(EnemyController enemy)
    {
        if (IsAttackReady(enemy))
        {
            enemy.ExecuteAttack();
            RecordAttack(enemy);
        }
    }

    protected bool IsAttackReady(EnemyController enemy)
    {
        return Time.time - enemy.GetLastTimeAttack() >= enemy.GetAttackCooldown();
    }

    protected void RecordAttack(EnemyController enemy)
    {
        enemy.SetLastTimeAttack(Time.time);
    }
}
