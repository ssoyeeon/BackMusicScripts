using UnityEngine;
using System;
using System.Collections;
using Unity.VisualScripting;

public class EnemyHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("죽음 효과")]
    public GameObject deathEffect;        // 죽을 때 재생할 이펙트
    public GameObject damagePerfectEffect;        // 정박 맞았을 때 재생 이펙트
    public GameObject damageBadEffect;        // 정박 맞았을 때 재생 이펙트
    public float destroyDelay = 0.5f;     // 죽은 후 오브젝트 삭제까지의 딜레이

    [Header("테스트 설정")]
    public float testDamageAmount = 25f;     // 테스트용 데미지 양

    // 적이 죽었을 때 스포너에게 알리기 위한 이벤트
    public event Action OnEnemyDeath;

    public GameObject player;
    public PlayerController playerController;
    private bool isAttacking = false;

    void Start()
    {
        currentHealth = maxHealth;
        playerController = FindObjectOfType<PlayerController>();
        player = playerController.gameObject;
    }

    void Update()
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackDamage());
        }
    }

    IEnumerator AttackDamage()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < 5f)
            {
                isAttacking = true;
                playerController.Damaged(10);
                yield return new WaitForSeconds(3.0f); 
                isAttacking = false;
            }
        }
    }

    // 즉시 죽이는 함수
    public void Die()
    {
        currentHealth = 0;

        // 죽음 이펙트 재생
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
        }

        // 스포너에게 죽음 알림
        OnEnemyDeath?.Invoke();

        Debug.Log($"{gameObject.name}이 죽었습니다!");

        // 오브젝트 삭제
        Destroy(gameObject, destroyDelay);
    }

    // 강제로 즉시 죽이는 함수 (체력 상관없이)
    public void ForceKill()
    {
        // 죽음 이펙트 재생
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
        }

        // 스포너에게 죽음 알림
        OnEnemyDeath?.Invoke();

        currentHealth = 0;
        Debug.Log($"{gameObject.name}이 강제로 죽었습니다!");

        // 오브젝트 삭제
        Destroy(gameObject, destroyDelay);
    }

    // 체력 회복 함수
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);

        Debug.Log($"{gameObject.name}이 {amount} 체력을 회복했습니다. 현재 체력: {currentHealth}");
    }

    // 테스트용 - 인스펙터에서 우클릭으로 실행 가능
    [ContextMenu("데미지 테스트")]
    void TestDamage()
    {
        TakeDamage(testDamageAmount, true);
    }

    [ContextMenu("즉시 죽이기")]
    void TestDie()
    {
        ForceKill(); // 강제 죽이기 사용
    }

    // 인스펙터 버튼용 함수들
    public void TestDamageButton()
    {
        if (Application.isPlaying)
        {
            TakeDamage(testDamageAmount , true);
        }
        else
        {
            Debug.LogWarning("게임이 실행 중일 때만 데미지를 줄 수 있습니다!");
        }
    }

    public void TestKillButton()
    {
        if (Application.isPlaying)
        {
            ForceKill(); // Die() 대신 ForceKill() 사용
        }
        else
        {
            Debug.LogWarning("게임이 실행 중일 때만 적을 죽일 수 있습니다!");
        }
    }

    public void TestHealButton()
    {
        if (Application.isPlaying)
        {
            Heal(testDamageAmount);
        }
        else
        {
            Debug.LogWarning("게임이 실행 중일 때만 치료할 수 있습니다!");
        }
    }

    public void TakeDamage(float damage, bool isRhythmic)
    {
        currentHealth -= damage;

        // 리듬에 따른 효과 구분
        if (isRhythmic)
        {
            StartCoroutine(RhythmicDamageEffect()); // 초록색
            Instantiate(damagePerfectEffect, transform.position, transform.rotation);      //대미지 이펙트 
        }
        else
        {
            StartCoroutine(NormalDamageEffect()); // 빨간색
            if(damageBadEffect != null) Instantiate(damageBadEffect, transform.position, transform.rotation);      //대미지 이펙트 
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator RhythmicDamageEffect()
    {
        // 적이 초록색으로 깜빡이는 효과 (정박 히트)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.green;
            yield return new WaitForSeconds(0.15f);
            renderer.material.color = originalColor;
        }
    }

    IEnumerator NormalDamageEffect()
    {
        // 적이 빨간색으로 깜빡이는 효과 (정박 미스)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            renderer.material.color = originalColor;
        }
    }
}