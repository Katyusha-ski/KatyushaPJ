using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //moving variables
    [SerializeField] private float runSpeed = 5.0f;
    [SerializeField] private float walkSpeed = 2.5f;

    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    [Header("Skill List")]
    
    public SkillManager skillManager;


    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private Health health;

    public float CurrentSpeed { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        MovingPlayer();
        CheckGrounded();
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        int direction = sr.flipX ? -1 : 1;
        if (Input.GetKeyDown(KeyCode.F))
        {
            skillManager.ActivateSkill(0, direction);
        }
    }

    private void MovingPlayer()
    {
        float speed;
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), 0);

        if (movement.x > 0)
        {
            sr.flipX = false;
        }
        else if (movement.x < 0)
        {
            sr.flipX = true;
        }
        if (Input.GetKey(KeyCode.LeftShift) && movement.x != 0)
        {
            speed = runSpeed;
            animator.SetBool("isRun", true);
        }
        else
        {
            speed = walkSpeed;
            animator.SetBool("isRun", false);
            if (movement.x != 0)
            {
                animator.SetBool("isWalk", true);
            }
            else
            {
                animator.SetBool("isWalk", false);
            }
        }

        rb.linearVelocity = movement * speed;
        CurrentSpeed = speed;
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    
}