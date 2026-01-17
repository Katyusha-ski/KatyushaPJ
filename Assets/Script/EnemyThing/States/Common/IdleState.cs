using UnityEngine;

public class IdleState : IEnemyState
{
    public void OnEnter(EnemyController enemy)
    {
        Debug.Log($"{enemy.gameObject.name} entered Idle State");
        enemy.SetAnimatorBool("Run", false);
    }

    public void OnUpdate(EnemyController enemy)
    {
        float distanceToPlayer = enemy.GetDistanceToPlayer();
        if (distanceToPlayer <= enemy.GetVisionRange())
        {
            Debug.Log($"{enemy.gameObject.name} sees player! Distance: {distanceToPlayer}, VisionRange: {enemy.GetVisionRange()}");
            enemy.ChangeStateByName("Pursuit");
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
