using UnityEngine;

public class GolemAttackState : BaseAttackState
{
    public SkillManager skillManager;

    public override void OnEnter(EnemyController enemy)
    {
        base.OnEnter(enemy);
        skillManager = enemy.GetComponent<SkillManager>();
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (enemy.GetDistanceToPlayer() > enemy.GetAttackRange())
        {
            enemy.ChangeState(enemy.GetPursuitState());
            return;
        }

        ExecuteAttackPattern(enemy);
    }
}
