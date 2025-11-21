using UnityEngine;

public class AttackState : BaseAttackState
{
    protected override void ExecuteAttackPattern(EnemyController enemy)
    {
        if (IsAttackReady(enemy))
        {
            enemy.ExecuteAttack();
            RecordAttack(enemy);
        }
    }
}