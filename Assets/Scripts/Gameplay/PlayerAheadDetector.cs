using SWS;
using UnityEngine;
using UnityEngine.AI;

namespace BusSimulator
{
	public class PlayerAheadDetector : MonoBehaviour
	{
		private RaycastHit hit;

		public GameObject left;

		public GameObject right;

		public GameObject center;

		private float previousSpeed;

		private bool playerDetected;

		private void Start()
		{
			previousSpeed = GetComponent<NavMeshAgent>().speed;
		}

		private void Update()
		{
			if (Physics.Raycast(center.transform.position, center.transform.forward, out hit, 10f) && !playerDetected)
			{
				if (hit.transform.tag == "Player")
				{
					playerDetected = true;
					GetComponent<navMove>().ChangeSpeed(0f);
				}
				else
				{
					GetComponent<navMove>().ChangeSpeed(previousSpeed);
				}
			}
			else if (Physics.Raycast(left.transform.position, left.transform.forward, out hit, 10f) && !playerDetected)
			{
				if (hit.transform.tag == "Player")
				{
					playerDetected = true;
					GetComponent<navMove>().ChangeSpeed(0f);
				}
				else
				{
					GetComponent<navMove>().ChangeSpeed(previousSpeed);
				}
			}
			else if (Physics.Raycast(right.transform.position, right.transform.forward, out hit, 10f) && !playerDetected)
			{
				if (hit.transform.tag == "Player")
				{
					playerDetected = true;
					GetComponent<navMove>().ChangeSpeed(0f);
				}
				else
				{
					GetComponent<navMove>().ChangeSpeed(previousSpeed);
				}
			}
			else if (playerDetected)
			{
				playerDetected = false;
				Debug.Log("speeeeed : " + GetComponent<NavMeshAgent>().speed);
				GetComponent<navMove>().ChangeSpeed(previousSpeed);
			}
		}
	}
}
