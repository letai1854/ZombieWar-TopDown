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

    public float gameDuration = 180f; // 3 phút
    public float CurrentTime { get; private set; }
    
    // Các event để UI dễ dàng kết nối
    public event Action<GameState> OnGameStateChanged;
    public event Action<float> OnTimeUpdated;

    protected override void Awake()
    {
        base.Awake();
        // Kiểm tra xem chúng ta đang bắt đầu ở Scene nào để set State tương ứng
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

    public void StartGame()
    {
        if (SoundManager.HasInstance) SoundManager.Instance.PlayBGM(SoundManager.Instance.gameplayBGM);
        Time.timeScale = 1f;
        CurrentTime = gameDuration;
        ChangeState(GameState.Playing);
        SceneManager.LoadScene("02_Gameplay"); 
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
        StartCoroutine(LoadSceneRoutine("02_Gameplay", GameState.Playing));
    }

    private System.Collections.IEnumerator LoadSceneRoutine(string sceneName, GameState newState)
    {
        // 1. Chuyển nhạc nền ngay lập tức
        if (SoundManager.HasInstance) 
        {
            SoundManager.Instance.PlayBGM(newState == GameState.Home ? SoundManager.Instance.homeBGM : SoundManager.Instance.gameplayBGM);
        }

        // 2. Reset dữ liệu trước
        if (newState == GameState.Playing)
        {
            CurrentTime = gameDuration;
        }
        ResetZombies();

        // 3. Load Scene (quá trình này mất 1 frame)
        SceneManager.LoadScene(sceneName);

        // 4. ĐỢI ĐÚNG 1 FRAME ĐỂ SCENE MỚI RENDER XONG
        yield return null;

        // 5. Lúc này scene mới đã che kín màn hình, ta tắt Popup trước
        if (UIManager.HasInstance)
        {
            UIManager.Instance.HideAllPopups();
        }

        // 6. Sau khi tắt Popup xong, mới đổi State (lúc này UI Joystick mới được phép hiện lên)
        ChangeState(newState);

        Time.timeScale = 1f;
    }

    private void ResetZombies()
    {
        // Vì ObjectPooler là Singleton (DontDestroyOnLoad), các Zombie đang sống sẽ ko bị huỷ khi chuyển Scene
        // Nên ta phải tự tắt chúng đi (trả về pool)
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
