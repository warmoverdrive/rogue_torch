using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    float attackTime = 0.25f;

    bool isAttacking = false;
    bool isBlocking = false;

    Animator animator;
    PlayerMovementController movementController;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        movementController = GetComponent<PlayerMovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Attack") && !isBlocking)
		{
            //Attack
            StartCoroutine(Attack());
		}

        if (Input.GetButton("Block") && !isAttacking)
        {
            //block
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

        yield return new WaitForSeconds(attackTime);

        isAttacking = false;
        movementController.isTakingAction = false;
	}
}
