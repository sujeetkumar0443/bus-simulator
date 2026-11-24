using System.Collections;
using UnityEngine;

public class ParkingAnimator : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(Animate());
	}

	private IEnumerator Animate()
	{
		base.transform.GetChild(0).gameObject.SetActive(false);
		base.transform.GetChild(1).gameObject.SetActive(true);
		base.transform.GetChild(2).gameObject.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		base.transform.GetChild(0).gameObject.SetActive(true);
		base.transform.GetChild(1).gameObject.SetActive(false);
		base.transform.GetChild(2).gameObject.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		base.transform.GetChild(0).gameObject.SetActive(true);
		base.transform.GetChild(1).gameObject.SetActive(true);
		base.transform.GetChild(2).gameObject.SetActive(false);
		yield return new WaitForSeconds(0.5f);
		base.transform.GetChild(0).gameObject.SetActive(true);
		base.transform.GetChild(1).gameObject.SetActive(false);
		base.transform.GetChild(2).gameObject.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(Animate());
	}
}
