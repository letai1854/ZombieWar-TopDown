using UnityEngine;
using UnityEngine.UI;

public class WinPopup : BasePopup
{
    public void OnClick_Retry()
    {
        if (SoundManager.HasInstance) SoundManager.Instance.PlayButtonClick();
        if (GameManager.HasInstance)
        {
            GameManager.Instance.RetryGame();
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
