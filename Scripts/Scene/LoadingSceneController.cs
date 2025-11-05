using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoadingSceneController : MonoBehaviour
{
    public GameObject mainCamera;
    public int currentSceneEffect;
    public Transform npcTransform;
    public GameObject cameraMovePosition;

    public void Update()
    {
        switch (currentSceneEffect)
        {
            case 0:
                StartCoroutine(FirstLoadingScene());
                break;
            case 1:
                break;
        }
    }

    IEnumerator FirstLoadingScene()
    {
        yield return new WaitForSeconds(1f);
        npcTransform.DOLookAt(mainCamera.transform.position, 0.5f);
        mainCamera.transform.DOMove(cameraMovePosition.transform.position, 3);
        //mainCamera.transform.DOShakePosition(0.5f);
        currentSceneEffect = 1;
    }
}
