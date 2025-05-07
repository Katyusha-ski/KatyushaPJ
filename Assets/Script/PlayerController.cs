using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float speed = 5.0f;
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
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rb.linearVelocity = movement.normalized * speed;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        //xoay nhan vat
        if (movement.x > 0)
        {
            sr.flipX = false;
        }
        else if (movement.x < 0)
        {
            sr.flipX = true;
        }

        //player Walking
        if (movement != Vector2.zero)
        {
            animator.SetBool("isWalk", true);
        }
        else
        {
            animator.SetBool("isWalk", false);
        }

        //player Sprinting
        if (isSprinting $$)
        {
            animator.SetBool("isSprint", true);
            rb.velocity = movement.normalized * speed * 2;
        }
        else
        {
            animator.SetBool("isSprint", false);
            rb.velocity = movement.normalized * speed;
        }

    }
}
