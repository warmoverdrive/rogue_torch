using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Design Levers")]
    [SerializeField]
    float moveSpeed = 1f;
    [SerializeField]
    float jumpForce = 1f;
    [SerializeField]
    float dropSpeed = 1f;
    [SerializeField]
    float footstepRate = 0.5f;
    [SerializeField]
    AudioClip[] footstepSounds;
    [SerializeField]
    PhysicsMaterial2D groundMaterial, jumpMaterial;

    // Internal Component References
    PlayerStatusController statusController;
    Animator animator;
    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;

    // status variables
    bool facingRight = true;
    public bool isJumping = false;
    public bool canDrop = false;
    bool isDropping = false;
    public bool isTakingAction = false;
    public bool hasExited;
    float footstepCooldown;

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
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        groundLayer = LayerMask.NameToLayer("Ground");
        platformLayer = LayerMask.NameToLayer("Platform");
        audioSource = GetComponent<AudioSource>();
        footstepCooldown = footstepRate;
    }

    void Update()
    {
        capsuleCollider.sharedMaterial = (isJumping == true) ? jumpMaterial : groundMaterial;
        
        if (hasExited || statusController.IsDead()) return;
        if(!isTakingAction)
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

        Footsteps();

        float deltaX = moveSpeed * Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(deltaX, rb.velocity.y);

        if (Mathf.Sign(deltaX) == 1) facingRight = true;
        else if (Mathf.Sign(deltaX) == -1) facingRight = false;
    }

	private void Footsteps()
	{
        footstepCooldown -= Time.deltaTime;
        if (footstepCooldown <= Mathf.Epsilon && !isJumping)
		{
            audioSource.pitch += Random.Range(-0.25f, 0.25f);
            audioSource.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
            audioSource.pitch = 1;
            footstepCooldown = footstepRate;
		}
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
            canDrop = false;
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
