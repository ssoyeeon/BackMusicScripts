using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameeSceneController : MonoBehaviour
{
    [Tooltip("말풍선 띄울 UI 캔버스 필요")]
    public GameObject PopupCanvas;
    bool isPigBoss;
    bool isActive;
    private void Start()
    {
        if(PopupCanvas != null)
        {
            PopupCanvas.SetActive(false);
        }
        isPigBoss = false;  
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(PopupCanvas != null)
            {
                StartCoroutine(PopupUI());
            }
            SceneManager.Instance.LoadPigBossScene();
        }
    }
    
    public IEnumerator PopupUI()
    {
        PopupCanvas.SetActive(true);
        isActive = true;
        yield return new WaitForSeconds(1f);        //만약 진짜 갈거라면 키보드 2를 눌러줘! 느낌으로 ㄱㄱ 
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2) && isActive)
        {
            isPigBoss = true;
        }
    }
}
