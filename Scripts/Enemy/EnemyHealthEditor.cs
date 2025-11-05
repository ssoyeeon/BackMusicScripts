#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyHealth))]
public class EnemyHealthEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 그리기
        DrawDefaultInspector();

        EnemyHealth enemyHealth = (EnemyHealth)target;

        // 구분선
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("테스트 버튼들", EditorStyles.boldLabel);

        // 게임 실행 중일 때만 버튼 활성화
        GUI.enabled = Application.isPlaying;

        EditorGUILayout.BeginHorizontal();

        // 데미지 버튼
        if (GUILayout.Button($"데미지 주기 ({enemyHealth.testDamageAmount})", GUILayout.Height(30)))
        {
            enemyHealth.TestDamageButton();
        }

        // 치료 버튼
        if (GUILayout.Button($"치료하기 ({enemyHealth.testDamageAmount})", GUILayout.Height(30)))
        {
            enemyHealth.TestHealButton();
        }

        EditorGUILayout.EndHorizontal();

        // 즉시 죽이기 버튼 (빨간색)
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("즉시 죽이기", GUILayout.Height(35)))
        {
            enemyHealth.TestKillButton();
        }
        GUI.backgroundColor = Color.white;

        // 게임이 실행 중이 아닐 때 안내 메시지
        if (!Application.isPlaying)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("게임을 실행한 후 버튼을 사용하세요!", MessageType.Info);
        }
        else
        {
            // 실시간 체력 정보 표시
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("현재 상태", EditorStyles.boldLabel);

            string healthInfo = $"체력: {enemyHealth.currentHealth} / {enemyHealth.maxHealth}";
            float healthPercent = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;

            // 체력 바 색상 결정
            Color barColor = Color.green;
            if (healthPercent < 0.3f) barColor = Color.red;
            else if (healthPercent < 0.6f) barColor = Color.yellow;

            // 체력 바 그리기
            Rect rect = GUILayoutUtility.GetRect(0, 20);
            EditorGUI.DrawRect(rect, Color.gray);
            rect.width *= healthPercent;
            EditorGUI.DrawRect(rect, barColor);

            EditorGUILayout.LabelField(healthInfo, EditorStyles.centeredGreyMiniLabel);
        }

        GUI.enabled = true;

        // 자동 새로고침 (실행 중일 때)
        if (Application.isPlaying)
        {
            EditorUtility.SetDirty(target);
            Repaint();
        }
    }
}
#endif