using UnityEngine;

public class SlimeE : EnemyController
{
    private bool checkAttack = false;

    public override void ExecuteAttack()
    {
        if (checkAttack)
        {
            PlayAnimTrigger("Attack");
            checkAttack = false;
        }
        else
        {
            PlayAnimTrigger("Hit");
            checkAttack = true;
        }
    }
}
