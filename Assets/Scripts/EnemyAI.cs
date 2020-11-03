using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Design Levers")]
    [SerializeField]
    float sightDistance = 8;
    [SerializeField]
    float sightCircleRadius = 2;
    [SerializeField]
    Vector3 castOffset = Vector2.right/2;

    public bool playerSighted { get; private set; } = false;

    EnemyMovement enemyMovement;
    Vector2 sightDirection = Vector2.right;
    int PLAYER_MASK;

    void Start()
    {
        PLAYER_MASK = LayerMask.GetMask("Player");
        enemyMovement = GetComponent<EnemyMovement>();
    }

    void FixedUpdate()
    {
        RaycastHit2D sightCast = Physics2D.CircleCast(
            transform.position + castOffset, sightCircleRadius, sightDirection, sightDistance, PLAYER_MASK);

        if (sightCast.collider)
        {
            playerSighted = true;

            var player = sightCast.collider.gameObject;

            var direction = transform.position - player.transform.position;
            Debug.Log(direction.x < 0 ? "right" : "left");

            if (direction.x < 0) enemyMovement.FacePlayer(true);
            else enemyMovement.FacePlayer(false);
        }
        else playerSighted = false;
    }

    public void Flip()
	{
        sightDirection = -sightDirection;
        castOffset = -castOffset;
	}
}
