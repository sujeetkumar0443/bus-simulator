using UnityEngine;

public class ToD_Clock : MonoBehaviour
{
	public GameObject gTimeOfDay;

	public Transform tHourHand;

	public Transform tMinuteHand;

	private ToD_Base clToDBase;

	private float fHoursToDegrees = 30f;

	private float fMinutesToDegrees = 6f;

	private void Awake()
	{
		clToDBase = gTimeOfDay.GetComponent<ToD_Base>();
	}

	private void Update()
	{
		float num = 24f * clToDBase.Get_fCurrentTimeOfDay;
		float num2 = 60f * (num - Mathf.Floor(num));
		tHourHand.localRotation = Quaternion.Euler(0f, num * fHoursToDegrees, 0f);
		tMinuteHand.localRotation = Quaternion.Euler(0f, num2 * fMinutesToDegrees, 0f);
	}
}
