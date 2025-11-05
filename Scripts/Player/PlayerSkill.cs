using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PlayerSkill : MonoBehaviour
{
    public EnemyHealth enemy;
    PlayerController PC;
    public float beatShotCoolTimer;
    public float powerCoolTimer;
    public float powerTimer;
    public float guitarFinisherCoolTimer; 
    public float SonicRoarCoolTimer;  
    public Camera playerCamera;
    public float attackRange = 3f;
    public LayerMask enemyLayer;

    public KeyCode PowerRoarKey = KeyCode.Q;          //차지
    public KeyCode BeatShotKey = KeyCode.E;          //리듬
    public KeyCode GuitarFinisherKey = KeyCode.X;        //궁

    private void Awake()
    {
        beatShotCoolTimer = 0;
        powerCoolTimer = 0;
        guitarFinisherCoolTimer = 0;
        SonicRoarCoolTimer = 0;
    }

    private void Start()
    {
        enemy = FindObjectOfType<EnemyHealth>();
        PC = FindObjectOfType<PlayerController>();
    }
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            PlayerStates(States.SonicRoar);
        }

        if(Input.GetKey(PowerRoarKey))
        {
            powerTimer += Time.deltaTime;
            Debug.Log(powerTimer);
            if(powerTimer >= 3)
            {
                PlayerStates(States.PowerRoar);
            }

        }

        if (Input.GetKeyDown(BeatShotKey))
        {
            PlayerStates(States.BeatShot);

        }

        if(Input.GetKeyDown(GuitarFinisherKey))
        {
            PlayerStates(States.GuitarFinisher);

        }
    }
    void PlayerStates(States states)
    {
        switch(states)
        {
            case States.SonicRoar:
                Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                if (Physics.Raycast(ray, out RaycastHit hit, attackRange, enemyLayer))
                {
                    enemy.TakeDamage(10 , true);
                }
                break;
            case States.PowerRoar:
                Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
                    
                foreach (Collider enemys in enemies)
                {
                    Vector3 target = (enemys.transform.position - transform.position).normalized;
                    float angle = Vector3.Angle(transform.forward, target);

                    if(angle < 90f / 2)
                    {
                        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                        if (enemyHealth != null)
                        {
                            enemyHealth.TakeDamage(10, true);
                        }
                    }
                }
                powerTimer = 0;
                break;
            case States.BeatShot:
                this.GetComponent<RhythmAttackSystem>().enabled = true;
                break;
            case States.GuitarFinisher:
                Collider[] enemiess = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
                foreach (Collider enemy in enemiess)
                {
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(10, true);
                    }
                }
                break;
            case States.Default:
                this.GetComponent<RhythmAttackSystem>().enabled = false;
                break;
        }
    }

    public enum States
    {
        SonicRoar,      //기본 공격 -> 입에서 직전 음파 발사
        PowerRoar,      //차지 공격 (스킬) -> 길게 눌러 충전 후 넓은 부채꼴 범위 공격
        BeatShot,       //리듬 대응 공격 (스킬) -> BGM 타이밍에 맞춰 누르면 데미지 배율 증가
        GuitarFinisher,  //궁극기 (스킬) -> 전방 광역 공격 + 버프
        Default,
    }
}
