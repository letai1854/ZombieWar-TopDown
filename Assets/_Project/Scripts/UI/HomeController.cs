using UnityEngine;

public class HomeController : MonoBehaviour
{
    public void PlayGame()
    {
        if (SoundManager.HasInstance) SoundManager.Instance.PlayButtonClick();
        
        if (GameManager.HasInstance)
        {
            GameManager.Instance.StartGame();
        }
    }
}
