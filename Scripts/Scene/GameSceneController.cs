using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;

public class GameSceneController : MonoBehaviour
{
    public GameObject moveCanvas;
    public GameObject Player;
    public GameObject PopUpUI;
    public GameObject NPCUI;

    public LayerMask enemyLayer;
    public PlayerController playerController;
    public Vector3 originalPosition;


    private void Awake()
    {
        originalPosition = moveCanvas.transform.position;
    }

    void Start()
    {
        if (GameManager.Instance.currentGameState != GameState.Playing)
        {
            GameManager.Instance.ChangeGameState(GameState.Playing);
        }
        playerController = FindAnyObjectByType<PlayerController>();

    }

    private void Update()
    {
        if (Player.transform.position.z > -33)
        {
        //    //PopUpUI.gameObject.transform.DOShakePosition(PopUpUI.gameObject.transform.position.z);
        //    //PopUpUI.gameObject.SetActive(false);
        //    Door_Left.transform.DOMoveX(15, 3f);
        //    Door_Right.transform.DOMoveX(-15, 3f);
            moveCanvas.transform.DOMoveX(originalPosition.x, 0.7f);
        }

        //if (Player.transform.position.z > 30f && FindObjectsOfType<EnemyHealth>().Length == 0)
        //{
        //    SceneManager.Instance.LoadScene("GameeScene");
        //}
    }


    private void OnTriggerEnter(Collider other)
    {

    }
}
