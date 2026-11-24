using UnityEngine;

public class PassengersPAthSettig : MonoBehaviour
{
	private void Start()
	{
		base.transform.Find("passengers").transform.GetChild(VehicleManager.vNo).gameObject.SetActive(true);
	}
}
