using UnityEngine;
using UnityEngine.UI;

public class DashboardController : MonoBehaviour
{
	public GameObject needleSprite;

	public GameObject rpmNeedleSprite;

	private float kph;

	public Text sppedText;

	private int speed;

	private RCC_CarControllerV3 playerHandler;

	public static DashboardController instance;

	public bool isWiperActive;

	public GameObject[] wipers;

	private float kmhNeedleRotation;

	private float rpmNeedleRotation;

	public GameObject hands;

	public GameObject Driver;

	public static DashboardController Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType<DashboardController>();
			}
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		instance = this;
		playerHandler = GetComponent<RCC_CarControllerV3>();
	}

	private void Update()
	{
		if ((bool)rpmNeedleSprite)
		{
			rpmNeedleRotation = playerHandler.engineRPM / 50f;
			rpmNeedleSprite.transform.localEulerAngles = new Vector3(rpmNeedleSprite.transform.localEulerAngles.x, rpmNeedleSprite.transform.localEulerAngles.y, 0f - rpmNeedleRotation);
		}
		if ((bool)needleSprite)
		{
			if (RCC_Settings.Instance.units == RCC_Settings.Units.KMH)
			{
				kmhNeedleRotation = playerHandler.speed;
			}
			else
			{
				kmhNeedleRotation = playerHandler.speed * 0.62f;
			}
			needleSprite.transform.localEulerAngles = new Vector3(needleSprite.transform.localEulerAngles.x, needleSprite.transform.localEulerAngles.y, 0f - kmhNeedleRotation);
		}
	}

	public void SetDriver(bool isDriver)
	{
		if (isDriver)
		{
			Driver.SetActive(true);
			hands.SetActive(false);
		}
		else
		{
			Driver.SetActive(false);
			hands.SetActive(true);
		}
	}

	public void ActivateWiper(bool isActivate)
	{
		isWiperActive = isActivate;
		if (wipers.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < wipers.Length; i++)
		{
			if (!wipers[i].GetComponent<Animator>().GetBool("wiperactive"))
			{
				wipers[i].GetComponent<Animator>().SetBool("wiperactive", true);
			}
			else
			{
				wipers[i].GetComponent<Animator>().SetBool("wiperactive", false);
			}
		}
	}
}
