using UnityEngine;

public class BoarChaseState : BaseState
{
    public override void OnEnter(Enemy currentEnemy)
    {
        this.currentEnemy = currentEnemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("run", true);
    }

    public override void LogicUpdate()
    {
        // 丢失玩家目标
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }
        // 不在地面 || 撞左墙且面向左 || 撞右墙且面向右
        if (!currentEnemy.physicsCheck.isGrounded ||
            currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0 ||
            currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0)
        {
            currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDir.x, 1, 1);
        }
    }

    public override void PhysicsUpdate()
    {
        if (currentEnemy.isHurt || currentEnemy.isDead || currentEnemy.wait) return;
        currentEnemy.Move();
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("run", false);
    }
}
