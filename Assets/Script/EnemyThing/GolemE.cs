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

        if (distanceToPlayer < attackRange)
        {
            if (Time.time - lastTimeAttack >= attackCooldown)
            {
                
                if (checkAttack1 == false)
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
            float moveDirection = player.position.x > transform.position.x ? 1 : -1;
            direction = (int)moveDirection;
            rb.linearVelocity = new Vector2(speed * 1.5f * direction, rb.linearVelocity.y);
            sr.flipX = direction < 0;
        }
        else
        {
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

        var playerHealth = player.GetComponent<Health>();
        if (player != null && Vector2.Distance(transform.position, player.position) < attackRange)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

}
