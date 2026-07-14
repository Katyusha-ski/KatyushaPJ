using System;
using UnityEngine;

public class DieState : IEnemyState
{
    private float dieDuration;
    private float elapsedTime;
    private Action onDeath;

    public DieState() : this(1.0f, null) { }

    public DieState(float dieDuration = 1.0f, Action onDeath = null)
    {
        this.dieDuration = dieDuration;
        this.onDeath = onDeath;
    }

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsedTime = 0f;
        combat.PlayAnimTrigger("Die");
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= dieDuration)
        {
            onDeath?.Invoke();
            GameObject.Destroy(((MonoBehaviour)ctx).gameObject);
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
    }
}
