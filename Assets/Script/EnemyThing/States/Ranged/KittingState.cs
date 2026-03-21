using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class KittingState : IEnemyState
{
    public void OnEnter(EnemyController enemy)
    {
        Debug.Log($"Entering Kitting State for {enemy.name}");
        enemy.SetAnimatorBool("Run", false);
    }

    public void OnUpdate(EnemyController enemy)
    {
        if (enemy is IRangedEnemy ranged)
        { 
            float distanceToPlayer = enemy.GetDistanceToPlayer();

            // Too far from attack range → pursue
            if (distanceToPlayer > enemy.GetAttackRange())
            {
                enemy.ChangeStateByName("Pursuit");
                return;
            }

            // Reached preferred distance → attack
            if (distanceToPlayer >= ranged.GetCloseDistance() && distanceToPlayer <= ranged.GetPreferedDistance())
            {
                enemy.ChangeStateByName("Attack");
                return;
            }

            // Maintain distance - kitting
            ranged.ExecuteKitting();
        }
    }

    public void OnExit(EnemyController enemy)
    {

    }
}