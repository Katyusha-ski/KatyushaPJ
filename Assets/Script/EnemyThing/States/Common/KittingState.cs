using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class KittingState : IEnemyState
{
    public void OnEnter(EnemyController enemy)
    {
        enemy.SetAnimatorBool("Run", false);
    }

    public void OnUpdate(EnemyController enemy)
    {
        if (enemy is IRangedEnemy ranged)
        { 
            float distanceToPlayer = enemy.GetDistanceToPlayer();
            if (distanceToPlayer <= enemy.GetAttackRange() && distanceToPlayer > ranged.GetPreferedDistance()) 
            {   
                enemy.ChangeStateByName("Attack");
                return;
            }
            ranged.ExecuteKitting();
        }
        
    }

    public void OnExit(EnemyController enemy)
    {

    }
}