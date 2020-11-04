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
	[SerializeField]
	float attackWindUp = 0.25f,
		attackCoolDown = 0.25f,
		blockTime = 1f;
	[Header("Action Properties")]
	[SerializeField]
	bool canBlock = true;
	[SerializeField]
	int attackChance = 50,
		blockChance = 30,
		idleChance = 20;

	int ratioTotal;

    public bool playerSighted { get; private set; } = false;
	public bool isPerformingAction { get; private set; } = false;

    EnemyMovement enemyMovement;
	Animator animator;
    ActorStatusController statusController;
    Vector2 sightDirection = Vector2.right;
    int PLAYER_MASK;
	GameObject target;
	IAttack attack;


	void Start()
    {
		animator = GetComponentInChildren<Animator>();
        statusController = GetComponent<ActorStatusController>();
        PLAYER_MASK = LayerMask.GetMask("Player");
        enemyMovement = GetComponent<EnemyMovement>();
		attack = GetComponent<IAttack>();
    }

    void FixedUpdate()
	{
		if (!statusController.IsDead())
		{
			SearchForPlayer();
			if (target) ChooseAction();
		}
		else if (statusController.IsDead()) StopAllCoroutines();
	}

	private void Update()
	{
		if (animator.GetBool("isDead")) return;
		else if (statusController.IsDead()) animator.SetBool("isDead", true);
	}

	public bool InAttackRange()
	{
		var distance = Vector3.Distance(transform.position, target.transform.position);
		return distance <= attack.GetAttackRange();
	}

	void ChooseAction()
	{
		if (!isPerformingAction && InAttackRange())
		{
			ratioTotal = attackChance + blockChance + idleChance;

			int randResult = Random.Range(0, ratioTotal);
			if ((randResult -= attackChance) < 0)
				StartCoroutine(AttackAction());
			else if (canBlock && (randResult -= blockChance) < 0)
				StartCoroutine(BlockingAction());
			else return;
		}
	}

	IEnumerator BlockingAction()
	{
		isPerformingAction = true;
		statusController.isBlocking = true;
		animator.SetBool("isBlocking", true);

		yield return new WaitForSeconds(blockTime);

		isPerformingAction = false;
		statusController.isBlocking = false;
		animator.SetBool("isBlocking", false);
	}

	IEnumerator AttackAction()
	{
		isPerformingAction = true;
		yield return new WaitForSeconds(attackWindUp);

		animator.SetTrigger("attack");
		attack.Attack(enemyMovement.isFacingRight);

		yield return new WaitForSeconds(attackCoolDown);

		isPerformingAction = false;
	}

	private void SearchForPlayer()
	{
		RaycastHit2D sightCast = Physics2D.CircleCast(
			transform.position + castOffset, sightCircleRadius, sightDirection, sightDistance, PLAYER_MASK);

		if (sightCast.collider)
		{
			playerSighted = true;

			target = sightCast.collider.gameObject;

			var direction = transform.position - target.transform.position;

			if (direction.x < 0) enemyMovement.FacePlayer(true);
			else enemyMovement.FacePlayer(false);
		}
		else
		{
			playerSighted = false;
			target = null;
		}
	}

	public void Flip()
	{
        sightDirection = -sightDirection;
        castOffset = -castOffset;
	}

	public void DestroySelf() => Destroy(gameObject);
}
