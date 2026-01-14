public class RangedAttackState : BaseAttackState
{

    public override void OnEnter(EnemyController enemy)
    {
        base.OnEnter(enemy);
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (enemy is IRangedEnemy ranged)
        {

            float distanceToPlayer = enemy.GetDistanceToPlayer();
            if (distanceToPlayer > enemy.GetAttackRange())
            {
                enemy.ChangeStateByName("Pursuit");  
                return;
            }

            if (distanceToPlayer < ranged.GetCloseDistance())
            {
                enemy.ChangeStateByName("Kitting");  
                return;
            }
        }

        ExecuteAttackPattern(enemy);
    }
}
