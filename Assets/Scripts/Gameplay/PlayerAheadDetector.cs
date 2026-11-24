using SWS;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAheadDetector : MonoBehaviour
{
	private RaycastHit hit;

	public GameObject left;

	public GameObject right;

	public GameObject center;

	private float prevspeed;

	private bool playerdetected;

	private void Start()
	{
		prevspeed = GetComponent<NavMeshAgent>().speed;
	}

	private void Update()
	{
		if (Physics.Raycast(center.transform.position, center.transform.forward, out hit, 10f) && !playerdetected)
		{
			if (hit.transform.tag == "Player")
			{
				playerdetected = true;
				GetComponent<navMove>().ChangeSpeed(0f);
			}
			else
			{
				GetComponent<navMove>().ChangeSpeed(prevspeed);
			}
		}
		else if (Physics.Raycast(left.transform.position, left.transform.forward, out hit, 10f) && !playerdetected)
		{
			if (hit.transform.tag == "Player")
			{
				playerdetected = true;
				GetComponent<navMove>().ChangeSpeed(0f);
			}
			else
			{
				GetComponent<navMove>().ChangeSpeed(prevspeed);
			}
		}
		else if (Physics.Raycast(right.transform.position, right.transform.forward, out hit, 10f) && !playerdetected)
		{
			if (hit.transform.tag == "Player")
			{
				playerdetected = true;
				GetComponent<navMove>().ChangeSpeed(0f);
			}
			else
			{
				GetComponent<navMove>().ChangeSpeed(prevspeed);
			}
		}
		else if (playerdetected)
		{
			playerdetected = false;
			Debug.Log("speeeeed : " + GetComponent<NavMeshAgent>().speed);
			GetComponent<navMove>().ChangeSpeed(prevspeed);
		}
	}
}
