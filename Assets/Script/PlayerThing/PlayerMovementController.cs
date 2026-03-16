using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float runMultiplier = 2f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private PlayerAnimationController animationController;
    private StatusEffectController seController;

    private bool isGrounded;
    private int direction = 1;// 1 for right, -1 for left
    private float currentSpeed;
    private float speedMultiplier = 1.0f;

    // Properties for external access
    public float CurrentSpeed => currentSpeed;
    public int Direction => direction;
    public bool IsGrounded => isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animationController = GetComponent<PlayerAnimationController>();
        seController = GetComponent<StatusEffectController>();
        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if (rb == null)
            Debug.LogError("MovementController requires Rigidbody2D on " + gameObject.name);
        if (sr == null)
            Debug.LogError("MovementController requires SpriteRenderer on " + gameObject.name);
        if (animationController == null)
            Debug.LogError("MovementController requires PlayerAnimationController on " + gameObject.name);
    }

    private void Update()
    {
        CheckGrounded();
    }

    public void Move(float horizontalInput, bool isRunning)
    {
        if(rb == null) return;
        if(seController != null && seController.IsStunned)
        {
            return;
        }

        if (horizontalInput > 0)
        {
            direction = 1;
            sr.flipX = false;
        }
        else if (horizontalInput < 0)
        {
            direction = -1;
            sr.flipX = true;
        }

        currentSpeed = baseSpeed * (isRunning ? runMultiplier : 1f) * speedMultiplier;
        bool isMoving = horizontalInput != 0;
        rb.linearVelocity = new Vector2(horizontalInput * currentSpeed, rb.linearVelocity.y);
        animationController.SetMovementState(isMoving, isRunning);
    }

    public bool TryJump()
    {
        if (rb == null || !isGrounded) return false;
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        return true;
    }

    public void Stop()
    {
        if(rb == null) return;

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animationController.SetMovementState(false, false);
    }

    public void SetVelocity(Vector2 velocity)
    {
        if(rb == null) return;
        rb.linearVelocity = velocity;
    }

    public Vector2 GetVelocity()
    {
        return rb != null ? rb.linearVelocity : Vector2.zero;
    }

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }

    private void CheckGrounded()
    {
        // Calculate position below player feet
        Vector2 groundCheckPosition = new Vector2(rb.position.x, rb.position.y - groundCheckDistance);

        // Check with the configured layer mask
        Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheckPosition, groundCheckRadius, groundLayer);
        isGrounded = hits.Length > 0;
    }

    
    public void SpeedMultiplier(float multiplier)
    {
        speedMultiplier = Mathf.Clamp01(multiplier);
    }
    public void ResetSpeedMultiplier()
    {
        speedMultiplier = 1.0f;
    }
}

