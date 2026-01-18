using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private PlayerAnimationController animationController;

    private bool isGrounded;
    private int direction = 1;// 1 for right, -1 for left
    private float currentSpeed;

    // Properties for external access
    public float CurrentSpeed => currentSpeed;
    public int Direction => direction;
    public bool IsGrounded => isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animationController = GetComponent<PlayerAnimationController>();
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
        if (groundCheck == null)
            Debug.LogError("MovementController requires groundCheck transform assigned");
    }

    private void FixedUpdate()
    {
        CheckGrounded();
    }

    public void Move(float horizontalInput, bool isRunning)
    {
        if(rb == null) return;

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

        currentSpeed = isRunning ? runSpeed : walkSpeed;
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
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}

