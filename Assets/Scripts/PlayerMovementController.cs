﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Design Levers")]
    [SerializeField]
    float moveSpeed = 1f;
    [SerializeField]
    float jumpForce = 1f;
    [SerializeField]
    float dropSpeed = 1f;

    Rigidbody2D rb;
    bool facingRight = true;
    bool isJumping = false;
    bool canDrop = false;
    bool isDropping = false;

    Vector2 rightFaceScale = new Vector2(1, 1);
    Vector2 leftFaceScale = new Vector2(-1, 1);

    int groundLayer;
    int platformLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundLayer = LayerMask.NameToLayer("Ground");
        platformLayer = LayerMask.NameToLayer("Platform");
    }

    void FixedUpdate()
    {
        if (Input.GetAxis("Horizontal") != 0 && !isDropping) Move();
        Flip();
        Jump();
        if (!isDropping) Drop();
    }

    private void Drop()
    {
        if (Input.GetAxis("Drop") != 0 && canDrop == true)
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
        if (Input.GetAxis("Jump") > 0 && isJumping == false)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
        }
    }

    private void Move()
    {
        float deltaX = moveSpeed * Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(deltaX, rb.velocity.y);

        if (Mathf.Sign(deltaX) == 1) facingRight = true;
        else if (Mathf.Sign(deltaX) == -1) facingRight = false;
    }

    void Flip()
    {
        if (facingRight) transform.localScale = rightFaceScale;
        else transform.localScale = leftFaceScale;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var layer = collision.gameObject.layer;
        if (layer == groundLayer)
        {
            isJumping = false;
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