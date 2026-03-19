using UnityEngine;

public class HealState : IEnemyState
{
    private IEnemyState previousState;

    public HealState(IEnemyState previousState)
    {
        this.previousState = previousState;
    }

    public void SetPreviousState(IEnemyState state)
    {
        previousState = state;
    }

    public void OnEnter(EnemyController enemy)
    {
        Debug.Log($"{enemy.gameObject.name} entered Heal State");
        enemy.SetAnimatorBool("Run", false);
    }

    public void OnUpdate(EnemyController enemy)
    {
        if (enemy is NecromancerE necromancer)
        {
            var health = enemy.GetComponent<Health>();
            if (health != null)
            {
                // Still need healing? Cast heal
                if (health.CurrentHealth < health.MaxHealth * 0.5f)
                {
                    enemy.SetAnimatorTrigger("Skill3");
                    return;
                }

                // Health recovered to 50%+ → return to previous state
                enemy.ChangeState(previousState ?? new IdleState());
                return;
            }
        }
    }

    public void OnExit(EnemyController enemy)
    {
    }
}
