using System;
using SWS;
using UnityEngine;

public class PassengerController : MonoBehaviour
{
	public bool idle;

	public bool walk;

	public bool sitdown;

	public bool standup;

	private Animator animator;

	public bool ActiveNow;

	public bool isInBus;

	private bool exitedPerson;

	private int lastIndex;

	private bool sitAlready;

	private GameObject allPaths;

	private GameObject thisInstance;

	private void OnEnable()
	{
		GameplayUIController.door = (Action<bool>)Delegate.Combine(GameplayUIController.door, new Action<bool>(MoveNow));
	}

	private void OnDisable()
	{
		GameplayUIController.door = (Action<bool>)Delegate.Remove(GameplayUIController.door, new Action<bool>(MoveNow));
	}

	private void Start()
	{
		GetComponent<splineMove>().Stop();
		animator = GetComponent<Animator>();
		animator.SetBool("isidle", idle);
		animator.SetBool("iswalk", walk);
		animator.SetBool("issit", sitdown);
		animator.SetBool("isstand", standup);
		thisInstance = base.gameObject;
	}

	public void MoveNow(bool temp)
	{
		if (ActiveNow)
		{
			if (temp)
			{
				animator.SetBool("iswalk", true);
				animator.SetBool("isidle", false);
				thisInstance.GetComponent<splineMove>().StartMove();
			}
			else
			{
				animator.SetBool("iswalk", false);
				animator.SetBool("issit", false);
				animator.SetBool("isstand", true);
				Invoke("walkNow", 3f);
			}
		}
	}

	private void walkNow()
	{
		thisInstance.GetComponent<splineMove>().enabled = true;
		thisInstance.GetComponent<splineMove>().startPoint = lastIndex - 1;
		thisInstance.GetComponent<splineMove>().reverse = true;
		animator.SetBool("issit", false);
		animator.SetBool("iswalk", true);
		animator.SetBool("isstand", false);
		exitedPerson = true;
		thisInstance.GetComponent<splineMove>().StartMove();
	}

	public void SitNow()
	{
		if (!sitAlready)
		{
			sitAlready = true;
			lastIndex = thisInstance.GetComponent<splineMove>().currentPoint;
			thisInstance.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			animator.SetBool("issit", true);
			BusController.passengerSittingCount++;
			thisInstance.GetComponent<splineMove>().Stop();
			thisInstance.GetComponent<splineMove>().enabled = false;
		}
	}

	public void StopAndTurnArround()
	{
		if (exitedPerson)
		{
			animator.SetBool("iswalk", false);
			animator.SetBool("isidle", true);
			animator.SetBool("isstand", false);
			thisInstance.transform.localRotation = Quaternion.Euler(thisInstance.transform.localRotation.x, -90f, thisInstance.transform.localRotation.z);
		}
	}
}
