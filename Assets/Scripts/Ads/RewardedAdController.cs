using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
 

public class RewardedAdController : MonoBehaviour
{
    private bool isLoaded;

    public Text statusLabel;

    public void LoadRewardedVideo()
    {
        statusLabel.text = "Loading rewardedVideo ad...";
        // Check if rewarded video ad is available
        if (Advertisements.Instance.IsRewardVideoAvailable())
        {
            isLoaded = true;
            statusLabel.text = "Ad loaded. Click 'Show' to watch.";
        }
        else
        {
            isLoaded = false;
            statusLabel.text = "Ad not loaded. Click 'Load' to request an ad.";
        }
    }

    public void ShowRewardedVideo()
    {
        if (isLoaded)
        {
            // Show rewarded video ad
            AdManager.Instance.ShowRewardedVideo();
            isLoaded = false;
            statusLabel.text = string.Empty;
        }
        else
        {
            statusLabel.text = "Ad not loaded. Click 'Load' to request an ad.";
        }
    }

    private void OnDestroy()
    {
        Debug.Log("RewardedVideoAdScene was destroyed!");
    }

    public void NextScene()
    {
        SceneManager.LoadScene("NativeAdScene");
    }
}
