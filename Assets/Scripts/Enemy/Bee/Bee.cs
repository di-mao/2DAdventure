using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bee : Enemy
{
    [Header("移动范围")]
    public float patrolRadius;
    
    protected override void Awake()
    {
        base.Awake();
        patrolState = new BeePatrolState();
        chaseState = new BeeChaseState();
    }

    public override void Move()
    {
        
    }

    public override void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
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

    public override bool FoundPlayer()
    {
        var obj = Physics2D.OverlapCircle(transform.position, checkDistance, attackerLayer);
        if (obj)
        {
            attacker = obj.transform;
            // print(attacker.name + " : " + attacker.position);
        }
        return obj;
    }

    public override Vector3 GetNewPoint()
    {
        var targetX = Random.Range(-patrolRadius, patrolRadius);
        var targetY = Random.Range(-patrolRadius, patrolRadius);
        return spawnPoint + new Vector3(targetX, targetY);
    }

    public override void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, checkDistance); // 检测玩家范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnPoint, new Vector3(2*patrolRadius, 2*patrolRadius)); // 巡逻范围
    }
    
}
