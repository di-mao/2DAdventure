using System;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;
    [Tooltip("蜜蜂攻击范围")]
    public float attackRange;
    [Tooltip("蜜蜂攻击速率")]
    public float attackRate;

    private void OnTriggerStay2D(Collider2D other)
    {
        other.GetComponent<Character>()?.TackDamage(this);
    }
}
