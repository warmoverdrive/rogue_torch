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
	Animator animator;
    IDamagable statusController;
    Vector2 sightDirection = Vector2.right;
    int PLAYER_MASK;

    void Start()
    {
		animator = GetComponentInChildren<Animator>();
        statusController = GetComponent<IDamagable>();
        PLAYER_MASK = LayerMask.GetMask("Player");
        enemyMovement = GetComponent<EnemyMovement>();
    }

    void FixedUpdate()
	{
		if (!statusController.IsDead())
			SearchForPlayer();
	}

	private void Update()
	{
		if (animator.GetBool("isDead")) return;
		else if (statusController.IsDead()) animator.SetBool("isDead", true);
	}

	private void SearchForPlayer()
	{
		RaycastHit2D sightCast = Physics2D.CircleCast(
			transform.position + castOffset, sightCircleRadius, sightDirection, sightDistance, PLAYER_MASK);

		if (sightCast.collider)
		{
			playerSighted = true;

			var player = sightCast.collider.gameObject;

			var direction = transform.position - player.transform.position;

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
