using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    float attackTime = 0.25f;

    bool isAttacking = false;
    public bool isBlocking { get; private set; } = false;

    Animator animator;
    PlayerMovementController movementController;
    IAttack attackAction;
    
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        movementController = GetComponent<PlayerMovementController>();
        attackAction = GetComponent<IAttack>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Attack") && !isBlocking)
		{
            StartCoroutine(Attack());
		}

        if (Input.GetButton("Block") && !isAttacking)
        {
            isBlocking = true;
            movementController.isTakingAction = true;
            animator.SetBool("isBlocking", true);
        }

        if (Input.GetButtonUp("Block"))
		{
            isBlocking = false;
            animator.SetBool("isBlocking", false);
            movementController.isTakingAction = false;
		}
    }

    private IEnumerator Attack()
	{
        isAttacking = true;
        movementController.isTakingAction = true;
        animator.SetTrigger("attack");
        attackAction.Attack(movementController.FacingRight());

        yield return new WaitForSeconds(attackTime);

        isAttacking = false;
        movementController.isTakingAction = false;
	}
}
