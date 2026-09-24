using UnityEngine;
using UnityEngine.UI;

public class WinPopup : BasePopup
{
    [Header("Buttons")]
    public GameObject nextLevelButton;
    public GameObject retryButton;

    private void OnEnable()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        if (currentScene == "02_Level1")
        {
            if (nextLevelButton != null) nextLevelButton.SetActive(true);
            if (retryButton != null) retryButton.SetActive(false);
        }
        else 
        {
            if (nextLevelButton != null) nextLevelButton.SetActive(false);
            if (retryButton != null) retryButton.SetActive(true);
        }
    }
    public void OnClick_Retry()
    {
        if (SoundManager.HasInstance) SoundManager.Instance.PlayButtonClick();
        if (GameManager.HasInstance)
        {
            GameManager.Instance.RetryGame();
        }
    }

    public void OnClick_NextLevel()
    {
        if (SoundManager.HasInstance) SoundManager.Instance.PlayButtonClick();
        if (GameManager.HasInstance)
        {
            GameManager.Instance.LoadNextLevel();
        }
    }

    public void OnClick_Home()
    {
        if (SoundManager.HasInstance) SoundManager.Instance.PlayButtonClick();
        if (GameManager.HasInstance)
        {
            GameManager.Instance.LoadHome();
        }
    }
}
