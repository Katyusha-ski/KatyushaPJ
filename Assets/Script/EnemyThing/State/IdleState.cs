using UnityEngine;

public class IdleState : IEnemyState
{
    public void OnEnter(EnemyController enemy)
    {
        enemy.SetAnimatorBool("Run", false);
    }

    public void OnUpdate(EnemyController enemy)
    {
        if (enemy.GetDistanceToPlayer() < enemy.GetVisionRange())
        {
            enemy.ChangeState(enemy.GetPursuitState());
        }
        else
        {
            enemy.Patrol();
        }
    }

    public void OnExit(EnemyController enemy)
    {
    }
}
