using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : Actor
{
    public Transform target;        //플레이어

    NavMeshAgent nav;               //내비게이션

    public PlayerController playerController;

    Vector3 dis;
    public float targetDistance;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        target = playerController.transform;
    }

    protected override void ActorUpdate()
    {
        Move();
    }

    void Move()
    { // nav 컴포넌트가 활성화 상태이고, 유효한 경로를 가질 때만 목적지 설정
        if (nav.enabled)
        {
            dis = transform.position - target.position;

            if (dis.magnitude < targetDistance)
            {
                nav.SetDestination(target.position);
            }
        }
    }

    public void StartWakeup()
    {
        StartCoroutine(EnemyWakeup());
    }

    IEnumerator EnemyWakeup()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<NavMeshAgent>().isStopped = false;
    }

}
