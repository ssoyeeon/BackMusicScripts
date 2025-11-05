using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : SingletonMonoBehaviour<EventSystem>
{
    public bool isEventPlaying = false;

    public void StartEvent()
    {
        if (isEventPlaying) return;

        isEventPlaying = true;
        GameManager.Instance?.PausedGame();

        Debug.Log("이벤트 시작 - 게임 일시정지");
    }    

    public void EndEvent()
    {
        if(!isEventPlaying) return;

        isEventPlaying = false;
        GameManager.Instance.ResumeGame();

        Debug.Log("이벤트 종료 - 게임 재개");

    }

    public void PlayCutscene(float duration)
    {
        StartCoroutine(CutsceneCoroutine(duration));
    }

    private IEnumerator CutsceneCoroutine(float duration)
    {
        StartEvent();

        Debug.Log($"{duration}초 컷신 재생");
        yield return new WaitForSeconds(duration);

        EndEvent();
    }
}
