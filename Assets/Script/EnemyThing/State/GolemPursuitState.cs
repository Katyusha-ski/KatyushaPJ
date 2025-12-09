public class GolemPursuitState : BasePursuitState
{
    public SkillManager skillManager;

    public override void OnEnter(EnemyController enemy)
    {
        base.OnEnter(enemy);
        skillManager = enemy.GetComponent<SkillManager>();
    }

    public override void OnUpdate(EnemyController enemy)
    {
        float distanceToPlayer = enemy.GetDistanceToPlayer();

        if (distanceToPlayer > enemy.GetVisionRange())
        {
            enemy.ChangeState(enemy.GetAlertState());
            return;
        }

        if (distanceToPlayer <= enemy.GetAttackRange())
        {
            enemy.ChangeState(enemy.GetAttackState());
            return;
        }

        if (skillManager != null && skillManager.skills[0].CanActivate)
        {
            enemy.SetAnimatorBool("Run", false);
            enemy.SetAnimatorTrigger("Attack 2");
            return;
        }

        ExecutePursuit(enemy);
    }
}