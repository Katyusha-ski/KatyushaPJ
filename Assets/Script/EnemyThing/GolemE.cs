using UnityEngine;

public class GolemE : EnemyController
{
    //public RuntimeAnimatorController phase1Controller;
    //public RuntimeAnimatorController phase2Controller;
    public bool isPhase2 = false;
    private bool checkAttack1 = false;

    [Header("Skill List")]
    public SkillManager skillManager;


    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        direction = player.position.x > transform.position.x ? 1 : -1;

        if (distanceToPlayer < attackRange)
        {
            animator.SetBool("Run", false);
            if (Time.time - lastTimeAttack >= attackCooldown)
            {
                if (skillManager.skills[1].CanActivate)
                {
                    animator.SetTrigger("Attack 3");
                    lastTimeAttack = Time.time;
                    return;
                }
                if (!checkAttack1)
                {
                    NormalAttack();
                    lastTimeAttack = Time.time;
                    checkAttack1 = true;
                }
                else
                {
                    Attack1();
                    lastTimeAttack = Time.time;
                    checkAttack1 = false;
                }

            }
            rb.linearVelocity = Vector2.zero;
        }
        else if (distanceToPlayer < visionRange)
        {
            if (skillManager.skills[0].CanActivate)
            {
                animator.SetBool("Run", false);
                animator.SetTrigger("Attack 2");
                
                return;
            }
            else 
            {
                rb.linearVelocity = new Vector2(speed * 1.5f * direction, rb.linearVelocity.y);
                sr.flipX = direction < 0;
                animator.SetBool("Run", true);
            }
                
        }
        else
        {
            animator.SetBool("Run", false);
            Patrol();
        }
    }

    /*public void ChangeToPhase2()
    {
        if (!isPhase2)
        {
            isPhase2 = true;
            animator.runtimeAnimatorController = phase2Controller;
            attackDamage += 2; 
            
        }
    }*/

    public void Attack1()
    {
        animator.SetTrigger("Attack 1");
    } 

    public void CastGolemMagic()
    {
        skillManager.ActivateSkill(0, direction);
        animator.ResetTrigger("Attack 2");
    }

    public void CastStoneSpike()
    {
        skillManager.ActivateSkill(1, 0);
    }
}
