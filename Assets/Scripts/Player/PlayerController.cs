using System;
using System.Collections;
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
    public float slideSpeed;
    public float slideDistance;
    public float slidePowerCost;
    private float runSpeed;
    private float walkSpeed => speed / 2.5f; // 每次调用 walkSpeed 都会执行一次 Lambda 表达式
    public float jumpForce;
    [Tooltip("蹬墙跳的力")] 
    public float jumpWallForce;
    
    [Header("状态")]
    [Tooltip("表示是否处于蹬墙跳的过程中")]
    public bool wallJump;
    public bool isDead;
    public bool isHurt;
    public float hurtForce;
    [Tooltip("攻击")]
    public bool isAttack;
    [Tooltip("是否在滑铲")]
    public bool isSlide;
    
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
        InputControl.Gameplay.Slide.started += Slide;
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
        if (!isCrouch && !wallJump)
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
            isSlide = false;
            StopAllCoroutines();    // 打断滑铲
        }
        else if (physicsCheck.onWall)
        {
            rb.AddForce(new Vector2(-inputDirection.x, 2.5f) * jumpWallForce, ForceMode2D.Impulse);
            wallJump = true;
        }
    }

    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        playerAnimation.PlayAttackAnimation();
        isAttack = true;
    }
    
    private void Slide(InputAction.CallbackContext obj)
    {
        if (!isSlide && !physicsCheck.isGrounded)
        {
            isSlide = true;
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);
            StartCoroutine(TriggerSlide(targetPos));
        }
    }

    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            yield return null;
            if (!physicsCheck.isGrounded) 
                break;

            // 撞墙提前结束滑铲
            if (physicsCheck.touchLeftWall && transform.localScale.x < 0 || physicsCheck.touchRightWall && transform.localScale.x > 0)
            {
                isSlide = false;
                break;
            }
            
            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed, transform.position.y));
        } while (Mathf.Abs(target.x - transform.position.x) > 0.1f);

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        
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
        // 玩家在墙上滑
        if (physicsCheck.onWall)
        {
            rb.linearVelocityY /= 2;
        }
        else
        {
            rb.linearVelocityY = rb.linearVelocityY;
        }

        if (wallJump && rb.linearVelocityY < 0f)
        {
            wallJump = false;
        }
    }
}