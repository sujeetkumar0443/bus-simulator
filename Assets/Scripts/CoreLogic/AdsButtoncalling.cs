using UnityEngine;

public class AdsButtoncalling : MonoBehaviour
{
	private void Start()
	{
	}

	public void HideBanner()
	{
		Advertisements.Instance.HideBanner();
	}

	public void ShowInterstitial()
	{
		Advertisements.Instance.ShowInterstitial();
	}

	public void ShowApplovin()
	{
		Advertisements.Instance.ShowInterstitial();
	}

	public void ShowApplovinRewarded()
	{
		AdManager.Instance.ShowRewardedVideo();
	}

	public void ShowChartBoost()
	{
		Advertisements.Instance.ShowInterstitial();
	}

	public void ShowUnityAd()
	{
		Advertisements.Instance.ShowInterstitial();
	}

	public void ShowAdcoloyInterstitial()
	{
	}

	public void ShowAdcoloyRewardedVideo()
	{
	}
}
