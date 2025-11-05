using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;
using DG.Tweening.Core.Easing;

public class PigBossAI : MonoBehaviour
{
    public float attackCooldown = 10.0f; // 공격 후 다음 행동까지의 대기 시간
    private float lastAttackTime = -99f; // 마지막 공격 시간 기록

    public Rigidbody rb;
    private bool isGround;
    private bool isWall;
    private bool isPlayer;
    private bool isActionInProgress = false; // 현재 행동 중인지 확인하는 플래그

    public PlayerController playerController;
    public GameObject player;

    public EnemyHealth enemyHealth;
    public int plusDamage = 5;
    public EnemyAI enemyAI;               //내비게이션
                                      
    [Header("공격 설정")]
    public Vector3 boxSize = new Vector3(10f, 10f, 100f); // 공격 범위 상자의 크기 (가로, 높이, 길이)
    public float boxDistance = 5f;  // 내 위치에서 상자 중심까지의 거리
    public LayerMask targetLayer;     // 공격할 대상의 레이어

    [Header("효과")]
    public float attackDuration = 3.0f; // 공격이 지속되는 시간
    // public GameObject roarEffectPrefab; // 시각 효과가 있다면 연결

    private bool isAttacking = false;

    public NavMeshAgent navMeshAgent;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyAI = GetComponent<EnemyAI>();
        playerController = FindObjectOfType<PlayerController>();
        player = playerController.gameObject;
    }

    void Update()
    {
        // 행동 중이거나 쿨타임이면 아무것도 안함
        if (isActionInProgress || Time.time < lastAttackTime + attackCooldown)
        {
            return;
        }

        // 쿨타임이 지났으면 다음 패턴을 랜덤으로 실행
        RandomPattern();

        if(enemyHealth.currentHealth <= 0)
        {
            StartCoroutine(Clear());
        }
    }

    IEnumerator Clear()
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("클리어");
        SceneManager.Instance.LoadGameScene();
    }

    // 내려찍기 패턴
    IEnumerator StompPattern()
    {
        navMeshAgent.isStopped = true;
        isActionInProgress = true;
        for (int i = 0; i < 3; i++)
        {
            isGround = false;
            //rb.velocity = Vector3.up * 25000f; // 점프 높이 조절

            //yield return new WaitUntil(() => isGround);         //WaitUntil -> isGround가 true 값이 될 때까지 기다림. 이 줄에서 영원히 우하하하하하

            StartCoroutine(ShockwaveDamageRoutine());

            yield return new WaitForSeconds(0.5f);
        }
        lastAttackTime = Time.time; // 패턴이 끝나면 쿨타임 시작
        navMeshAgent.isStopped = false;
        isActionInProgress = false;
    }

    IEnumerator AAAAAAAAAAAAAPattern()
    {
        isAttacking = true;

        float attackTimer = 0f;      // 전체 포효 지속 시간 타이머
        float damageTickTimer = 0f;    // 다음 대미지 틱(Tick)까지의 시간을 재는 타이머
        float damageInterval = 1.0f; // 대미지가 들어가는 간격 (1초)

        // 1. 전체 포효 지속 시간(3초) 동안 매 프레임 반복합니다.
        while (attackTimer < attackDuration)
        {
            // 2. 틱 타이머를 매 프레임마다 증가시킵니다.
            damageTickTimer += Time.deltaTime;

            // 3. 틱 타이머가 대미지 간격(1초)에 도달했는지 확인합니다.
            if (damageTickTimer >= damageInterval)
            {
                Debug.Log("대미지 틱!");
                Vector3 boxCenter = transform.position + transform.forward * boxDistance;
                Collider[] hitTargets = Physics.OverlapBox(boxCenter, boxSize / 2f, transform.rotation, targetLayer);

                // 4. 범위 안의 모든 대상에게 대미지를 줍니다.
                // 공격 대상이 1명이므로, 첫 번째 대상에게만 피해를 줍니다.
                if (hitTargets.Length > 0)
                {
                    playerController.Damaged(10);
                }

                // 5. 틱 타이머를 리셋해서 다음 1초를 다시 세도록 합니다.
                damageTickTimer = 0f;
            }

            attackTimer += Time.deltaTime; // 전체 포효 시간 타이머는 계속 증가
            yield return null; // 다음 프레임까지 대기
        }

        Debug.Log("포효 종료!");
        isAttacking = false;
    }

    

    IEnumerator ShockwaveDamageRoutine()
    {
        float shockwaveTimer = 0f;
        float shockwaveDuration = 2f;
        float shockwaveSpeed = 5f;
        float shockwaveWidth = 1f;
        bool hasDamagedPlayer = false;

        while (shockwaveTimer < shockwaveDuration)
        {
            shockwaveTimer += Time.deltaTime;
            float currentRadius = shockwaveTimer * shockwaveSpeed;
            DrawCircle(transform.position, currentRadius, 32);

            Vector3 bossP = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 playerP = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            float distanceToPlayer = Vector3.Distance(bossP, playerP);

            if (Mathf.Abs(distanceToPlayer - currentRadius) <= shockwaveWidth / 2f && !hasDamagedPlayer)
            {
                if (player.transform.position.y < 0.5f)
                {
                    if(enemyHealth.currentHealth <= 300)
                    {
                        playerController.Damaged(30 + plusDamage);
                        hasDamagedPlayer = true;
                    }
                    else
                    {
                        playerController.Damaged(30);
                        hasDamagedPlayer = true;
                    }
                }
            }
            yield return null;
        }
    }

    // 돌진 패턴 (수정됨)
    IEnumerator DashPattern()
    {
        navMeshAgent.isStopped = true;
        isActionInProgress = true;
        isWall = false; // 돌진 시작 전, 벽 충돌 상태 초기화

        float dashDuration = 1f; // 1초 동안 돌진
        float dashTimer = 0f;
        bool hasDamagedPlayer = false;

        while (dashTimer < dashDuration && !isWall && !isPlayer) // 1초 동안 또는 벽에 부딪히기 전까지
        {
            rb.velocity = transform.forward * 40f; // 앞으로 계속 힘을 줌
            dashTimer += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector3.zero; // 돌진 후 정지

        if (isPlayer || !hasDamagedPlayer)
        {
            if (enemyHealth.currentHealth <= 300)
            {
                playerController.Damaged(30 + plusDamage);
                hasDamagedPlayer = true;
            }
            else
            {
                playerController.Damaged(30 + plusDamage);
                hasDamagedPlayer = true;
            }
        }

        if (isWall)
        {
            Debug.Log("벽에 충돌! 2초 스턴!");
            yield return new WaitForSeconds(2f);
            isWall = false;
        }


        lastAttackTime = Time.time; // 패턴이 끝나면 쿨타임 시작
        isActionInProgress = false;
        navMeshAgent.isStopped = false;
    }

    void RandomPattern()
    {
        int randomNumber = Random.Range(0,4);
        Debug.Log(randomNumber);
        if (randomNumber == 1 || randomNumber == 0)
        {
            Debug.Log("대쉬");
            StartCoroutine(DashPattern());
        }
        else if (randomNumber == 2)
        {
            Debug.Log("쿵 내려찍기");
            StartCoroutine(StompPattern());
        }
        else if(randomNumber == 3)
        {
            StartCoroutine(AAAAAAAAAAAAAPattern());
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGround = true;
        }
        if (collision.gameObject.tag == "Wall")
        {
            isWall = true;
        }
        if (collision.gameObject.tag == "Player")
        {
            isPlayer = true;
        }
    }
    
    // Scene 뷰에서 공격 범위를 시각적으로 보여주는 기능 (테스트용)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Gizmos가 오브젝트의 회전을 따라가도록 Matrix 설정
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;

        // 회전된 좌표계 기준으로 박스를 그림
        Gizmos.DrawWireCube(Vector3.forward * boxDistance, boxSize);
    }

    void DrawCircle(Vector3 center, float radius, int segments)
    {
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * 360 * Mathf.Deg2Rad;
            float nextAngle = (float)(i + 1) / segments * 360 * Mathf.Deg2Rad;
            Vector3 startPoint = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Vector3 endPoint = center + new Vector3(Mathf.Cos(nextAngle), 0, Mathf.Sin(nextAngle)) * radius;
            Debug.DrawLine(startPoint, endPoint, Color.yellow);
        }
    }
}