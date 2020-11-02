using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Design Levers")]
    [SerializeField]
    float moveSpeed = 5;
    [SerializeField]
    float wallRayDistance = 0.5f;

    public Vector2 rayDirection = Vector2.right;
    public float rayOffset = 0.5f;

    float debugWallDist;
    
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //debug code
        debugWallDist = wallRayDistance;
    }

    void FixedUpdate()
    {
        Vector2 raycastOrigin = new Vector2(transform.position.x + rayOffset, transform.position.y);

        RaycastHit2D wallRay = Physics2D.Raycast(raycastOrigin, rayDirection, wallRayDistance);

        // not working right now but good enough
        Debug.DrawRay(raycastOrigin, new Vector2(rayDirection.x + debugWallDist, rayDirection.y), Color.red);

        if (wallRay.collider != null)
		{
            Flip();
		}

        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    private void Flip()
	{
        transform.localScale = new Vector3(-transform.localScale.x, 1, 0);
        moveSpeed = -moveSpeed;
        rayDirection = -rayDirection;
        rayOffset = -rayOffset;

        //debug code
        debugWallDist = -debugWallDist;
    }
}
