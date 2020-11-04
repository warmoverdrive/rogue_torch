using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Design Levers")]
    [SerializeField]
    float moveSpeed = 5;
    [SerializeField]
    float wallRayDistance = 0.5f,
        floorRayDistance = 1f;
    [Header("Debug Raycast Properties")]
    public Vector2 rayDirection = Vector2.right/2;
    public float rayOffset = 0.25f;
    public bool isFacingRight { get; private set; } = true;

    IDamagable statusController;
	Rigidbody2D rb;
    EnemyAI enemyAI;
    int GROUND_LAYER_MASK;

	void Start()
    {
        GROUND_LAYER_MASK = LayerMask.NameToLayer("Ground");
        rb = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAI>();
        statusController = GetComponent<IDamagable>();
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

        Debug.Log(enemyAI.playerSighted);

		if (!enemyAI.playerSighted)
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
