using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    // 이 충격파가 몇 초 뒤에 사라질지 정합니다.
    public float lifetime = 2.0f;

    void Start()
    {
        // 만들어지자마자 lifetime(2초) 뒤에 스스로를 파괴하도록 예약합니다.
        Destroy(gameObject, lifetime);
    }
}
