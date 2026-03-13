using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl InputControl;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private CapsuleCollider2D capsuleCollider;
    private PlayerAnimation playerAnimation;

    public bool isCrouch;

    [Header("基本参数")]
    public Vector2 inputDirection;

    public float speed;
    private float runSpeed;
    private float walkSpeed => speed / 2.5f; // 每次调用 walkSpeed 都会执行一次 Lambda 表达式
    public float jumpForce;

    [Header("受伤")]
    public bool isDead;

    public bool isHurt;
    public float hurtForce;
    
    [Header("攻击")]
    public bool isAttack;
    
    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    // 碰撞体
    private Vector2 originalOffset;
    private Vector2 originalSize;

    private void Awake()
    {
        InputControl = new PlayerInputControl();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();

        // 记录碰撞体大小
        originalOffset = capsuleCollider.offset;
        originalSize = capsuleCollider.size;

        InputControl.Gameplay.Jump.started += Jump;

        #region 下蹲

        runSpeed = speed;
        InputControl.Gameplay.Walk.performed += ctx =>
        {
            if (physicsCheck.isGrounded) speed = walkSpeed;
        };
        InputControl.Gameplay.Walk.canceled += ctx =>
        {
            if (physicsCheck.isGrounded) speed = runSpeed;
        };

        #endregion

        InputControl.Gameplay.Attack.started += PlayerAttack;
    }


    private void OnEnable()
    {
        InputControl.Enable();
    }

    private void OnDisable()
    {
        InputControl.Disable();
    }

    private void Update()
    {
        inputDirection = InputControl.Gameplay.Move.ReadValue<Vector2>();
        CheckState();
    }

    private void FixedUpdate()
    {
        if (!isHurt && !isAttack) Move();
    }


    private void Move()
    {
        // 移动
        if (!isCrouch)
            rb.linearVelocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.linearVelocity.y);
        // 翻转
        int faceDirection = (int)transform.localScale.x;
        if (inputDirection.x > 0) faceDirection = 1;
        else if (inputDirection.x < 0) faceDirection = -1;
        transform.localScale = new Vector3(faceDirection, 1f, 1f);
        // 下蹲
        isCrouch = physicsCheck.isGrounded && inputDirection.y < -0.5f;
        if (isCrouch)
        {
            // 缩小
            capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, originalOffset.y / 2);
            capsuleCollider.size = new Vector2(capsuleCollider.size.x, originalSize.y / 2);
        }
        else
        {
            // 还原
            capsuleCollider.offset = originalOffset;
            capsuleCollider.size = originalSize;
        }
    }

    #region InputBinding

    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGrounded)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        playerAnimation.PlayAttackAnimation(); 
        isAttack = true;
    }

    #endregion


    #region UnityEvent

    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.linearVelocity = Vector2.zero;
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized;
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    public void PlayerDead()
    {
        isDead = true;
        InputControl.Gameplay.Disable();
    }

    #endregion
    
    private void CheckState()
    {
        capsuleCollider.sharedMaterial = physicsCheck.isGrounded ? normal : wall;
    }
}