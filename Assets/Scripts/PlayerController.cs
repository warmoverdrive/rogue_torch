using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Design Levers")]
    [SerializeField]
    float moveSpeed = 1f;
    [SerializeField]
    float jumpForce = 1f;

    Rigidbody2D rb;
    bool facingRight = true;
    bool isJumping = false;

    Vector2 rightFaceScale = new Vector2(1, 1);
    Vector2 leftFaceScale = new Vector2(-1, 1);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetAxis("Horizontal") != 0) Move();
        Flip();
        Jump();
    }

    private void Jump()
    {
        if (Input.GetAxis("Jump") > 0 && isJumping == false)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
        }
    }

    private void Move()
    {
        float deltaX = moveSpeed * Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(deltaX, rb.velocity.y);

        if (Mathf.Sign(deltaX) == 1) facingRight = true;
        else if (Mathf.Sign(deltaX) == -1) facingRight = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isJumping = false;
        }
    }

    void Flip()
    {
        if (facingRight) transform.localScale = rightFaceScale;
        else transform.localScale = leftFaceScale;
    }
}
