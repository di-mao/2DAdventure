using UnityEngine;

public class SnailChaseState : BaseState
{
    public override void OnEnter(Enemy currentEnemy)
    {
        this.currentEnemy = currentEnemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }

    public override void LogicUpdate()
    {
        
    }

    public override void PhysicsUpdate()
    {
    }

    public override void OnExit()
    {
    }
}
