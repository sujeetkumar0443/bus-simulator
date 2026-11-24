using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
    }

    private void Start()
    {
        Advertisements.Instance.Initialize();
        // ShowBanner();
    }

    public void ShowBanner()
    {
        if (Advertisements.Instance.IsBannerOnScreen())
        {
            Advertisements.Instance.ShowBanner(BannerPosition.TOP, BannerType.Banner);
        }
    }

    public void HideBanner()
    {
        Advertisements.Instance.HideBanner();
    }

    public void ShowInterstitial()
    {
        if (Advertisements.Instance.IsInterstitialAvailable())
        {
            Advertisements.Instance.ShowInterstitial();
        }
    }

    public void ShowRewardedVideo()
    {
        if (Advertisements.Instance.IsRewardVideoAvailable())
        {
            Advertisements.Instance.ShowRewardedVideo(VideoComplete);
        }
    }

    private void VideoComplete(bool completed)
    {
        if (completed)
        {
            // Handle the reward here, e.g., give the player some in-game currency.
        }
    }
}
