using UnityEngine;

public class BeePatrolState : BaseState
{
    public Vector3 targetPos;
    public Vector3 moveDir;

    public override void OnEnter(Enemy currentEnemy)
    {
        this.currentEnemy = currentEnemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
        targetPos = currentEnemy.GetNewPoint();
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }

        // 到达目标点
        if (Mathf.Abs(targetPos.x - currentEnemy.transform.position.x) < 0.1f &&
            Mathf.Abs(targetPos.y - currentEnemy.transform.position.y) < 0.1f)
        {
            currentEnemy.wait = true;
            targetPos = currentEnemy.GetNewPoint();
        }

        // 面朝像玩家的方向
        moveDir = (targetPos - currentEnemy.transform.position).normalized;
        if (moveDir.x > 0) currentEnemy.transform.localScale = new Vector3(-1, 1, 1); // 面朝右
        if (moveDir.x < 0) currentEnemy.transform.localScale = new Vector3(1, 1, 1); // 面朝左
    }

    public override void PhysicsUpdate()
    {
        if (!currentEnemy.isHurt && !currentEnemy.isDead && !currentEnemy.wait)
        {
            currentEnemy.rb.linearVelocity = moveDir * (currentEnemy.normalSpeed * Time.deltaTime);
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