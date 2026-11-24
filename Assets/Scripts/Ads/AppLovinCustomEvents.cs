using UnityEngine;

public class AppLovinCustomEvents : MonoBehaviour
{
	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void onAppLovinEventReceived(string ev)
	{
		if (!ev.Contains("DISPLAYEDINTER"))
		{
			if (ev.Contains("HIDDENINTER"))
			{
				AppLovin.PreloadInterstitial();
			}
			else if (!ev.Contains("LOADEDINTER") && !string.Equals(ev, "LOADINTERFAILED"))
			{
			}
		}
		if (!ev.Contains("REWARDAPPROVEDINFO") && !ev.Contains("LOADEDREWARDED") && !ev.Contains("LOADREWARDEDFAILED") && ev.Contains("HIDDENREWARDED"))
		{
			AppLovin.LoadRewardedInterstitial();
		}
	}
}
