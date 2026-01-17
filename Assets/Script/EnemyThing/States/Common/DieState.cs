using UnityEngine;

public class DieState : IEnemyState
{
    private float dieDuration = 1.0f;
    private float elapsedTime = 0f;
    public void OnEnter(EnemyController enemy)
    {
        Debug.Log($"{enemy.gameObject.name} entered Die State");
        enemy.SetAnimatorTrigger("Die");
        enemy.HandleEnemyDeath();
    }

    public void OnUpdate(EnemyController enemy) 
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= dieDuration)
        {
            Object.Destroy(enemy.gameObject);
        }
    }
    public void OnExit(EnemyController enemy) 
    {   
    }
}