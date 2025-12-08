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
        elaspedTime = 0f;
        enemy.SetAnimatorTrigger("Hurt");
        enemy.SetAnimatorBool("Run", false);
    }

    public void OnUpdate(EnemyController enemy)
    {
        elaspedTime += Time.deltaTime;
        if (elaspedTime >= hurtDuration)
        {
            enemy.ChangeState(preState);
        }
    }

    public void OnExit(EnemyController enemy) { }
}