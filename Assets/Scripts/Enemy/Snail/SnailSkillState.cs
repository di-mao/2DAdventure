using UnityEngine;

/// <summary>
/// 蜗牛缩壳技能
/// </summary>
public class SnailSkillState : BaseState
{
    public override void OnEnter(Enemy currentEnemy)
    {
        this.currentEnemy = currentEnemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
        currentEnemy.anim.SetBool("walk", false);
        currentEnemy.anim.SetBool("hiding", true);
        currentEnemy.anim.SetTrigger("skill");
        
        currentEnemy.GetComponent<Character>().invulnerableCounter = 5f;
        currentEnemy.GetComponent<Character>().isInvulnerable = true;
    } 

    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }

        // 蜗牛的无敌不需要计时器，这里是为了让蜗牛保持无敌状态
        currentEnemy.GetComponent<Character>().invulnerableCounter = 5f;
    }

    public override void PhysicsUpdate()
    {
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("hiding", false);
        currentEnemy.GetComponent<Character>().isInvulnerable = false;
        
    }
}
