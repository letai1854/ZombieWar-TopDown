using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public enum GameState
{
    Init,
    Home,
    Playing,
    Win,
    Lose
}

public class GameManager : BaseManager<GameManager>
{
    public GameState CurrentState { get; private set; }

    [Header("Game Settings")]
    public string startingLevel = "02_Level1"; 
    public float gameDuration = 180f; 
    public float CurrentTime { get; private set; }
    

    public event Action<GameState> OnGameStateChanged;
    public event Action<float> OnTimeUpdated;

    protected override void Awake()
    {
        base.Awake();
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.ToLower().Contains("init")) CurrentState = GameState.Init;
        else if (sceneName.ToLower().Contains("home")) CurrentState = GameState.Home;
        else if (sceneName.ToLower().Contains("game")) CurrentState = GameState.Playing;
        else CurrentState = GameState.Init;
    }

    private void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentTime -= Time.deltaTime;
            OnTimeUpdated?.Invoke(CurrentTime);

            if (CurrentTime <= 0)
            {
                CurrentTime = 0;
                GameWin();
            }
        }
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
        Debug.Log("[GameManager] Đổi trạng thái sang: " + newState);
    }

    public void LoadHome()
    {
        StartCoroutine(LoadSceneRoutine("01_Home", GameState.Home));
    }

    public void LoadNextLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "02_Level1")
        {
            StartCoroutine(LoadSceneRoutine("03_Level2", GameState.Playing));
        }
        else
        {
            StartCoroutine(LoadSceneRoutine("01_Home", GameState.Home));
        }
    }

    public void StartGame()
    {
        if (SoundManager.HasInstance) SoundManager.Instance.PlayBGM(SoundManager.Instance.gameplayBGM);
        Time.timeScale = 1f;
        CurrentTime = gameDuration;
        ChangeState(GameState.Playing);
        SceneManager.LoadScene(startingLevel); 
    }

    public void GameLose()
    {
        if (CurrentState == GameState.Playing)
        {
            Time.timeScale = 0f;
            ChangeState(GameState.Lose);
            if (UIManager.HasInstance) UIManager.Instance.ShowPopup<LosePopup>();
        }
    }

    public void GameWin()
    {
        if (CurrentState == GameState.Playing)
        {
            Time.timeScale = 0f;
            ChangeState(GameState.Win);
            if (UIManager.HasInstance) UIManager.Instance.ShowPopup<WinPopup>();
        }
    }

    public void RetryGame()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadSceneRoutine(currentScene, GameState.Playing));
    }

    private System.Collections.IEnumerator LoadSceneRoutine(string sceneName, GameState newState)
    {
        if (SoundManager.HasInstance) 
        {
            SoundManager.Instance.PlayBGM(newState == GameState.Home ? SoundManager.Instance.homeBGM : SoundManager.Instance.gameplayBGM);
        }

        if (newState == GameState.Playing)
        {
            CurrentTime = gameDuration;
        }
        ResetZombies();

        SceneManager.LoadScene(sceneName);

        yield return null;

        if (UIManager.HasInstance)
        {
            UIManager.Instance.HideAllPopups();
        }

        ChangeState(newState);

        Time.timeScale = 1f;
    }

    private void ResetZombies()
    {
   
        if (ObjectPooler.HasInstance)
        {
            foreach (Transform child in ObjectPooler.Instance.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
}
