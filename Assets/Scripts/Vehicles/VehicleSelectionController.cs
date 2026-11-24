using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BusSimulator
{
	public class VehicleSelectionController : MonoBehaviour
	{
		public GameObject vehiclelock;

		public GameObject lockedDialoge;

		public GameObject vehicle;

		public Text cashtext;

		public Text vehicleprice;

		public GameObject Buybtn;

		private int vehicleCount;

		public int[] vehicleprices;

		public SoundManager sound;

		public string UnlockAllBuses_Product_id;

		public GameObject cashWaringDialoge;

		public GameObject ConratulationDialoge;

		public Text ConratulationDialoge_text;

		public Image SpeedBar;

		public Image engineBar;

		public Image SteerBar;

		public Image Powerbar;

		public float[] SpeedBarValue;

		public float[] EngineBarValue;

		public float[] SteerBarValue;

		public float[] PowerBarValue;

		public GameObject UnLockAllBtn;

		public GameObject[] rewardedVideoBtn;
 
	 

		private void OnEnable()
		{
	 
		}

		private void OnDisable()
		{
		 
		}

		private void Start()
		{
			PlayerPrefs.SetInt("vehicle", 0);
			PlayerPrefs.SetInt("vehicle0lock", 1);
			SetCash();
			SetLock();
			SetProperties(0);
			if (!PlayerPrefs.HasKey("UnLockAll"))
			{
				PlayerPrefs.SetInt("UnLockAll", 0);
			}
			if (PlayerPrefs.GetInt("UnLockAll") == 1)
			{
				if (UnLockAllBtn != null) UnLockAllBtn.SetActive(false);
				CheckIAP();
			}
			if (Advertisements.Instance.IsRewardVideoAvailable())
			{
				for (int i = 0; i < rewardedVideoBtn.Length; i++)
				{
					rewardedVideoBtn[i].SetActive(true);
				}
			}
		
		 
			Advertisements.Instance.ShowInterstitial();
		}

		private void SetCash()
		{
			cashtext.text = PlayerPrefs.GetInt("cash", 0).ToString();
		}

		private void CheckIAP()
		{
			for (int i = 0; i < vehicle.transform.childCount; i++)
			{
				PlayerPrefs.SetInt("vehicle" + i + "lock", 1);
			}
			Buybtn.SetActive(false);
			vehiclelock.SetActive(false);
		}

		private void UnLockAllBuses(string name, string id)
		{
			if (id == UnlockAllBuses_Product_id)
			{
				for (int i = 0; i < vehicle.transform.childCount; i++)
				{
					PlayerPrefs.SetInt("vehicle" + i + "lock", 1);
				}
				Buybtn.SetActive(false);
				vehiclelock.SetActive(false);
				cashWaringDialoge.SetActive(false);
				if (UnLockAllBtn != null) UnLockAllBtn.SetActive(false);
				ConratulationDialoge.SetActive(true);
				PlayerPrefs.SetInt("UnLockAll", 1);
				ConratulationDialoge_text.text = "Congratulation! you unlocked all buses succesfully.";
			}
		}

		private void SetLock()
		{
			if (PlayerPrefs.GetInt("vehicle" + vehicleCount + "lock") == 1)
			{
				Buybtn.SetActive(false);
				vehiclelock.SetActive(false);
			}
			else
			{
				Buybtn.SetActive(true);
				vehiclelock.SetActive(true);
				vehicleprice.text = vehicleprices[vehicleCount].ToString();
			}
		}

		public void Locked()
		{
			lockedDialoge.SetActive(true);
		}

		public void LockedOkButton(GameObject obj)
		{
			sound.ButtonClickSound();
			lockedDialoge.SetActive(false);
		}

		public void BuyBtn(GameObject obj)
		{
			sound.ButtonClickSound();
			if (PlayerPrefs.GetInt("cash") >= vehicleprices[vehicleCount])
			{
				PlayerPrefs.SetInt("cash", PlayerPrefs.GetInt("cash") - vehicleprices[vehicleCount]);
				PlayerPrefs.SetInt("vehicle" + vehicleCount + "lock", 1);
				SetCash();
				SetLock();
				ConratulationDialoge.SetActive(true);
				ConratulationDialoge_text.text = "Congratulation! you unlocked bus succesfully.";
			}
			else
			{
				cashWaringDialoge.SetActive(true);
			}
		}

		public void ButtonReleased(GameObject btn)
		{
			btn.transform.localScale = new Vector3(1f, 1f, 1f);
		}

		private IEnumerator SpeedBarfill(int vehicleno)
		{
			while (SpeedBar.fillAmount <= SpeedBarValue[vehicleno])
			{
				yield return new WaitForSeconds(0f);
				SpeedBar.fillAmount += 0.02f;
			}
		}

		private IEnumerator EngineBarFill(int vehicleno)
		{
			while (engineBar.fillAmount <= EngineBarValue[vehicleno])
			{
				yield return new WaitForSeconds(0f);
				engineBar.fillAmount += 0.02f;
			}
		}

		private IEnumerator SteerBarFill(int vehicleno)
		{
			while (SteerBar.fillAmount <= SteerBarValue[vehicleno])
			{
				yield return new WaitForSeconds(0f);
				SteerBar.fillAmount += 0.02f;
			}
		}

		private IEnumerator PowerBarFill(int vehicleno)
		{
			while (Powerbar.fillAmount <= PowerBarValue[vehicleno])
			{
				yield return new WaitForSeconds(0f);
				Powerbar.fillAmount += 0.02f;
			}
		}

		private void ResetBars()
		{
			SpeedBar.fillAmount = 0f;
			engineBar.fillAmount = 0f;
			SteerBar.fillAmount = 0f;
			Powerbar.fillAmount = 0f;
		}

		private void SetProperties(int vehicleno)
		{
			StopAllCoroutines();
			ResetBars();
			StartCoroutine(SpeedBarfill(vehicleno));
			StartCoroutine(EngineBarFill(vehicleno));
			StartCoroutine(SteerBarFill(vehicleno));
			StartCoroutine(PowerBarFill(vehicleno));
		}

		public void NextVehicle(GameObject obj)
		{
			sound.ButtonClickSound();
			obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			vehicle.transform.GetChild(vehicleCount).gameObject.SetActive(false);
			if (vehicleCount + 1 <= vehicle.transform.childCount - 1)
			{
				vehicleCount++;
			}
			else
			{
				vehicleCount = 0;
			}
			vehicle.transform.GetChild(vehicleCount).gameObject.SetActive(true);
			PlayerPrefs.SetInt("vehicle", vehicleCount);
			SetLock();
			SetProperties(vehicleCount);
		}

		public void PrevVehicle(GameObject obj)
		{
			sound.ButtonClickSound();
			obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			vehicle.transform.GetChild(vehicleCount).gameObject.SetActive(false);
			if (vehicleCount - 1 >= 0)
			{
				vehicleCount--;
			}
			else
			{
				vehicleCount = vehicle.transform.childCount - 1;
			}
			vehicle.transform.GetChild(vehicleCount).gameObject.SetActive(true);
			PlayerPrefs.SetInt("vehicle", vehicleCount);
			SetLock();
			SetProperties(vehicleCount);
		}
		[System.Obsolete]
		public void Play(GameObject obj)
		{
			sound.ButtonClickSound();
			obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			Debug.Log("vehicle no : " + PlayerPrefs.GetInt("vehicle"));
			if (!vehiclelock.activeSelf)
			{
				Application.LoadLevel(Application.loadedLevel + 1);
			}
			else
			{
				Locked();
			}
		}
		[System.Obsolete]
		public void Back(GameObject obj)
		{
			sound.ButtonClickSound();
			obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			Application.LoadLevel(Application.loadedLevel - 1);
		}

		public void WatchVideo()
		{
			if (Advertisements.Instance.IsRewardVideoAvailable())
			{
				AdManager.Instance.ShowRewardedVideo();
				PlayerPrefs.SetInt("cash", PlayerPrefs.GetInt("cash") + 500);
				SetCash();
			}
		}

		private void ResetVariables()
		{
		 
		}

		public void CashWarningBack()
		{
			sound.ButtonClickSound();
			cashWaringDialoge.SetActive(false);
		}

		public void Congrats_Ok()
		{
			sound.ButtonClickSound();
			ConratulationDialoge.SetActive(false);
		}
	}
}
