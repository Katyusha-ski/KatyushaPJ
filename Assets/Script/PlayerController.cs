using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float runSpeed = 5.0f;
    [SerializeField] private float walkSpeed = 2.5f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MovingPlayer();
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
    }
}
