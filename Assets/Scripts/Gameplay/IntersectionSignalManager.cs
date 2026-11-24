using System.Collections;
using UnityEngine;

namespace BusSimulator
{
	public class IntersectionSignalManager : MonoBehaviour
	{
		public int signalChowkType = 3;

		public float greenLightDelay;

		public float YellowLightDelay;

		public float RedLightDelay;

		private void Start()
		{
			StartCoroutine(StartSignal());
		}

		private IEnumerator StartSignal()
		{
			if (signalChowkType == 3)
			{
				base.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
				base.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
				yield return new WaitForSeconds(greenLightDelay);
				base.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
				base.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(true);
				base.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
				yield return new WaitForSeconds(YellowLightDelay);
				base.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(true);
				base.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
				yield return new WaitForSeconds(RedLightDelay);
				base.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(true);
				base.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(true);
				base.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
				yield return new WaitForSeconds(YellowLightDelay);
				base.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(true);
				yield return new WaitForSeconds(greenLightDelay);
				base.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
				base.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(true);
				base.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
				yield return new WaitForSeconds(YellowLightDelay);
			}
			else
			{
				base.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
				base.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(false);
				yield return new WaitForSeconds(greenLightDelay);
				base.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
				base.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(true);
				base.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(false);
				yield return new WaitForSeconds(YellowLightDelay);
				base.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(true);
				base.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(false);
				yield return new WaitForSeconds(RedLightDelay);
				base.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(true);
				base.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(true);
				base.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(false);
				yield return new WaitForSeconds(YellowLightDelay);
				base.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(true);
				base.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(false);
				yield return new WaitForSeconds(greenLightDelay);
				base.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(true);
				base.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(true);
				base.transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(false);
				yield return new WaitForSeconds(YellowLightDelay);
				base.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(true);
				yield return new WaitForSeconds(RedLightDelay);
				base.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
				base.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
				base.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
				base.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);
				base.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(true);
				base.transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(false);
				yield return new WaitForSeconds(YellowLightDelay);
			}
			StartCoroutine(StartSignal());
		}

		private void Update()
		{
		}
	}
}
