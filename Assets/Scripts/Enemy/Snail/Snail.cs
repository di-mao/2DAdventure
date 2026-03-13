using UnityEngine;

public class Snail : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new SnailPatrolState();
        chaseState = new SnailChaseState();
        skillState = new SnailSkillState();
    }
}
