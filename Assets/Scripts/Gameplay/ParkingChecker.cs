using System.Collections;
using EVP;
using SWS;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ParkingChecker : MonoBehaviour
{
	private bool park1;

	private bool park2;

	private bool park3;

	private bool park4;

	private float trafficSpeed;

	public GameObject parkingbar;

	public GameObject controls;

	private bool parkingComplete;

	private bool parkingEnter;

	private GameObject parkingObject;

	public LevelCompleteController gamecond;

	private GameObject passenger;

	public static bool parksucces;

	public static int passengerloadcount;

	private bool done;

	public CameraController campos;

	private bool passengerLoaded;

	private bool done1;

	public GameplayUIController controllers;

	private int count;

	private void Start()
	{
		passengerLoaded = false;
		passengerloadcount = 0;
		parksucces = false;
	}

	private void Update()
	{
		if (park1 && park2 && park3 && park4 && !done)
		{
			done = true;
			parkingComplete = true;
		}
		if (parkingComplete && parkingbar.GetComponent<Image>().fillAmount < 1f)
		{
			parkingbar.GetComponent<Image>().fillAmount += 0.05f;
		}
		else if (parkingComplete && parkingbar.GetComponent<Image>().fillAmount == 1f)
		{
			parkingComplete = false;
			park1 = false;
			park2 = false;
			park3 = false;
			park4 = false;
			parkingbar.GetComponent<Image>().fillAmount = 0f;
			if (passengerLoaded)
			{
				for (int i = 0; i < base.transform.Find("Passenger").transform.childCount; i++)
				{
					base.transform.Find("Passenger").transform.GetChild(i).gameObject.SetActive(true);
				}
				controls.SetActive(false);
				GetComponent<RigidbodyPause>().pause = true;
				GetComponent<VehicleAudio>().enabled = false;
				controllers.BreakButtonPressed();
				campos.RotateCam(base.gameObject);
				StartCoroutine(GameComplete());
			}
			else
			{
				campos.SetDefaultCam(base.gameObject);
				parksucces = true;
				done1 = true;
			}
		}
		if (passengerloadcount >= LevelManager.passenger && !parksucces)
		{
			passengerLoaded = true;
			LevelManager.lastparking.SetActive(true);
		}
		if (parksucces)
		{
			StartCoroutine(characterDelay());
			parkingObject.transform.GetChild(0).gameObject.SetActive(false);
			parkingObject.transform.GetChild(1).gameObject.SetActive(false);
			controls.SetActive(false);
			GetComponent<RigidbodyPause>().pause = true;
			controllers.BreakButtonPressed();
			done = false;
		}
		else if (done1)
		{
			done1 = false;
			controls.SetActive(true);
			GetComponent<RigidbodyPause>().pause = true;
			GetComponent<RigidbodyPause>().pause = false;
			controllers.BreakButtonReleased();
		}
	}

	private IEnumerator characterDelay()
	{
		passenger = parkingObject.transform.Find("passenger").gameObject;
		passenger.transform.GetChild(count).gameObject.GetComponent<PassengerWalker>().enabled = true;
		yield return new WaitForSeconds(1f);
		if (count < 2)
		{
			StartCoroutine(characterDelay());
			count++;
		}
		else
		{
			StartCoroutine(ParkAccessFalse());
			StopCoroutine("characterDelay");
			count = 0;
		}
	}

	private IEnumerator ParkAccessFalse()
	{
		yield return new WaitForSeconds(2f);
		parksucces = false;
	}

	private IEnumerator GameComplete()
	{
		yield return new WaitForSeconds(1.5f);
		gamecond.LevelComplete();
	}

	private void OnCollisionEnter(Collision coll)
	{
		if (coll.gameObject.tag == "over")
		{
			gamecond.LevelFail();
		}
	}

	private void OnTriggerEnter(Collider coll)
	{
		if (coll.gameObject.tag == "trafic")
		{
			trafficSpeed = coll.gameObject.GetComponent<NavMeshAgent>().speed;
			coll.gameObject.GetComponent<navMove>().ChangeSpeed(0f);
		}
	}

	private void OnTriggerStay(Collider coll)
	{
		if (coll.gameObject.tag == "park1")
		{
			park1 = true;
			parkingObject = coll.gameObject.transform.parent.gameObject.transform.parent.gameObject;
		}
		if (coll.gameObject.tag == "park2")
		{
			park2 = true;
			parkingObject = coll.gameObject.transform.parent.gameObject.transform.parent.gameObject;
		}
		if (coll.gameObject.tag == "park3")
		{
			park3 = true;
			parkingObject = coll.gameObject.transform.parent.gameObject.transform.parent.gameObject;
		}
		if (coll.gameObject.tag == "park4")
		{
			park4 = true;
			parkingObject = coll.gameObject.transform.parent.gameObject.transform.parent.gameObject;
		}
	}

	private void OnTriggerExit(Collider coll)
	{
		if (coll.gameObject.tag == "trafic")
		{
			coll.gameObject.GetComponent<navMove>().ChangeSpeed(trafficSpeed);
		}
		if (coll.gameObject.tag == "park1")
		{
			park1 = false;
			done = false;
			parkingComplete = false;
			parkingbar.GetComponent<Image>().fillAmount = 0f;
		}
		if (coll.gameObject.tag == "park2")
		{
			park2 = false;
			done = false;
			parkingComplete = false;
			parkingbar.GetComponent<Image>().fillAmount = 0f;
		}
		if (coll.gameObject.tag == "park3")
		{
			park3 = false;
			done = false;
			parkingComplete = false;
			parkingbar.GetComponent<Image>().fillAmount = 0f;
		}
		if (coll.gameObject.tag == "park4")
		{
			park4 = false;
			done = false;
			parkingComplete = false;
			parkingbar.GetComponent<Image>().fillAmount = 0f;
		}
	}
}
