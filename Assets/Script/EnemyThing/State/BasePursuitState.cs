using UnityEngine;

public abstract class BasePursuitState : IEnemyState
{
    public virtual void OnEnter(EnemyController enemy)
    {
        enemy.SetAnimatorBool("Run", true);
    }

    public void OnUpdate(EnemyController enemy)
    {
        float distanceToPlayer = enemy.GetDistanceToPlayer();

        if (distanceToPlayer > enemy.GetAttackRange())
        {
            enemy.ChangeState(enemy.GetAttackState());
            return;
        }

        if (distanceToPlayer > enemy.GetVisionRange())
        {
            enemy.ChangeState(enemy.GetIdleState());
            return;
        }

        ExecutePursuit(enemy);
    }

    public virtual void OnExit(EnemyController enemy)
    {
        enemy.SetAnimatorBool("Run", false);
    }

    protected abstract void ExecutePursuit(EnemyController enemy);
}
