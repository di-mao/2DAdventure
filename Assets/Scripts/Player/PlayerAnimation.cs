using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private PlayerController playerController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        SetAnimationState();
    }

    private void SetAnimationState()
    {
        animator.SetFloat("velocityX", MathF.Abs(rb.linearVelocityX));
        animator.SetFloat("velocityY", rb.linearVelocityY);
        animator.SetBool("isGrounded", physicsCheck.isGrounded);
        animator.SetBool("isCrouch", playerController.isCrouch);
        animator.SetBool("isDead", playerController.isDead);
        animator.SetBool("isAttack", playerController.isAttack);
        
    }

    public void PlayHurtAnimation()
    {
        animator.SetTrigger("hurt");
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger("attack");
    }
}