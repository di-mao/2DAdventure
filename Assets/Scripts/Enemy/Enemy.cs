using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PhysicsCheck))]
public class Enemy : MonoBehaviour
{
    #region 组件

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public PhysicsCheck physicsCheck;

    #endregion
    
    #region 基本参数

    [Header("基本参数")]
    public float normalSpeed;

    public float chaseSpeed;
    public float currentSpeed;
    [Space] public Vector3 faceDir;
    [HideInInspector] public Transform attacker;
    public float hurtForce;
    [Tooltip("出生点")]
    public Vector3 spawnPoint;

    #endregion

    #region 计时器

    [Header("计时器")]
    [Tooltip("撞墙等待计时器开关")] public bool wait; // 计时器开关

    [Tooltip("撞墙等待持续时间")] public float waitTime;
    [Tooltip("撞墙等待计时器")] public float waitTimeCounter;
    [Space] [Tooltip("丢失目标持续时间")] public float lostTime; // 丢失目标
    [Tooltip("丢失目标计时器")] public float lostTimeCounter;

    #endregion

    #region 检测

    [Header("检测")]
    public Vector2 centerOffset;

    public Vector2 checkBoxSize;
    [Tooltip("蜜蜂检查半径，野猪蜗牛向前检测距离")] public float checkDistance;
    public LayerMask attackerLayer;

    #endregion

    #region 状态

    [Header("状态")]
    public bool isHurt;

    public bool isDead;

    #endregion


    #region 状态机

    private BaseState currentState;
    protected BaseState patrolState; // 通用巡逻
    protected BaseState chaseState; // 野猪追击
    protected BaseState skillState; // 蜗牛技能

    #endregion


    #region Unity生命周期函数

    protected virtual void Awake()
    {
        // 初始化组件
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();

        // 初始化变量
        currentSpeed = normalSpeed;
        waitTimeCounter = waitTime;
        spawnPoint = transform.position;
    }

    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }


    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);
        currentState.LogicUpdate();
        TimeCounter();
    }

    private void FixedUpdate()
    {
        currentState.PhysicsUpdate();

        // if (!isHurt && !isDead && !wait) Move();
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }

    #endregion

    public virtual void Move()
    {
        rb.linearVelocityX = faceDir.x * currentSpeed * Time.deltaTime;
    }


    // 计时器
    public virtual void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale =
                    new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        if (!FoundPlayer())
        {
            if (lostTimeCounter > 0)
                lostTimeCounter -= Time.deltaTime; // 丢失玩家目标倒计时
        }
        else
        {
            lostTimeCounter = lostTime; // 重新发现玩家重置时间
        }
    }


    #region 事件方法

    /// <summary>
    /// 受伤：转向攻击者、开启受伤状态、播放受伤动画
    /// 开启协程：击退、0.5s后关闭受伤状态
    /// </summary>
    /// <param name="attackTransform">攻击者</param>
    public void TakeDamage(Transform attackTransform)
    {
        attacker = attackTransform;
        // 转身
        if (attackTransform.position.x - transform.position.x > 0) // 玩家在右，怪物在左
            transform.localScale = new Vector3(-1, 1, 1);
        if (attackTransform.position.x - transform.position.x < 0) // 玩家在左，怪物在右
            transform.localScale = new Vector3(1, 1, 1);

        // 受伤被击退
        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attackTransform.position.x, 0).normalized;
        rb.linearVelocityX = 0;
        StartCoroutine(OnHurt(dir));
    }

    private IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        isHurt = false;
    }

    /// <summary>
    /// 死亡：修改图层、更新状态、播放死亡动画
    /// </summary>
    public void OnDead()
    {
        gameObject.layer = 2;
        isDead = true;
        anim.SetBool("dead", true);
    }

    /// <summary>
    /// 播放死亡动画以后销毁游戏物体
    /// </summary>
    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }

    #endregion


    public virtual bool FoundPlayer()
    {
        return Physics2D.BoxCast(
            (Vector2)transform.position + centerOffset, // 投射起点
            checkBoxSize, // 盒子尺寸 - Vector2
            0, // 盒子角度
            faceDir, // 投射方向
            checkDistance, // 投射距离
            attackerLayer // 检测图层
        );
    }

    public void SwitchState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            NPCState.Skill => skillState,
            _ => null
        };
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
        print(gameObject.name + " 切换状态 -> " + newState);
    }

    /// <summary>
    /// 蜜蜂用来获取出生点
    /// </summary>
    /// <returns>坐标点</returns>
    public virtual Vector3 GetNewPoint()
    {
        return transform.position;
    }

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(
            transform.position + (Vector3)centerOffset + new Vector3(checkDistance * -transform.localScale.x, 0),
            checkBoxSize);
    }
}