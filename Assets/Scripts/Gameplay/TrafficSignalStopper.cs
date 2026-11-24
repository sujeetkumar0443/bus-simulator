using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TrafficSignalStopper : MonoBehaviour
{
	public static bool StopAtSignal;

	private float speed;

	private void Start()
	{
		StopAtSignal = false;
		speed = GetComponent<NavMeshAgent>().speed;
	}

	private void Update()
	{
	}

	private IEnumerator CheckSignalPosition(GameObject obj)
	{
		yield return new WaitForSeconds(0.5f);
		if (!obj.transform.GetChild(0).gameObject.activeSelf)
		{
			StopAtSignal = false;
			StopAllCoroutines();
		}
		else
		{
			StartCoroutine(CheckSignalPosition(obj));
		}
	}

	public void CheckSig(GameObject obj)
	{
		if (obj.transform.GetChild(0).gameObject.activeSelf)
		{
			StopAtSignal = true;
			StartCoroutine(CheckSignalPosition(obj));
		}
	}
}
