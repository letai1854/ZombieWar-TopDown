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
            Canvas[] canvases = FindObjectsOfType<Canvas>(true);
            foreach (var canvas in canvases)
            {
              
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
                if (bombButton != null) bombButton.interactable = true;
                if (bombCooldownImage != null) bombCooldownImage.gameObject.SetActive(false);
                if (bombCooldownText != null) bombCooldownText.gameObject.SetActive(false);
            }
        }
    }
}
