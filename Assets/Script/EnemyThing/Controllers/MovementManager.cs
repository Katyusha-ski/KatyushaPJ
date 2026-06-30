using UnityEngine;

public class MovementManager
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CharacterStats stats;
    private int direction = 1;
    private int lastPatrolDirection = 1;

    public MovementManager(Rigidbody2D rb, SpriteRenderer sr, CharacterStats stats)
    {
        this.rb = rb;
        this.sr = sr;
        this.stats = stats;
    }

    public void Patrol()
    {
        direction = lastPatrolDirection;
        rb.linearVelocityX = stats.MovementSpeed * direction;
        sr.flipX = direction < 0;
    }

    public void LookAtPlayer(Transform player)
    {
        if (player == null) return;

        float diffX = player.position.x - rb.position.x;
        if (Mathf.Abs(diffX) > 0.2f)
        {
            direction = diffX > 0 ? 1 : -1;
            sr.flipX = direction < 0;
        }
        rb.linearVelocity = Vector2.zero;
    }

    public void MoveTowardPlayer(Transform player, float speedMultiplier = 1.5f)
    {
        if (player == null) return;

        float diffX = player.position.x - rb.position.x;
        if (Mathf.Abs(diffX) > 0.2f)
        {
            direction = diffX > 0 ? 1 : -1;
            sr.flipX = direction < 0;
            rb.linearVelocityX = stats.MovementSpeed * speedMultiplier * direction;
        }
        else
        {
            rb.linearVelocityX = 0f;
        }
    }

    public void RetreatFromPlayer(Transform player, float speedMultiplier = 0.8f)
    {
        if (player == null) return;

        float diffX = player.position.x - rb.position.x;
        direction = diffX > 0 ? -1 : 1;
        sr.flipX = direction < 0;
        rb.linearVelocityX = stats.MovementSpeed * speedMultiplier * direction;
    }

    public float GetDistanceToPlayer(Transform player)
    {
        if (player == null) return Mathf.Infinity;
        return Vector2.Distance(rb.position, player.position);
    }

    public void SetDirection(int dir)
    {
        direction = dir;
        lastPatrolDirection = dir;
    }

    public int GetDirection() => direction;

    public void OnHitObstacle()
    {
        direction *= -1;
        lastPatrolDirection = direction;
    }
}
