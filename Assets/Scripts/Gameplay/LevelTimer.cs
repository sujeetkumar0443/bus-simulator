using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour
{
	public float min;

	public float sec;

	public GameObject time;

	public LevelCompleteController gamecond;

	public GameObject controls;

	public float[] levelsMin;

	public Slider timebar;

	public Text percentgText;

	private int fuelPercentage;

	private bool fuelOnceDone;

	public GameObject FuelShortDialoge;

	public Text seconds;

	public Text levelFailReasonText;

	private void Start()
	{
	}

	public void settime()
	{
		if (!ModeSelectionController.FreeMoode)
		{
			min = levelsMin[LevelManager.LevelNumber - 1];
		}
		else
		{
			min = 29f;
		}
		sec = min * 60f;
		timebar.maxValue = sec;
		timebar.value = sec;
		StartCoroutine(TimeRun());
	}

	public void StartTimeAgain()
	{
		StartCoroutine(TimeRun());
	}

	public void ResetTime()
	{
		StopAllCoroutines();
		min = levelsMin[LevelManager.LevelNumber - 1];
		sec = min * 60f;
		percentgText.color = Color.green;
		StartCoroutine(FuelBarFill());
	}

	private IEnumerator FuelBarFill()
	{
		yield return new WaitForSeconds(0f);
		if (timebar.value != timebar.maxValue)
		{
			timebar.value += 1f;
			fuelPercentage = (int)(timebar.value / (min * 60f) * 100f);
			percentgText.text = fuelPercentage + " %";
			StartCoroutine(FuelBarFill());
		}
		else
		{
			StopAllCoroutines();
			timebar.transform.parent.GetComponent<Animator>().SetBool("open", false);
			timebar.transform.parent.GetComponent<Animator>().SetBool("close", true);
			Invoke("StartFuelAgain", timebar.transform.parent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
		}
	}

	private void StartFuelAgain()
	{
		VehicleManager.vehicleManager.StopVehicle(false);
		StartCoroutine(TimeRun());
	}

	private IEnumerator TimeRun()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			if (sec > 0f)
			{
				if (GameState.starttime && !GameState.LevelOver)
				{
					sec -= 1f;
					seconds.text = sec.ToString();
					timebar.value -= 1f;
					fuelPercentage = (int)(timebar.value / (min * 60f) * 100f);
					percentgText.text = fuelPercentage + "%";
					if (fuelPercentage == 15 && !fuelOnceDone &&Advertisements.Instance.IsRewardVideoAvailable())
					{
						fuelOnceDone = true;
						percentgText.color = Color.red;
						FuelShortDialoge.SetActive(true);
						Time.timeScale = 0f;
						StopAllCoroutines();
					}
				}
			}
			else
			{
				levelFailReasonText.text = "You are out of fuel!";
				gamecond.LevelFail();
			}
		}
	}
}
