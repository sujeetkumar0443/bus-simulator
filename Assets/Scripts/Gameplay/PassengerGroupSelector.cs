using UnityEngine;

namespace BusSimulator
{
	public class PassengerGroupSelector : MonoBehaviour
	{
		private void Start()
		{
			base.transform.Find("passengers").transform.GetChild(VehicleManager.vNo).gameObject.SetActive(true);
		}
	}
}
