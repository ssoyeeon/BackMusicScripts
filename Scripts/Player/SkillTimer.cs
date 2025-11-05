using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class SkillTimer
{
    public string skillName;
    public float cooldownTime;
    public Image cooldownImage;
    public TMP_Text cooldownText;

    [System.NonSerialized]
    public float currentCooldown;

    public bool IsReady => currentCooldown <= 0f;

    public void StartCooldown()
    {
        currentCooldown = cooldownTime;

        // 쿨타임 시작 시 UI를 0(투명)으로 설정
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f;
        }
    }

    public void UpdateTimer(float deltaTime)
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= deltaTime;

            // UI 업데이트 (쿨타임이 줄어들수록 fillAmount 증가)
            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = 1f - (currentCooldown / cooldownTime);
            }

            if (cooldownText != null)
            {
                cooldownText.text = currentCooldown > 0f ? currentCooldown.ToString("F1") : "";
            }
        }
        else
        {
            // 쿨타임이 끝났을 때 UI를 1(완전히 보이게)
            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = 1f;
            }
        }
    }
}
