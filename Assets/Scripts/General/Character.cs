using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header("受击无敌")]
    public float invulnerableDuration;
    public float invulnerableCounter;
    public bool isInvulnerable;

    [Header("受伤事件")]
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;
    [Header("改变血量")]
    public UnityEvent<Character> OnHealthChange;

    private void Start()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

    private void Update()
    {
        
        // 无敌计时器
        if (isInvulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                isInvulnerable = false;
            }
        }

        if (currentPower < maxPower)
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }

    public void TackDamage(Attack attack)
    {
        if (isInvulnerable) return;

        if (currentHealth - attack.damage > 0)
        {
            // 触发受伤
            currentHealth -= attack.damage;
            TriggerInvulnerable();
            OnTakeDamage?.Invoke(attack.transform);
        }
        else
        {
            currentHealth = 0;
            // 触发死亡
            OnDie?.Invoke();
        }
        OnHealthChange?.Invoke(this); // 激活血量发生了改变的事件
    }
    
    private void TriggerInvulnerable()
    {
        if (!isInvulnerable)
        {
            isInvulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    public void OnSlide(int cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }
}