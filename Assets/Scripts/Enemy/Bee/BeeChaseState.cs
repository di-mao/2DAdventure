using UnityEngine;

public class BeeChaseState : BaseState
{
    public Vector3 targetPos;
    public Vector3 moveDir;
    public Attack attack;

    public override void OnEnter(Enemy currentEnemy)
    {
        this.currentEnemy = currentEnemy;
        attack = currentEnemy.GetComponent<Attack>();
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
    }

    public override void LogicUpdate()
    {
        // 丢失玩家目标
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }

        targetPos = new Vector3(
            currentEnemy.attacker.position.x,
            currentEnemy.attacker.position.y + 1.5f,
            0
        );
        // 玩家位于蜜蜂攻击范围
        if (Mathf.Abs(targetPos.x - currentEnemy.transform.position.x) <= attack.attackRange &&
            Mathf.Abs(targetPos.y - currentEnemy.transform.position.y) <= attack.attackRange)
        {
            currentEnemy.rb.linearVelocity = Vector3.zero;
            
        }
        
        // 朝向玩家
        moveDir = (targetPos - currentEnemy.transform.position).normalized;
        if (moveDir.x > 0) currentEnemy.transform.localScale = new Vector3(-1, 1, 1); // 面朝右
        if (moveDir.x < 0) currentEnemy.transform.localScale = new Vector3(1, 1, 1); // 面朝左
    }

    public override void PhysicsUpdate()
    {
        if (!currentEnemy.isHurt && !currentEnemy.isDead)
        {
            currentEnemy.rb.linearVelocity = moveDir * (currentEnemy.chaseSpeed * Time.deltaTime);
            Debug.DrawLine(currentEnemy.transform.position, targetPos, Color.red);
        }
        else
        {
            currentEnemy.rb.linearVelocity = Vector3.zero;
        }
    }

    public override void OnExit()
    {
    }
}