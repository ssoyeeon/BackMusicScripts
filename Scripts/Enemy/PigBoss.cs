using UnityEngine;
using System.Collections;
using System.Collections.Generic; // List를 사용하기 위해 추가

public class PigBoss : MonoBehaviour
{
    [Header("기본 설정")]
    public Transform player;
    public float attackCooldown = 2.0f;

    [Header("공격 패턴 거리")]
    public float stompRange = 12f;
    public float jumpRange = 20f;
    public float dashRange = 30f;

    [Header("공격 상세 설정")]
    public float dashSpeed = 30f;
    public float stunDuration = 5f;

    [Header("오브젝트/프리팹 연결")]
    public GameObject stompWarningIndicator;
    public GameObject shockwavePrefab;
    public GameObject largeShockwavePrefab;
    public GameObject weakPointCollider;

    private Rigidbody rb;
    private bool isActionInProgress = false;
    private float lastAttackTime = -99f;

    // ===== 추가된 부분: 연속 패턴 방지 =====
    private enum Pattern { None, Stomp, Jump, Dash }
    private Pattern lastPattern = Pattern.None; // 마지막에 사용한 패턴을 기억
    private int consecutiveCount = 0; // 같은 패턴을 연속으로 사용한 횟수

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;

        stompWarningIndicator.SetActive(false);
        weakPointCollider.SetActive(false);
    }

    void Update()
    {
        if (isActionInProgress || Time.time < lastAttackTime + attackCooldown)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > dashRange)
        {
            return;
        }

        // ===== 수정한 부분: 패턴 선택 로직 =====
        ChooseAndExecutePattern(distanceToPlayer);
    }

    void ChooseAndExecutePattern(float distance)
    {
        // 1. 현재 거리에서 사용 가능한 패턴 목록을 만듭니다.
        List<Pattern> availablePatterns = new List<Pattern>();
        if (distance <= stompRange) availablePatterns.Add(Pattern.Stomp);
        if (distance <= jumpRange) availablePatterns.Add(Pattern.Jump);
        if (distance <= dashRange) availablePatterns.Add(Pattern.Dash);

        // 2. 만약 마지막 패턴을 2번 연속 사용했다면, 해당 패턴을 목록에서 제거합니다.
        if (consecutiveCount >= 2)
        {
            availablePatterns.Remove(lastPattern);
        }

        // 3. 사용할 수 있는 패턴이 없다면 함수를 종료합니다. (예: 돌진만 가능한데 2번 연속 쓴 경우)
        if (availablePatterns.Count == 0) return;

        // 4. 남은 목록에서 랜덤으로 패턴 하나를 고릅니다.
        lastAttackTime = Time.time;
        Pattern chosenPattern = availablePatterns[Random.Range(0, availablePatterns.Count)];

        // 5. 연속 사용 횟수를 갱신합니다.
        if (chosenPattern == lastPattern)
        {
            consecutiveCount++;
        }
        else
        {
            lastPattern = chosenPattern;
            consecutiveCount = 1;
        }

        // 6. 선택된 패턴을 실행합니다.
        switch (chosenPattern)
        {
            case Pattern.Stomp:
                StartCoroutine(StompPattern());
                break;
            case Pattern.Jump:
                StartCoroutine(JumpPattern());
                break;
            case Pattern.Dash:
                StartCoroutine(DashPattern());
                break;
        }
    }

    // (이하 패턴 코루틴들은 이전과 동일합니다.)

    IEnumerator StompPattern()
    {
        isActionInProgress = true;
        for (int i = 0; i < 3; i++)
        {
            stompWarningIndicator.transform.position = player.position;
            stompWarningIndicator.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            stompWarningIndicator.SetActive(false);
            Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);
        }
        isActionInProgress = false;
    }

    IEnumerator JumpPattern()
    {
        isActionInProgress = true;
        rb.isKinematic = true;

        stompWarningIndicator.SetActive(true);
        yield return new WaitForSeconds(1.0f);

        Vector3 jumpTargetPos = player.position;
        stompWarningIndicator.transform.position = jumpTargetPos;

        float jumpDuration = 1.5f;
        Vector3 startPos = transform.position;
        float elapsedTime = 0;
        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / jumpDuration;
            float height = Mathf.Sin(progress * Mathf.PI) * 10f;
            transform.position = Vector3.Lerp(startPos, jumpTargetPos, progress) + Vector3.up * height;
            yield return null;
        }
        transform.position = jumpTargetPos;
        stompWarningIndicator.SetActive(false);
        Instantiate(largeShockwavePrefab, transform.position, Quaternion.identity);

        rb.isKinematic = false;
        yield return new WaitForSeconds(1.5f);
        isActionInProgress = false;
    }

    IEnumerator DashPattern()
    {
        isActionInProgress = true;
        rb.velocity = transform.forward * dashSpeed;
        yield return new WaitForSeconds(2f);
        if (isActionInProgress)
        {
            rb.velocity = Vector3.zero;
            isActionInProgress = false;
        }
    }

    IEnumerator Stun()
    {
        isActionInProgress = true;
        rb.velocity = Vector3.zero;
        weakPointCollider.SetActive(true);
        yield return new WaitForSeconds(stunDuration);
        weakPointCollider.SetActive(false);
        isActionInProgress = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isActionInProgress && collision.gameObject.CompareTag("Wall"))
        {
            StopAllCoroutines();
            StartCoroutine(Stun());
        }
    }
}