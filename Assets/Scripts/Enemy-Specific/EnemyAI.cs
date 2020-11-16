using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
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
	float actionRange = 3f,
		actionWindUp = 0.25f,
		actionCoolDown = 0.25f,
		blockTime = 1f;
	[SerializeField]
	AudioClip alertSound;

	[Header("Action Properties")]
	[SerializeField]
	bool canBlock = true;
	[SerializeField]
	int attackChance = 50,
		blockChance = 30,
		idleChance = 20;

	int ratioTotal;
	bool alerted = false;
    public bool playerInSight { get; private set; } = false;
	public bool isPerformingAction { get; private set; } = false;

    EnemyMovement enemyMovement;
	Animator animator;
    ActorStatusController statusController;
    Vector2 sightDirection = Vector2.right;
    int PLAYER_MASK;
	GameObject target;
	IAttack attack;
	AudioSource audioSource;


	void Start()
    {
		animator = GetComponentInChildren<Animator>();
        statusController = GetComponent<ActorStatusController>();
        PLAYER_MASK = LayerMask.GetMask("Player");
        enemyMovement = GetComponent<EnemyMovement>();
		attack = GetComponent<IAttack>();
		audioSource = GetComponent<AudioSource>();
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



		if (playerInSight && !alerted)
		{
			audioSource.PlayOneShot(alertSound);
			alerted = true;
		}

		else if (statusController.IsDead()) animator.SetBool("isDead", true);
	}

	public bool InActionRange()
	{
		var distance = Vector3.Distance(transform.position, target.transform.position);
		return distance <= actionRange;
	}

	void ChooseAction()
	{
		if (!isPerformingAction && InActionRange())
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
		yield return new WaitForSecondsRealtime(actionWindUp);

		statusController.isBlocking = true;
		animator.SetBool("isBlocking", true);

		yield return new WaitForSecondsRealtime(blockTime);

		statusController.isBlocking = false;
		animator.SetBool("isBlocking", false);

		yield return new WaitForSecondsRealtime(actionCoolDown);
		isPerformingAction = false;
	}

	IEnumerator AttackAction()
	{
		isPerformingAction = true;
		yield return new WaitForSecondsRealtime(actionWindUp);

		animator.SetTrigger("attack");
		attack.Attack(enemyMovement.isFacingRight);

		yield return new WaitForSecondsRealtime(actionCoolDown);
		isPerformingAction = false;
	}

	private void SearchForPlayer()
	{
		RaycastHit2D sightCast = Physics2D.CircleCast(
			transform.position + castOffset, sightCircleRadius, sightDirection, sightDistance, PLAYER_MASK);

		if (sightCast.collider)
		{
			playerInSight = true;

			target = sightCast.collider.gameObject;

			var direction = transform.position - target.transform.position;

			if (direction.x < 0) enemyMovement.FacePlayer(true);
			else enemyMovement.FacePlayer(false);
		}
		else
		{
			playerInSight = false;
			target = null;
			alerted = false;
		}
	}

	public void Flip()
	{
        sightDirection = -sightDirection;
        castOffset = -castOffset;
	}

	public void DestroySelf() => Destroy(gameObject);
}
