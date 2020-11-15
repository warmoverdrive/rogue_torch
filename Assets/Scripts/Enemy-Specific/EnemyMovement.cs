using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Design Levers")]
    [SerializeField]
    float moveSpeed = 5;
    [SerializeField]
    float wallRayDistance = 0.5f,
        floorRayDistance = 1f,
        footstepRate = 0.5f,
        footstepJitter = 0.25f,
        footstepYVelocityBuffer = 0.1f;
    [SerializeField]
    AudioClip[] footstepSounds;
    [Header("Debug Raycast Properties")]
    public Vector2 rayDirection = Vector2.right/2;
    public float rayOffset = 0.25f;
    public bool isFacingRight { get; private set; } = true;

    IDamagable statusController;
	Rigidbody2D rb;
    EnemyAI enemyAI;
    AudioSource audioSource;
    float footstepCooldown;

	void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAI>();
        statusController = GetComponent<IDamagable>();
        audioSource = GetComponent<AudioSource>();
        footstepCooldown = (footstepRate += Random.Range(-footstepJitter, footstepJitter));
    }

    void FixedUpdate()
	{
        if (!statusController.IsDead() && !enemyAI.isPerformingAction)
            Move();
        else if (statusController.IsDead()) rb.simulated = false;
	}

	private void Move()
	{
		rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        Footstep();

		if (!enemyAI.playerInSight)
		{
			Vector2 raycastOrigin = new Vector2(transform.position.x + rayOffset, transform.position.y);

			RaycastHit2D wallRay = Physics2D.Raycast(
                raycastOrigin, rayDirection, wallRayDistance);
			RaycastHit2D floorRay = Physics2D.Raycast(
                raycastOrigin, rayDirection + Vector2.down, floorRayDistance);

			bool wallRayHitObject = wallRay.collider;

			if (wallRayHitObject || floorRay.collider == null)
			{
				if (wallRayHitObject &&
					wallRay.collider.gameObject.layer != LayerMask.NameToLayer("Ground")) return;
				else if (rb.velocity.y >= -.25f) Flip(); // stops flipping if falling, which just looks goofy af
			}
		}
	}

	private void Footstep()
	{
        footstepCooldown -= Time.deltaTime;
        if (footstepCooldown <= Mathf.Epsilon && Mathf.Abs(rb.velocity.y) < footstepYVelocityBuffer)
        {
            audioSource.pitch += Random.Range(-0.25f, 0.25f);
            audioSource.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
            audioSource.pitch = 1;
            footstepCooldown = footstepRate;
        }
    }

	private void Flip()
	{
        transform.localScale = new Vector3(-transform.localScale.x, 1, 0);
        moveSpeed = -moveSpeed;
        rayDirection = -rayDirection;
        rayOffset = -rayOffset;

        isFacingRight = !isFacingRight;

        enemyAI.Flip();
    }

    public void FacePlayer(bool playerRightOfThis)
	{
        if (playerRightOfThis != isFacingRight) Flip();
	}
}
