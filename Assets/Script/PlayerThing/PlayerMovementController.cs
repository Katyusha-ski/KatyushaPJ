using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float baseSpeed;
    [SerializeField] private float runMultiplier = 2f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private PlayerAnimationController animationController;
    private StatusEffectController seController;
    private CharacterStats stats;

    private bool isGrounded;
    private int direction = 1;// 1 for right, -1 for left
    private float currentSpeed;
    private float cachedHorizontalInput;
    private bool cachedIsRunning;

    // Properties for external access
    public float CurrentSpeed => currentSpeed;
    public int Direction => direction;
    public bool IsGrounded => isGrounded;
    public bool CanMove { get; set; } = true;

    // Bật trong lúc đang dash (VD: DashSkill set true khi bắt đầu, false khi kết thúc).
    // Khi true, FixedUpdate KHÔNG ghi đè rb.linearVelocity — để Dash giữ được vận tốc
    // riêng của nó thay vì bị movement thường áp đè (regression từ jitter fix).
    public bool IsDashing { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animationController = GetComponent<PlayerAnimationController>();
        seController = GetComponent<StatusEffectController>();
        stats = GetComponent<CharacterStats>();
        baseSpeed = stats != null ? stats.MovementSpeed : baseSpeed;

        // Subscribe to movement speed changes
        if (stats != null)
        {
            stats.MovementSpeedChanged += OnMovementSpeedChanged;
        }

        SetupZeroFrictionCollider();
        ValidateComponents();
    }

    // Collider player dùng BoxCollider2D (thêm ở từng scene) không có physics material,
    // friction mặc định 0.4 làm player bị "dính" vào mặt bên / góc collider ground.
    // Đặt friction = 0 → combined friction (friction player * friction ground) = 0.
    private void SetupZeroFrictionCollider()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null || col.sharedMaterial != null) return;

        PhysicsMaterial2D mat = new PhysicsMaterial2D("PlayerNoFriction")
        {
            friction = 0f,
            bounciness = 0f
        };
        col.sharedMaterial = mat;
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

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    public void Move(float horizontalInput, bool isRunning)
    {
        if(rb == null) return;
        if(!CanMove)
        {
            // Khi bị khóa (thoại/cutscene...): dừng animation run/walk + khử vận tốc ngang
            // để player đứng yên chứ không giữ nguyên trạng thái run đang chạy.
            animationController?.SetMovementState(false, false);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }
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

        // Use CharacterStats for movement speed (includes modifiers like slow effects)
        float effectiveSpeed = stats != null ? stats.MovementSpeed : baseSpeed;
        currentSpeed = effectiveSpeed * (isRunning ? runMultiplier : 1f);
        cachedHorizontalInput = horizontalInput;
        cachedIsRunning = isRunning;
        animationController.SetMovementState(horizontalInput != 0, isRunning);
    }

    public bool TryJump()
    {
        if (rb == null || !isGrounded) return false;
        if (!CanMove) return false;
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        return true;
    }

    public void Stop()
    {
        if(rb == null) return;

        cachedHorizontalInput = 0f;
        cachedIsRunning = false;
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

    private void OnMovementSpeedChanged(float newSpeed)
    {
        baseSpeed = newSpeed;
    }

    private void ApplyMovement()
    {
        if (rb == null) return;
        if (!CanMove) return;
        if (seController != null && seController.IsStunned)
            return;
        if (IsDashing)
            return;

        float effectiveSpeed = stats != null ? stats.MovementSpeed : baseSpeed;
        currentSpeed = effectiveSpeed * (cachedIsRunning ? runMultiplier : 1f);
        rb.linearVelocity = new Vector2(cachedHorizontalInput * currentSpeed, rb.linearVelocity.y);
    }

    private void OnDestroy()
    {
        if (stats != null)
        {
            stats.MovementSpeedChanged -= OnMovementSpeedChanged;
        }
    }
}

