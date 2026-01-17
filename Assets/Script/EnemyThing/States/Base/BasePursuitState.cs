using UnityEngine;

public class BasePursuitState : IEnemyState
{
    public virtual void OnEnter(EnemyController enemy)
    {
        Debug.Log($"{enemy.gameObject.name} entered Pursuit State");
        enemy.SetAnimatorBool("Run", true);
    }

    public virtual void OnUpdate(EnemyController enemy)
    {
        float distanceToPlayer = enemy.GetDistanceToPlayer();

        if (distanceToPlayer > enemy.GetVisionRange())
        {
            enemy.ChangeStateByName("Idle"); 
            return;
        }
        
        if (distanceToPlayer <= enemy.GetAttackRange())
        {
            enemy.ChangeStateByName("Attack"); 
            return;
        }

        enemy.Pursue();
    }

    public virtual void OnExit(EnemyController enemy)
    {
        enemy.SetAnimatorBool("Run", false);
    }
}
