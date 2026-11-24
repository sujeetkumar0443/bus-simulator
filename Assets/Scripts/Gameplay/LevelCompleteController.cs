using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteController : MonoBehaviour
{
	public GameObject LevelCompDialog;

	public GameObject LevelFailDialog;

	public GameObject TimeUpDialog;

	public GameObject RateUsDialoge;

	private bool done;

	public SoundManager sound;

	 

	public Text TotalCash;

	private void Start()
	{
		done = false;
		if (!PlayerPrefs.HasKey("rateuscounter"))
		{
			PlayerPrefs.SetInt("rateuscounter", 0);
		}
	}

	public IEnumerator gameFail()
	{
		yield return new WaitForSeconds(1.2f);
		LevelFail();
	}

	public void LevelComplete()
	{
		if (done)
		{
			return;
		}
		done = true;
		GameState.LevelOver = true;
		PlayerPrefs.SetInt("cash", PlayerPrefs.GetInt("cash") + 11000);
		TotalCash.text = PlayerPrefs.GetInt("cash").ToString();
		LevelCompDialog.SetActive(true);
		if (PlayerPrefs.GetInt("levelno") != 10)
		{
			PlayerPrefs.SetInt("levelno", PlayerPrefs.GetInt("levelno") + 1);
			PlayerPrefs.SetInt("unlocklevelno" + PlayerPrefs.GetInt("levelno"), 1);
		}
		else
		{
			PlayerPrefs.SetInt("levelno", 1);
		}
		sound.pauseSounds(false);
		VehicleManager.vehicleManager.PauseVehicleSounds(false);
		Time.timeScale = 0f;
		if (PlayerPrefs.GetInt("rateuscounter") != -1)
		{
			PlayerPrefs.SetInt("rateuscounter", PlayerPrefs.GetInt("rateuscounter") + 1);
			if (PlayerPrefs.GetInt("rateuscounter") == 2)
			{
				PlayerPrefs.SetInt("rateuscounter", 0);
				RateUsDialoge.SetActive(true);
			}
		}
		Advertisements.Instance.ShowInterstitial();
	}

	public void LevelFail()
	{
		if (!done)
		{
			Debug.Log("level Fail");
			LevelFailDialog.SetActive(true);
			Time.timeScale = 0f;
			done = true;
			sound.pauseSounds(false);
			VehicleManager.vehicleManager.PauseVehicleSounds(false);
			Advertisements.Instance.ShowInterstitial();
		}
	}

	public void TimeUp()
	{
		if (!done)
		{
			Debug.Log("Time up");
			LevelFailDialog.SetActive(true);
			Time.timeScale = 0f;
			done = true;
			sound.pauseSounds(false);
			VehicleManager.vehicleManager.PauseVehicleSounds(false);
			Advertisements.Instance.ShowInterstitial();
		}
	}
}
