using UnityEngine;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timeText;

    [Header("Bomb UI References")]
    public UnityEngine.UI.Button bombButton;
    public UnityEngine.UI.Image bombCooldownImage;
    public TextMeshProUGUI bombCooldownText;
    
    private Soldier playerSoldier;

    private void Start()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.OnTimeUpdated += HandleTimeUpdated;
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            
            // Đồng bộ trạng thái UI ngay khi vừa load scene (tránh việc bật UI sớm khi chưa Play)
            HandleGameStateChanged(GameManager.Instance.CurrentState);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.OnTimeUpdated -= HandleTimeUpdated;
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    private void HandleTimeUpdated(float timeRemaining)
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void HandleGameStateChanged(GameState state)
    {
        bool isPlaying = (state == GameState.Playing);
        if (state == GameState.Win || state == GameState.Lose || isPlaying)
        {
            // Tìm tất cả các Canvas trong Game
            Canvas[] canvases = FindObjectsOfType<Canvas>(true);
            foreach (var canvas in canvases)
            {
                // Chỉ ẩn/hiện các Canvas nằm trong Scene hiện tại (02_Gameplay)
                // KHÔNG đụng tới Canvas chứa Win/Lose Popup (nằm trong DontDestroyOnLoad)
                if (canvas.gameObject.scene.name == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
                {
                    canvas.gameObject.SetActive(isPlaying);
                }
            }
        }
    }

    private void Update()
    {
        if (playerSoldier == null)
        {
            playerSoldier = FindObjectOfType<Soldier>();
        }

        if (playerSoldier != null)
        {
            float remaining = playerSoldier.BombCooldownRemaining;
            float total = playerSoldier.BombCooldownTotal;

            if (remaining > 0)
            {
                // Đang trong thời gian hồi chiêu
                if (bombButton != null) bombButton.interactable = false;
                if (bombCooldownImage != null)
                {
                    bombCooldownImage.gameObject.SetActive(true);
                    bombCooldownImage.fillAmount = remaining / total;
                }
                if (bombCooldownText != null)
                {
                    bombCooldownText.gameObject.SetActive(true);
                    bombCooldownText.text = Mathf.CeilToInt(remaining).ToString();
                }
            }
            else
            {
                // Sẵn sàng sử dụng
                if (bombButton != null) bombButton.interactable = true;
                if (bombCooldownImage != null) bombCooldownImage.gameObject.SetActive(false);
                if (bombCooldownText != null) bombCooldownText.gameObject.SetActive(false);
            }
        }
    }
}
