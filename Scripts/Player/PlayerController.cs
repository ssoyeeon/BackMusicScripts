using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerController : Actor
{
    [SerializeField] private float moveSpeed = 5f;      // 움직일 속도
    public float mouseSensitivity = 2f;                 // 마우스 감도
    public Rigidbody rb;                                // 플레이어 리지드바디
    public Transform cameraTransform;                   // 카메라
    public LayerMask groundMask;                        // 플레이어가 밟을 땅 
    private float verticalRotation = 0f;
    private Vector3 moveDirection;

    public CinemachineVirtualCamera virCamera;          // 시네머신카메라
    public int Php = 100;                               // 플레이어 HP
    public TMP_Text PhpText;
    public LayerMask NPCLayer;

    bool isGrounded;
    float jumpTime;
    public float jumpForce;

    bool isTexting = false;

    private float damageReduction = 1f;                 // 1 = 100% 데미지(기본), 0.5 = 50%만 받음
    private float damageReductionEndTime = 0f;          // 피해감소 종료시각 (중복 사용 대비)

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //virCamera.m_Lens.Dutch = 180;        // 카메라 반전
        if (GameManager.Instance.currentGameState == GameState.Loading)
        {
            GameManager.Instance.ChangeGameState(GameState.Playing);
        }
    }

    protected override void ActorUpdate()
    {
        if(Php <= 0)
        {
            PhpText.text = "플레이어 사망";
        }
        else PhpText.text = $"플레이어 체력 : {Php}";

        // 마우스 회전 처리
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 이동 입력 저장
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        moveDirection = transform.right * moveX + transform.forward * moveZ;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f); // 대각선 이동 속도 정규화

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true && jumpTime <= 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpTime = 1f;
            isGrounded = false;
            Debug.Log("Jump");
        }
        else if (isGrounded == false)
        {
            jumpTime -= Time.deltaTime;
            if (jumpTime < 0)
            {
                jumpTime = 0;
                isGrounded = true;
            }
        }
        else moveSpeed = 5;

        if (Input.GetKeyDown(KeyCode.F) && !isTexting)
        {
            Debug.Log("F키 눌림! Interact() 함수를 호출합니다!");
            StartCoroutine(Interaction());
        }
    }

    protected override void ActorFixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        // Y축 속도는 유지하면서 XZ평면의 이동만 조정
        Vector3 targetVelocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);

        // 현재 속도와 목표 속도를 부드럽게 보간
        Vector3 velocityChange = (targetVelocity - rb.velocity);
        velocityChange.y = 0; // Y축 변화는 무시

        // 속도 변경을 제한하여 적용
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    public void Damaged(int Damage)
    {
        int finalDamage = Mathf.RoundToInt(Damage * damageReduction);
        Php -= finalDamage;

        Debug.Log($"플레이어 피해: {finalDamage} (원래 {Damage}), 피해감소계수: {damageReduction}");

        if (Php <= 0)
        {
            Debug.Log("플레이어 사망");
            //Destroy(this);
        }
    }

    public void Heal(int heal)
    {
        Php += heal;
        if (Php >= 100)
        {
            Php = 100;
            Debug.Log("피 100임 힐 ㄴㄴ");
        }
    }

    public IEnumerator Interaction()
    {
        isTexting = true;
        float rayDistance = 20f;

        Vector3 rayOrigin = cameraTransform.position + new Vector3(0, 1, 0);

        Debug.DrawRay(rayOrigin, cameraTransform.forward * rayDistance, Color.yellow, 2f);

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, cameraTransform.forward, out hit, rayDistance, NPCLayer))
        {
            DialogueTrigger trigger = hit.collider.GetComponent<DialogueTrigger>();
            if (trigger != null)
            {
                DialogueManager.instance.SetDialogue(trigger.dialogue);
            }
        }

        yield return new WaitForSeconds(4f);
        isTexting = false;
        DialogueManager.instance.EndDialogue();

    }

    public IEnumerator ReduceDamageForSeconds(float multiplier, float duration)
    {
        damageReduction = multiplier;
        damageReductionEndTime = Time.time + duration;
        Debug.Log($"[피해감소 시작] {duration}초 동안 {multiplier * 100f}% 피해만 적용");

        yield return new WaitForSeconds(duration);

        if (Time.time >= damageReductionEndTime)
        {
            damageReduction = 1f;
        }
    }
}
