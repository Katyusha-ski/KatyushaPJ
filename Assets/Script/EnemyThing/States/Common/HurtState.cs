using UnityEngine;

public class HurtState : IEnemyState
{
    private float hurtDuration = 0.5f;
    private float elaspedTime = 0f;
    public IEnemyState preState;

    public HurtState(IEnemyState preState)
    {
        this.preState = preState;
    }

    public void OnEnter(EnemyController enemy)
    {
        Debug.Log($"{enemy.gameObject.name} entered Hurt State");
        elaspedTime = 0f;
        enemy.SetAnimatorTrigger("Hurt");
        enemy.SetAnimatorBool("Run", false);
    }

    public void OnUpdate(EnemyController enemy)
    {
        elaspedTime += Time.deltaTime;
        if (elaspedTime >= hurtDuration)
        {
            Debug.Log($"{enemy.gameObject.name} Hurt duration ended. preState = {preState?.GetType().Name ?? "null"}");
            if (preState != null)
            {
                enemy.ChangeState(preState);
            }
            else
            {
                Debug.LogError($"{enemy.gameObject.name} preState is null! Returning to Idle.");
                enemy.ChangeStateByName("Idle");
            }
        }
    }

    public void OnExit(EnemyController enemy) { }
}