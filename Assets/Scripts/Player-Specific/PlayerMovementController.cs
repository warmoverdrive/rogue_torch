using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Design Levers")]
    [SerializeField]
    float moveSpeed = 1f;
    [SerializeField]
    float jumpForce = 1f;
    [SerializeField]
    float dropSpeed = 1f;

    // Internal Component References
    PlayerStatusController statusController;
    Animator animator;
    Rigidbody2D rb;

    // status variables
    bool facingRight = true;
    public bool isJumping = false;
    public bool canDrop = false;
    bool isDropping = false;
    public bool isTakingAction = false;
    public bool hasExited;

    // Data variables
    int groundLayer;
    int platformLayer;
    Vector2 rightFaceScale = new Vector2(1, 1);
    Vector2 leftFaceScale = new Vector2(-1, 1);


    void Start()
    {
        statusController = GetComponent<PlayerStatusController>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        groundLayer = LayerMask.NameToLayer("Ground");
        platformLayer = LayerMask.NameToLayer("Platform");
    }

    void Update()
    {
        if (hasExited) return;
        if(statusController.IsDead() == false && !isTakingAction)
		{
            if (Input.GetAxis("Horizontal") != 0 && !isDropping) Move();
            else animator.SetBool("isWalking", false);

            Flip();
            Jump();

            if (!isDropping) Drop();
		}
    }

    private void Drop()
    {
        if (Input.GetButtonDown("Drop") && canDrop == true)
        {
            canDrop = false;
            StartCoroutine("DropMovement");
            isDropping = true;
        }
    }

    private IEnumerator DropMovement()
    {
        rb.isKinematic = true;
        rb.velocity = Vector3.down * dropSpeed;
        yield return new WaitForSeconds(0.1f);
        rb.isKinematic = false;
        isDropping = false;
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isJumping == false)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
        }
    }

    private void Move()
    {
        animator.SetBool("isWalking", true);

        float deltaX = moveSpeed * Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(deltaX, rb.velocity.y);

        if (Mathf.Sign(deltaX) == 1) facingRight = true;
        else if (Mathf.Sign(deltaX) == -1) facingRight = false;
    }

    void Flip()
    {
        if (facingRight) transform.localScale = rightFaceScale;
        else transform.localScale = leftFaceScale;
    }

    public bool FacingRight() => facingRight;

    private void OnTriggerStay2D(Collider2D collision)
    {
        var layer = collision.gameObject.layer;
        if (layer == groundLayer)
        {
            isJumping = false;
        }
        else if (layer == platformLayer)
        {
            isJumping = false;
            canDrop = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var layer = collision.gameObject.layer;
        if (layer == groundLayer || layer == platformLayer)
        {
            isJumping = true;
        }    
    }
}
