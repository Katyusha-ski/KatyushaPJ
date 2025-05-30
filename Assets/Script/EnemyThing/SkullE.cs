using UnityEngine;

public class SkullE : EnemyController
{

    void Update()
    {

        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            if (Time.time - lastTimeAttack >= attackCooldown)
            {
                NormalAttack();
                lastTimeAttack = Time.time;
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
}
