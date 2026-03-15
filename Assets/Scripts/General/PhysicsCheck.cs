using System;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class PhysicsCheck : MonoBehaviour
{
    public bool isPlayer;
    [Tooltip("手动调节参数")] public bool manualAdjustParameter;

    [Header("检测状态")]
    public bool isGrounded;

    public bool touchLeftWall;
    public bool touchRightWall;
    public bool onWall;

    [Header("检测参数")]
    public Vector2 bottomOffset;

    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public float checkRadius;
    public LayerMask groundLayer;

    private CapsuleCollider2D coll;
    private PlayerController playerController;
    private Rigidbody2D rb;

    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
        if (isPlayer) playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        
        if (!manualAdjustParameter)
        {
            bottomOffset = new Vector2(coll.offset.x, coll.offset.y - coll.size.y / 2);
            rightOffset = new Vector2(coll.offset.x + coll.bounds.size.x / 2, coll.offset.y);
            leftOffset = new Vector2(coll.offset.x - coll.bounds.size.x / 2, coll.offset.y);
        }

    }

    private void Update()
    {
        Check();
    }

    private void Check()
    {
        isGrounded = Physics2D.OverlapCircle(
            (Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y),
            checkRadius, groundLayer);
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRadius, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRadius, groundLayer);
        if (isPlayer)
            onWall = (touchLeftWall && playerController.inputDirection.x < 0f ||
                      touchRightWall && playerController.inputDirection.x > 0f) && rb.linearVelocityY < 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position +
                              new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y),
            checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRadius);
    }
}