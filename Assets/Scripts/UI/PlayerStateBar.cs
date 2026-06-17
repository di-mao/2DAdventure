using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateBar : MonoBehaviour
{
    public Image healthImage;
    public Image healthDelayImage;
    public Image powerImage;
    [Tooltip("能量条在恢复")]
    private bool isRecovering;

    private Character currentCharacter;

    private void Update()
    {
        if (healthImage.fillAmount < healthDelayImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime;
        }

        if (isRecovering)
        {
            float percentage = currentCharacter.currentPower / currentCharacter.maxPower;
            powerImage.fillAmount = percentage;
            if (percentage >= 1)
            {
                isRecovering = false;
                return;
            }

        }
    }

    /// <summary>
    /// 接收血量百分比变化
    /// </summary>
    /// <param name="percentage">百分比</param>
    public void OnHealthChange(float percentage)
    {
        healthImage.fillAmount = percentage;
    }

    public void OnPowerChange(Character character)
    {
        isRecovering = true;
        currentCharacter = character;
    }
}
