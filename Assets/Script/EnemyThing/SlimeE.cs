using UnityEngine;

public class SlimeE : EnemyController
{
    private bool checkAttack = false;

    public override void ExecuteAttack()
    {
        if (checkAttack)
        {
            NormalAttack();
            checkAttack = false;
        }
        else
        {
            animator.SetTrigger("Hit");
            checkAttack = true;
        }
    }
}
