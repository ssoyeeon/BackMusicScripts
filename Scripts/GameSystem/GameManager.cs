using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver,
    Loading
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [Header("Game Settings")]
    public GameState currentGameState = GameState.Menu;
    public bool isGamePaused = false;

    [Header("Game Stats")]
    public int currentScore = 0;
    public float gameTime = 0f;

    private GameState preciousGameState;

    public SettingsUI settingUI;

    protected override void Awake()
    {
        base.Awake();
        InitializeGame();
    }

    private void Start()
    {
        StartCoroutine(InitializeManagers());
    }

    private void Update()
    {
        if(currentGameState == GameState.Playing && !isGamePaused)
        {
            gameTime += Time.deltaTime;
        }

        HandleInput();
    }

    private void InitializeGame()
    {
        //게임 시작 시 기본 설정
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Debug.Log("GameManager 초기화 완료");

    }

    private IEnumerator InitializeManagers()
    {
        yield return new WaitForEndOfFrame();

        //여기에 매니저들 초기화. 순서 중요함!
        //if (SceneManager.Instance != null)
        //    Debug.Log("SceneManager 연결 확인");
        //if (SoundManager.Instance != null)
        //    Debug.Log("SoundManager 연결 확인");
        //if (UIManager.Instance != null)
        //    Debug.Log("UIManager 연결 확인");
        //if (SaveManager.Instance != null)
        //    Debug.Log("SaveManager 연결 확인");
    }

    private void HandleInput()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentGameState == GameState.Playing)
            {
                PausedGame();
            }
            else if(currentGameState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }

    public void ChangeGameState(GameState newState)
    {
        if (currentGameState == newState) return;

        preciousGameState = currentGameState;
        currentGameState = newState;

        OnGameStateChange(newState);
    }

    private void OnGameStateChange(GameState newState)
    {
        switch(newState)
        {
            case GameState.Menu:
                Cursor.lockState = CursorLockMode.None;
                break;
            case GameState.Playing:
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case GameState.Paused:
                Cursor.lockState = CursorLockMode.None;
                break;
            case GameState.GameOver:
                Cursor.lockState = CursorLockMode.None;
                break;
            case GameState.Loading:
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }

    public void StateGame()
    {
        ResetGameStats();
        ChangeGameState(GameState.Playing);
        GameEvents.GameResumed();
    }

    public void PausedGame()
    {
        if (currentGameState != GameState.Playing) return;

        isGamePaused = true;
        ChangeGameState(GameState.Paused);
        GameEvents.GamePaused();
        settingUI.ShowSettings();

    }

    public void ResumeGame()
    {
        if(currentGameState != GameState.Paused) return;

        isGamePaused = false;
        ChangeGameState(GameState.Playing);
        GameEvents.GameResumed();
        settingUI.OnCloseClicked();
    }

    public void GameOver()
    {
        ChangeGameState(GameState.GameOver);

        //게임 오버 처리
        //세이브 매니저랑 UI매니저 해서! 
    }    
    
    public void RestartGame()
    {
        ResetGameStats();
        ChangeGameState(GameState.Playing);
    }

    public void GoToMainMenu()
    {
        ChangeGameState(GameState.Menu);
        //SceneManager.Instance?.LoadScene("MainMenu");
    }

    private void ResetGameStats()
    {
        //얘네는 세이브 데이터로 바꿔주면 될 거 ㅅ강ㅌ습니다~~
        currentScore = 0;
        gameTime = 0f;
        GameEvents.ScoreChanged(currentScore);
    }

    public bool IsGamePlaying()
    {
        return currentGameState == GameState.Playing && !isGamePaused;

        //플레잉일 때 UI 띄우기 인거자나?
    }

    public void QuitGmae()
    {
        Debug.Log("게임 종료");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if(pauseStatus && currentGameState == GameState.Playing)
        {
            PausedGame();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if(!hasFocus && currentGameState == GameState.Playing)
        {
            PausedGame();
        }
    }
}