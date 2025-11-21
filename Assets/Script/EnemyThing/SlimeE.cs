using UnityEngine;

public class SlimeE : EnemyController
{
    public override IEnemyState GetAttackState()
    {
        return new SlimeAttackState();
    }
}
