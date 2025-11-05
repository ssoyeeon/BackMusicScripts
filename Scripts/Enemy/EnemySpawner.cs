using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("스포너 설정")]
    public GameObject enemyPrefab;        // 스폰할 적 프리팹
    public float respawnDelay = 3f;       // 리스폰 딜레이 (초)
    public Transform spawnPoint;          // 스폰 위치 (비어있으면 자신의 위치)

    [Header("기즈모 설정")]
    public bool showGizmo = true;         // 기즈모 표시 여부
    public Color gizmoColor = Color.red;  // 기즈모 색상
    public float gizmoSize = 1f;          // 기즈모 크기

    [Header("상태 표시")]
    public bool isEnemyAlive = false;     // 적이 살아있는지 확인용

    private GameObject currentEnemy;      // 현재 스폰된 적
    private float respawnTimer = 0f;      // 리스폰 타이머
    private bool waitingToRespawn = false; // 리스폰 대기 상태

    void Start()
    {
        // 스폰 위치가 설정되지 않았으면 자신의 위치 사용
        if (spawnPoint == null)
            spawnPoint = transform;

        // 시작할 때 첫 번째 적 스폰
        SpawnEnemy();
    }

    void Update()
    {
        // 적이 죽었고 리스폰 대기 중이면 타이머 감소
        if (waitingToRespawn)
        {
            respawnTimer -= Time.deltaTime;

            if (respawnTimer <= 0f)
            {
                SpawnEnemy();
                waitingToRespawn = false;
            }
        }

        // 현재 적이 존재하는지 확인
        CheckEnemyStatus();
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        // 새로운 적 생성
        currentEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        isEnemyAlive = true;

        // 적이 죽었을 때 알림받기 위해 EnemyHealth 컴포넌트 확인
        EnemyHealth enemyHealth = currentEnemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnEnemyDeath += OnEnemyDied;
        }

        Debug.Log($"적이 스폰되었습니다: {currentEnemy.name}");
    }

    void CheckEnemyStatus()
    {
        // 현재 적이 존재하지 않으면 (삭제되었으면)
        if (currentEnemy == null && isEnemyAlive)
        {
            OnEnemyDied();
        }
    }

    void OnEnemyDied()
    {
        isEnemyAlive = false;
        waitingToRespawn = true;
        respawnTimer = respawnDelay;

        Debug.Log($"{respawnDelay}초 후에 적이 리스폰됩니다.");
    }

    // 수동으로 적을 즉시 스폰하는 함수 (테스트용)
    [ContextMenu("즉시 스폰")]
    public void ForceSpawn()
    {
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
        }

        waitingToRespawn = false;
        SpawnEnemy();
    }

    // 씬 뷰에서 기즈모 그리기
    void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;

        // 스포너 위치에 구체 그리기
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(position, gizmoSize);

        // 스포너가 활성화되어 있으면 채워진 구체도 그리기
        if (Application.isPlaying && isEnemyAlive)
        {
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
            Gizmos.DrawSphere(position, gizmoSize * 0.8f);
        }

        // 대기 중일 때는 다른 색상으로 표시
        if (Application.isPlaying && waitingToRespawn)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(position, gizmoSize * 1.2f);
        }
    }

    // 선택되었을 때 기즈모 그리기 (더 상세한 정보 표시)
    void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;

        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;

        // 스포너 범위 표시
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(position, Vector3.one * gizmoSize * 2f);

#if UNITY_EDITOR
        // 스포너 정보를 3D 텍스트로 표시
        UnityEditor.Handles.color = Color.white;

        string spawnerInfo = "PAWNER";
        string monsterInfo = enemyPrefab != null ? $"{enemyPrefab.name}" : "No Prefab";
        string statusInfo = "";

        if (Application.isPlaying)
        {
            if (isEnemyAlive)
                statusInfo = "Enemy Alive";
            else if (waitingToRespawn)
                statusInfo = $"Respawn in {respawnTimer:F1}s";
            else
                statusInfo = "Ready to Spawn";
        }
        else
        {
            statusInfo = $"Respawn Delay: {respawnDelay}s";
        }

        // 텍스트 위치 계산
        Vector3 textPos = position + Vector3.up * (gizmoSize + 1f);

        // GUI 스타일 설정
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        style.alignment = TextAnchor.MiddleCenter;

        // 텍스트 표시
        UnityEditor.Handles.Label(textPos, spawnerInfo, style);
        UnityEditor.Handles.Label(textPos + Vector3.down * 0.5f, monsterInfo, style);
        UnityEditor.Handles.Label(textPos + Vector3.down * 1f, statusInfo, style);
#endif
    }
}