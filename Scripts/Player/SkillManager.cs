using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("스킬 설정")]
    public SkillTimer[] skills = new SkillTimer[4];

    [Header("키 설정")]
    public KeyCode[] skillKeys = { KeyCode.Mouse0, KeyCode.W, KeyCode.E, KeyCode.R };

    PlayerController playerController;
    public float powerHoldTimer;
    public float guitarFinisherEndTimer;
    public Camera playerCamera;
    public float attackRange = 5f;
    public LayerMask enemyLayer;
    public GameObject attackEffect;        // 공격 때 재생할 이펙트

    public Vector3 headPosition = new Vector3(0, 2, 0); // 플레이어 머리 위치

    void Start()
    {
        // 초기 UI 설정 (모든 스킬 사용 가능 상태)
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].cooldownImage != null)
                skills[i].cooldownImage.fillAmount = 1f;
            if (skills[i].cooldownText != null)
                skills[i].cooldownText.text = "";
        }
        ExecuteSkill(4);

        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // 타이머 업데이트
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].UpdateTimer(Time.deltaTime);
        }

        // 키 입력 처리
        for (int i = 0; i < skillKeys.Length && i < skills.Length; i++)
        {
            if (Input.GetKey(skillKeys[0]))
            {
                powerHoldTimer -= Time.deltaTime;
                if (powerHoldTimer <= 0)
                {
                    UseSkill(0);
                    powerHoldTimer = 3;
                }
            }
            if (Input.GetKeyUp(skillKeys[0]))
            {
                powerHoldTimer = 3;     // Q공격 차징
            }
            if (Input.GetKeyDown(skillKeys[2]))             //여기에 bool 값으로 동료 구출 했는지 적기 
            {
                UseSkill(2);            // C스킬 일정 시간 피해 감소
            }
            if (Input.GetKeyDown(skillKeys[1]))             //여기에 bool 값으로 동료 구출 했는지 적기 
            {
                UseSkill(1);            // E스킬 10만큼 힐 
            }
            if (Input.GetKeyDown(skillKeys[3]))
            {
                UseSkill(3);            // X 궁극기 주위 모든 적에게 피해 
            }
        }
    }

    public void UseSkill(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= skills.Length) return;

        SkillTimer skill = skills[skillIndex];

        if (skill.IsReady)
        {
            Debug.Log($"{skill.skillName} 스킬 사용!");

            // 실제 스킬 로직
            ExecuteSkill(skillIndex);

            // 쿨타임 시작
            skill.StartCooldown();
        }
        else
        {
            Debug.Log($"{skill.skillName} 아직 쿨타임 중 ({skill.currentCooldown:F1}초 남음)");
        }
    }

    // 실제 스킬 실행 로직
    void ExecuteSkill(int skillIndex)
    {
        switch (skillIndex)
        {
            case 0:  // 차징 공격 Q 꾹 
                {
                    Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

                    for(int i = 0; i < enemies.Length; i++)
                    {
                        Vector3 target = (enemies[i].transform.position - transform.position).normalized;
                        float angle = Vector3.Angle(transform.forward, target);

                        if (angle < 90f / 2)
                        {
                            EnemyHealth enemyHealth = enemies[i].GetComponent<EnemyHealth>();
                            if (enemyHealth != null)
                            {
                                enemyHealth.TakeDamage(10, true);
                            }
                        }
                    }
                    break;
                }

            case 1:     // E 힐 
                {
                    playerController.Heal(10);
                    break;
                }

            case 2:     // C 일정 시간 피해 감소
                {
                    Debug.Log("피해 감소 스킬 발동! 10초간 피해 절반");
                    // 코루틴 방식: 10초 동안 50%만 피해 적용
                    StartCoroutine(playerController.ReduceDamageForSeconds(0.5f, 10f));
                    break;
                }

            case 3:     // 궁극기 X
                {
                    Collider[] enemiess = Physics.OverlapSphere(transform.position, 40 /* AttackRange */, enemyLayer);
                    foreach (Collider enemy in enemiess)
                    {
                        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                        if (enemyHealth != null)
                        {
                            enemyHealth.TakeDamage(10, true);
                        }
                    }
                    break;
                }

            case 4:
                {
                    // 기본 상태 있으면 ㄱㄱ 
                    break;
                }
        }
    }

    // 외부에서 스킬 상태 확인용
    public bool IsSkillReady(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= skills.Length) return false;
        return skills[skillIndex].IsReady;
    }

    // 남은 쿨타임 확인용
    public float GetRemainingCooldown(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= skills.Length) return 0f;
        return skills[skillIndex].currentCooldown;
    }
}
