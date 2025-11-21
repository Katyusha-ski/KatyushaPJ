using UnityEngine;

public class PursuitState : BasePursuitState
{
    protected override void ExecutePursuit(EnemyController enemy)
    {
        enemy.LookAtPlayer();
        enemy.MoveTowardPlayer();
    }
}
