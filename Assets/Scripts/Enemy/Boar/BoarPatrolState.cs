using UnityEngine;

public class BoarPatrolState : BaseState
{
    public override void OnEnter(Enemy currentEnemy)
    {
        this.currentEnemy = currentEnemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }

    public override void LogicUpdate()
    {
        // 发现玩家，切换状态
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }
        
        // 不在地面 || 撞左墙且面向左 || 撞右墙且面向右
        if (!currentEnemy.physicsCheck.isGrounded ||
            currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0 ||
            currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0)
        {
            currentEnemy.wait = true; // 开启 wait 计时器，2s 后自动关闭并转身
            currentEnemy.anim.SetBool("walk", false);
        }
        else
        {
            currentEnemy.anim.SetBool("walk", true);
        }
    }

    public override void PhysicsUpdate()
    {
        if (!currentEnemy.isHurt && !currentEnemy.isDead && !currentEnemy.wait) 
            currentEnemy.Move();
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("walk", false);
    }
}