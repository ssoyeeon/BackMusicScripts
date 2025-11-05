using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScale : MonoBehaviour
{
    public float maxScale;
    public float minScaleTime;
    public float maxScaleTime;      

    public int movePosition;    //포지션 잡을 때

    public int VFXint;      //스위치 
    public float moveTimer; //타이머

    private void Start()
    {
        ObjectVFX();
    }
    void Update()
    {

    }

    public void ObjectVFX()
    {
        switch(VFXint)
        {
            case 0: //전체 스케일
                Vector3 originalScale = transform.localScale; // 시작 크기 저장

                Sequence seq = DOTween.Sequence();

                seq.Append(transform.DOScale(originalScale * maxScale, maxScaleTime))   // 3초 동안 3배로 확대
                   .Append(transform.DOScale(originalScale, minScaleTime))        // 1초 동안 원래 크기로 축소
                   .SetLoops(-1, LoopType.Restart);
                break;

            case 1: //Y축 스케일
                float originalScaleY = transform.localScale.y;

                Sequence seqY = DOTween.Sequence();
                seqY.Append(transform.DOScaleY(originalScaleY * maxScale, maxScaleTime))
                    .Append(transform.DOScaleY(originalScaleY, minScaleTime))        // 1초 동안 원래 크기로 축소
                   .SetLoops(-1, LoopType.Restart);
                break;

            case 2: //회전
                transform.DORotate(new Vector3(0, 360, 0), maxScaleTime, RotateMode.FastBeyond360)
                         .SetEase(Ease.Linear) // 일정한 속도로 회전하도록 설정
                         .SetLoops(-1, LoopType.Incremental); // 무한 반복 및 누적 회전 설정
                break;
            case 3:
                // 3초 지연 시간을 추가할 시퀀스 생성
                Sequence seqX = DOTween.Sequence();

                // 3초 대기 (Wait) 시간을 추가합니다.
                seqX.AppendInterval(3f);

                // 대기가 끝난 후 DOMoveX를 실행합니다.
                seqX.Append(transform.DOMoveX(movePosition, maxScaleTime));
                break;                
        }
    }
}
