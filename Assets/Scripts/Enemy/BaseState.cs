using UnityEngine;

public abstract class BaseState
{
    protected Enemy currentEnemy;
    public abstract void OnEnter(Enemy currentEnemy);
    public abstract void LogicUpdate();
    public abstract void PhysicsUpdate();
    public abstract void OnExit();
}
