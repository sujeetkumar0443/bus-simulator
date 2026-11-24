using System.Collections;
using UnityEngine;

namespace BusSimulator
{
	public class PassengerDropMovement : MonoBehaviour
	{
		private void Start()
		{
			StartCoroutine(DropPassenger());
		}

		private IEnumerator DropPassenger()
		{
			while (true)
			{
				yield return new WaitForSeconds(0.01f);
				base.gameObject.transform.LookAt(LevelManager.lastparking.transform.GetChild(2).transform.position);
				base.gameObject.GetComponent<Animation>().Play("Move_Walk225");
				base.transform.position = Vector3.MoveTowards(base.transform.position, LevelManager.lastparking.transform.GetChild(2).transform.position, 0.06f);
			}
		}
	}
}
