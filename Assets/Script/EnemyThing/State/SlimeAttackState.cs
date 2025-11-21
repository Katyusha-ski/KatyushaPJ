public class SlimeAttackState : IEnemyState
{
    private bool checkAttack = false;

    public void OnEnter(EnemyController enemy)
    {
        enemy.SetAnimatorBool("Run", false);
        enemy.LookAtPlayer();
        checkAttack = false;  // Reset khi vào state
    }

    public void OnUpdate(EnemyController enemy)
    {
        // Ki?m tra ra kh?i attack range
        if (enemy.GetDistanceToPlayer() > enemy.GetAttackRange())
        {
            enemy.ChangeState(enemy.GetPursuitState());
        }
        // Ki?m tra ra kh?i vision range
        else if (enemy.GetDistanceToPlayer() > enemy.GetVisionRange())
        {
            enemy.ChangeState(enemy.GetIdleState());
        }
        // Trong attack range - th?c hi?n attack xen k?
        else
        {
            ExecuteSlimeAttackPattern(enemy);
        }
    }

    public void OnExit(EnemyController enemy)
    {
    }

    /// <summary>
    /// Slime attack pattern: Hit ? Attack ? Hit ? Attack ? ...
    /// </summary>
    private void ExecuteSlimeAttackPattern(EnemyController enemy)
    {
        if (UnityEngine.Time.time - enemy.GetLastTimeAttack() >= enemy.GetAttackCooldown())
        {
            if (checkAttack)
            {
                // Th?c hi?n NormalAttack
                enemy.SetAnimatorTrigger("Attack");
                enemy.SetLastTimeAttack(UnityEngine.Time.time);
                checkAttack = false;
            }
            else
            {
                // Th?c hi?n Hit
                enemy.SetAnimatorTrigger("Hit");
                enemy.SetLastTimeAttack(UnityEngine.Time.time);
                checkAttack = true;
            }
        }
    }
}
