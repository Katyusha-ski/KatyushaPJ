using UnityEngine;

public class AlertState : IEnemyState
{
    private float alertDuration = 0.5f;
    private float elapsedTime = 0f;

    public void OnEnter(EnemyController enemy)
    {
        elapsedTime = 0f;
        enemy.SetAnimatorBool("Run", false);
        enemy.SetAnimatorTrigger("Alert");
    }

    public void OnUpdate(EnemyController enemy)
    {
        float distanceToPlayer = enemy.GetDistanceToPlayer();

        if (distanceToPlayer <= enemy.GetVisionRange())
        {
            enemy.ChangeState(enemy.GetPursuitState());
            return;
        }

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= alertDuration)
        {
            enemy.ChangeState(enemy.GetIdleState());
        }
    }

    public void OnExit(EnemyController enemy)
    {
    }
}
