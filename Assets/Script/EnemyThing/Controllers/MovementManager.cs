using UnityEngine;

public class MovementManager
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CharacterStats stats;
    private int direction = 1;
    private int lastPatrolDirection = 1;
    private float patrolMinX = float.NegativeInfinity;
    private float patrolMaxX = float.PositiveInfinity;
    private Collider2D col;
    private readonly RaycastHit2D[] groundHits = new RaycastHit2D[4];

    public MovementManager(Rigidbody2D rb, SpriteRenderer sr, CharacterStats stats)
    {
        this.rb = rb;
        this.sr = sr;
        this.stats = stats;
    }

    public void Patrol()
    {
        if (lastPatrolDirection > 0 && rb.position.x >= patrolMaxX)
            Flip();
        else if (lastPatrolDirection < 0 && rb.position.x <= patrolMinX)
            Flip();

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
        rb.linearVelocityX = 0f;
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

    public void MoveTowardsX(float targetX, float speedMultiplier = 1.0f)
    {
        float diffX = targetX - rb.position.x;
        if (Mathf.Abs(diffX) > 0.05f)
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
        Flip();
    }

    public void SetPatrolBounds(float minX, float maxX)
    {
        patrolMinX = minX;
        patrolMaxX = maxX;
    }

    public void Stop()
    {
        rb.linearVelocityX = 0f;
    }

    private void Flip()
    {
        direction *= -1;
        lastPatrolDirection = direction;
    }
    public bool IsGrounded()
    {
        if (col == null) col = rb.GetComponent<Collider2D>();
        if (col == null || rb == null) return false;

        Bounds b = col.bounds;
        float skin = 0.08f;

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.useLayerMask = false;

        int n = Physics2D.Raycast(
            new Vector2(b.center.x, b.center.y),
            Vector2.down,
            filter,
            groundHits,
            b.extents.y + skin);

        for (int i = 0; i < n; i++)
        {
            if (groundHits[i].collider.attachedRigidbody != rb)
                return true;
        }

        return false;
    }

    public void SnapToY (float targetY)
    {
        rb.position = new Vector2(rb.position.x, targetY);
        rb.linearVelocity = Vector2.zero;
    }

    public void SnapToPosition(Vector2 targetPos)
    {
        rb.position = targetPos;
        rb.linearVelocity = Vector2.zero;
    }
    
    public bool IsAtPlatformEdge(float lookAhead = 0.6f)
    {
        if (col == null) col = rb.GetComponent<Collider2D>();
        if (col == null || rb == null) return false;

        Bounds b = col.bounds;
        Vector2 origin = new Vector2(rb.position.x + direction * lookAhead, b.center.y);

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.useLayerMask = false;

        int n = Physics2D.Raycast(origin, Vector2.down, filter, groundHits, b.extents.y + 0.3f);
        for (int i = 0; i < n; i++)
            if (groundHits[i].collider.attachedRigidbody != rb)
                return false;

        return true;

    }
}