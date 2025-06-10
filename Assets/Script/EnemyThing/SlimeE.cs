using UnityEngine;

public class SlimeE : EnemyController
{
    private bool checkAttack = false;

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            animator.SetBool("Run", false);
            if (Time.time - lastTimeAttack >= attackCooldown) 
            {
                if (checkAttack)
                {
                    NormalAttack();
                    lastTimeAttack = Time.time;
                    checkAttack = false;
                }
                else
                {
                    Hit();
                    lastTimeAttack = Time.time;
                    checkAttack = true;
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
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
            Patrol();
        }
    }

    public void Hit()
    {
        animator.SetTrigger("Hit");
    }
}
