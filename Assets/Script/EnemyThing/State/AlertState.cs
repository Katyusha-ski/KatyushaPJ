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
            enemy.ChangeStateByName("Pursuit");
            return;
        }

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= alertDuration)
        {
            enemy.ChangeStateByName("Idle");
        }
    }

    public void OnExit(EnemyController enemy)
    {
    }
}
