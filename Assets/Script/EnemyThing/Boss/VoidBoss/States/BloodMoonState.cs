using UnityEngine;

public class BloodMoonState : IEnemyState
{
    private int currentWave;
    private float waveTimer;

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        currentWave = 0;
        waveTimer = 0f;
        combat.PlayAnimTrigger("BloodMoon");
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        if (ctx is VoidBossController boss)
        {
            waveTimer += Time.deltaTime;

            if (waveTimer >= boss.BloodMoonWaveInterval)
            {
                waveTimer = 0f;
                boss.SpawnBloodMoonWave();
                currentWave++;

                if (currentWave >= boss.BloodMoonWaves)
                {
                    ctx.SwitchTo("VoidIdle");
                }
            }
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx) { }
}
