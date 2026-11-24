using System;
using System.Collections;
using SWS;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BusController : MonoBehaviour
{
	public CameraController camcontrol;

	public GameObject doors;

	public GameObject passengers;

	private bool park1;

	private bool park2;

	private bool park3;

	private bool park4;
	 
	private float traficspeed;

	private GameObject parkingobj;

	private GameObject stationPassengers;

	public GameObject parkingbar;

	private bool parkingcomplete;

	private bool parkingCompletedCheck;

	public GameObject vehicleControls;

	public GameObject busdoorbtn;

	public int passengersTotalCount;

	public int inBusPassengersTotalCount;

	public static int passengerSittingCount;

	public static int passengerStandingCount;

	private int rand;

	private int taskTotal;

	public LevelCompleteController gamecond;

	private bool checkDirection;

	public GameObject TurnArround;

	public GameplayUIController controller;

	public GameObject fuelBars;

	public LevelTimer time;

	public GameObject SpeedUppassengers;

	public GameObject MiniMap;

	public SoundManager sound;

	public Text levelFailReason;

	public Text StopHereText;

	private bool checkPassengers;

	private bool passengerLoaded;

	private void Start()
	{
		passengersTotalCount = 0;
		passengerSittingCount = 0;
		passengers = base.transform.Find("passengers").gameObject;
	}

	private void OnEnable()
	{
		GameplayUIController.BusDoorEvent = (Action<bool>)Delegate.Combine(GameplayUIController.BusDoorEvent, new Action<bool>(OpenDoor));
	}

	private void OnDisable()
	{
		GameplayUIController.BusDoorEvent = (Action<bool>)Delegate.Remove(GameplayUIController.BusDoorEvent, new Action<bool>(OpenDoor));
	}

	private void OpenDoor(bool temp)
	{
		if (temp)
		{
			for (int i = 0; i < doors.transform.childCount; i++)
			{
				doors.transform.GetChild(i).transform.GetChild(0).GetComponent<Animator>().SetBool("doorclose", false);
				doors.transform.GetChild(i).transform.GetChild(0).GetComponent<Animator>().SetBool("dooropen", true);
			}
			return;
		}
		sound.BusDoorOpenSound(false);
		for (int j = 0; j < doors.transform.childCount; j++)
		{
			doors.transform.GetChild(j).transform.GetChild(0).GetComponent<Animator>().SetBool("dooropen", false);
			doors.transform.GetChild(j).transform.GetChild(0).GetComponent<Animator>().SetBool("doorclose", true);
		}
	}

	private void GameComplete()
	{
		gamecond.LevelComplete();
	}

	private IEnumerator ParkingBarFillUp()
	{
		Debug.Log("ParkingBarFillUp");
		parkingbar.SetActive(true);
		while (true)
		{
			yield return new WaitForSeconds(0f);
			if (parkingbar.GetComponent<Image>().fillAmount < 0.9f)
			{
				parkingbar.GetComponent<Image>().fillAmount += 0.05f;
				continue;
			}
			break;
		}
		CheckParkingCompletion();
	}

	private void CheckParkingCompletion()
	{
		taskTotal++;
		if (LevelManager.TotalsParking == taskTotal)
		{
			GameState.LevelOver = true;
			Invoke("GameComplete", 10f);
		}
		else
		{
			stationPassengers = parkingobj.transform.Find("passengers").gameObject;
		}
		park1 = false;
		park2 = false;
		park3 = false;
		park4 = false;
		parkingobj.tag = "Untagged";
		parkingobj.transform.GetChild(0).gameObject.SetActive(false);
		parkingobj.transform.GetChild(1).gameObject.SetActive(false);
		parkingobj.transform.GetChild(0).GetComponent<bl_MiniMapItem>().DestroyItem(true);
		parkingobj.transform.parent.transform.GetChild(1).gameObject.SetActive(true);
		parkingbar.GetComponent<Image>().fillAmount = 0f;
		parkingbar.SetActive(false);
		parkingcomplete = false;
		vehicleControls.SetActive(false);
		camcontrol.SetDoorCam();
		selectPassengers();
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		controller.ResetInputs();
		Invoke("miniMapDelay_Off", 1.5f);
	}

	private void miniMapDelay_Off()
	{
		MiniMap.SetActive(false);
	}

	private void BackToControls()
	{
		vehicleControls.SetActive(true);
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		camcontrol.ResetCamPosition();
		MiniMap.SetActive(true);
		SpeedUppassengers.SetActive(false);
		Time.timeScale = 1f;
	}

	private void selectPassengers()
	{
		Debug.Log("vehicle no : " + VehicleManager.vNo);
		if (passengers.transform.childCount > 0)
		{
			Debug.Log("passenger count : " + passengers.transform.childCount);
			rand = UnityEngine.Random.Range(3, passengers.transform.childCount);
			for (int i = 0; i < rand; i++)
			{
				passengers.transform.GetChild(i).GetComponent<PassengerController>().ActiveNow = true;
			}
			busdoorbtn.SetActive(true);
		}
		else
		{
			Debug.Log("station passenger : " + stationPassengers.transform.GetChild(VehicleManager.vNo).childCount);
			rand = UnityEngine.Random.Range(3, stationPassengers.transform.GetChild(VehicleManager.vNo).childCount);
			for (int j = 0; j < rand; j++)
			{
				stationPassengers.transform.GetChild(VehicleManager.vNo).transform.GetChild(j).GetComponent<PassengerController>().ActiveNow = true;
			}
			busdoorbtn.SetActive(true);
		}
	}

	private void SetFuelBar()
	{
		time.ResetTime();
	}

	private void checkpassengerSitting()
	{
		if (passengersTotalCount == passengerSittingCount && passengersTotalCount > 0)
		{
			passengersTotalCount = 0;
			passengerSittingCount = 0;
			Invoke("BackToControls", 4f);
			checkPassengers = false;
			passengerLoaded = true;
			GameplayUIController.BusDoorEvent(false);
			CancelInvoke("checkpassengerSitting");
		}
	}

	private IEnumerator ResetPumpParking(GameObject pump)
	{
		yield return new WaitForSeconds(10f);
		pump.SetActive(true);
		pump.transform.GetChild(0).GetComponent<bl_MiniMapItem>().ShowItem();
	}

	private void LevelFailDelay()
	{
		gamecond.LevelFail();
	}

	private void OnCollisionEnter(Collision coll)
	{
		Debug.Log("colllider with");
		if (GetComponent<Rigidbody>().velocity.magnitude > 2f)
		{
			Debug.Log("colllider with after");
			sound.BusHitSound();
		}
	}

	private void OnTriggerEnter(Collider coll)
	{
		if (coll.gameObject.tag == "passenger" && parkingCompletedCheck && !passengerLoaded)
		{
			coll.gameObject.GetComponent<PassengerController>().isInBus = true;
			coll.gameObject.transform.parent = passengers.transform;
			coll.gameObject.tag = "inbuspassenger";
			passengersTotalCount++;
			if (!checkPassengers)
			{
				checkPassengers = true;
				InvokeRepeating("checkpassengerSitting", 1f, 1f);
			}
		}
		if (coll.gameObject.tag == "passenger" && GetComponent<Rigidbody>().velocity.magnitude > 3f)
		{
			coll.gameObject.tag = "Untagged";
			levelFailReason.text = "You hit the passengers";
			sound.BusHitSound();
			Invoke("LevelFailDelay", 0.4f);
		}
		if (coll.gameObject.tag == "pumpparking")
		{
			VehicleManager.vehicleManager.StopVehicle(true);
			coll.gameObject.SetActive(false);
			StartCoroutine(ResetPumpParking(coll.gameObject));
			coll.gameObject.transform.GetChild(0).GetComponent<bl_MiniMapItem>().HideItem();
			fuelBars.GetComponent<Animator>().SetBool("open", true);
			Invoke("SetFuelBar", fuelBars.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
		}
		Debug.Log("Trigger enter in :" + coll.gameObject.tag);
		if (coll.gameObject.tag == "trafic")
		{
			traficspeed = coll.gameObject.GetComponent<NavMeshAgent>().speed;
			coll.gameObject.GetComponent<navMove>().ChangeSpeed(0f);
		}
		if (coll.gameObject.tag == "water")
		{
			levelFailReason.text = string.Empty;
			gamecond.LevelFail();
		}
	}

	private void StopHereTextDisable()
	{
		StopHereText.gameObject.SetActive(false);
	}

	private void OnTriggerStay(Collider coll)
	{
		if (coll.gameObject.tag == "checkdirection")
		{
			if (Vector3.Dot(base.transform.forward, coll.gameObject.transform.forward) > 0f)
			{
				checkDirection = true;
				StopHereText.gameObject.SetActive(true);
				StopHereText.text = "APPLY BRAKES AND STOP HERE";
				Invoke("StopHereTextDisable", 2f);
			}
			else
			{
				TurnArround.SetActive(true);
			}
		}
		if (checkDirection)
		{
			if (coll.gameObject.tag == "park1" && !park1)
			{
				park1 = true;
				CheckComplete();
				parkingobj = coll.gameObject.transform.parent.gameObject.transform.parent.gameObject;
			}
			if (coll.gameObject.tag == "park2" && !park2)
			{
				park2 = true;
				CheckComplete();
				parkingobj = coll.gameObject.transform.parent.gameObject.transform.parent.gameObject;
			}
			if (coll.gameObject.tag == "park3" && !park3)
			{
				park3 = true;
				CheckComplete();
				parkingobj = coll.gameObject.transform.parent.gameObject.transform.parent.gameObject;
			}
			if (coll.gameObject.tag == "park4" && !park4)
			{
				park4 = true;
				CheckComplete();
				parkingobj = coll.gameObject.transform.parent.gameObject.transform.parent.gameObject;
			}
		}
	}

	private void CheckComplete()
	{
		if (park1 && park2 && park3 && park4 && !parkingcomplete)
		{
			parkingCompletedCheck = true;
			parkingcomplete = true;
			park1 = false;
			park2 = false;
			park3 = false;
			park4 = false;
			Debug.Log("completeeeee");
			StartCoroutine(ParkingBarFillUp());
		 
		}
	}

	private void OnTriggerExit(Collider coll)
	{
		if (coll.gameObject.tag == "trafic")
		{
			coll.gameObject.GetComponent<navMove>().ChangeSpeed(traficspeed);
		}
		if (coll.gameObject.tag == "checkdirection")
		{
			TurnArround.SetActive(false);
			checkDirection = false;
		}
		if (coll.gameObject.tag == "park1")
		{
			parkingcomplete = false;
			park1 = false;
			StopAllCoroutines();
			parkingbar.SetActive(false);
			parkingbar.GetComponent<Image>().fillAmount = 0f;
		}
		if (coll.gameObject.tag == "park2")
		{
			parkingcomplete = false;
			park2 = false;
			StopAllCoroutines();
			parkingbar.SetActive(false);
			parkingbar.GetComponent<Image>().fillAmount = 0f;
		}
		if (coll.gameObject.tag == "park3")
		{
			parkingcomplete = false;
			park3 = false;
			StopAllCoroutines();
			parkingbar.SetActive(false);
			parkingbar.GetComponent<Image>().fillAmount = 0f;
		}
		if (coll.gameObject.tag == "park4")
		{
			parkingcomplete = false;
			park4 = false;
			StopAllCoroutines();
			parkingbar.SetActive(false);
			parkingbar.GetComponent<Image>().fillAmount = 0f;
		}
	}
}
