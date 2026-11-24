using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/RCC UI Dashboard Displayer")]
[RequireComponent(typeof(RCC_DashboardInputs))]
public class RCC_UIDashboardDisplay : MonoBehaviour
{
	public enum DisplayType
	{
		Full = 0,
		Customization = 1,
		TopButtonsOnly = 2,
		Off = 3
	}

	private RCC_Settings RCCSettingsInstance;

	private RCC_DashboardInputs inputs;

	public DisplayType displayType;

	public GameObject topButtons;

	public GameObject controllerButtons;

	public GameObject gauges;

	public GameObject customizationMenu;

	public Text RPMLabel;

	public Text KMHLabel;

	public Text GearLabel;

	public Text recordingLabel;

	public Image ABS;

	public Image ESP;

	public Image Park;

	public Image Headlights;

	public Image leftIndicator;

	public Image rightIndicator;

	public Image heatIndicator;

	public Image fuelIndicator;

	public Image rpmIndicator;

	private RCC_Settings RCCSettings
	{
		get
		{
			if (RCCSettingsInstance == null)
			{
				RCCSettingsInstance = RCC_Settings.Instance;
			}
			return RCCSettingsInstance;
		}
	}

	private void Awake()
	{
		inputs = GetComponent<RCC_DashboardInputs>();
		if (!inputs)
		{
			base.enabled = false;
		}
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
	}

	private void Update()
	{
		switch (displayType)
		{
		case DisplayType.Full:
			break;
		case DisplayType.Customization:
			if ((bool)topButtons && topButtons.activeInHierarchy)
			{
				topButtons.SetActive(false);
			}
			if ((bool)controllerButtons && controllerButtons.activeInHierarchy)
			{
				controllerButtons.SetActive(false);
			}
			if ((bool)gauges && gauges.activeInHierarchy)
			{
				gauges.SetActive(false);
			}
			if ((bool)customizationMenu && !customizationMenu.activeInHierarchy)
			{
				customizationMenu.SetActive(true);
			}
			break;
		case DisplayType.TopButtonsOnly:
			if (!topButtons.activeInHierarchy)
			{
				topButtons.SetActive(true);
			}
			if (controllerButtons.activeInHierarchy)
			{
				controllerButtons.SetActive(false);
			}
			if (gauges.activeInHierarchy)
			{
				gauges.SetActive(false);
			}
			if (customizationMenu.activeInHierarchy)
			{
				customizationMenu.SetActive(false);
			}
			break;
		case DisplayType.Off:
			if ((bool)topButtons && topButtons.activeInHierarchy)
			{
				topButtons.SetActive(false);
			}
			if ((bool)controllerButtons && controllerButtons.activeInHierarchy)
			{
				controllerButtons.SetActive(false);
			}
			if ((bool)gauges && gauges.activeInHierarchy)
			{
				gauges.SetActive(false);
			}
			if ((bool)customizationMenu && customizationMenu.activeInHierarchy)
			{
				customizationMenu.SetActive(false);
			}
			break;
		}
	}

	private void LateUpdate()
	{
		if (!RCC_SceneManager.Instance.activePlayerVehicle)
		{
			return;
		}
		if ((bool)RPMLabel)
		{
			RPMLabel.text = inputs.RPM.ToString("0");
		}
		if ((bool)KMHLabel)
		{
			if (RCCSettings.units == RCC_Settings.Units.KMH)
			{
				KMHLabel.text = inputs.KMH.ToString("0");
			}
			else
			{
				KMHLabel.text = (inputs.KMH * 0.62f).ToString("0");
			}
		}
		if ((bool)GearLabel)
		{
			if (!inputs.NGear && !inputs.changingGear)
			{
				GearLabel.text = ((inputs.direction != 1) ? "R" : (inputs.Gear + 1f).ToString("0"));
			}
			else
			{
				GearLabel.text = "N";
			}
		}
		if ((bool)recordingLabel)
		{
			switch (RCC_SceneManager.Instance.recordMode)
			{
			case RCC_SceneManager.RecordMode.Neutral:
				if (recordingLabel.gameObject.activeInHierarchy)
				{
					recordingLabel.gameObject.SetActive(false);
				}
				recordingLabel.text = string.Empty;
				break;
			case RCC_SceneManager.RecordMode.Play:
				if (!recordingLabel.gameObject.activeInHierarchy)
				{
					recordingLabel.gameObject.SetActive(true);
				}
				recordingLabel.text = "Playing";
				recordingLabel.color = Color.green;
				break;
			case RCC_SceneManager.RecordMode.Record:
				if (!recordingLabel.gameObject.activeInHierarchy)
				{
					recordingLabel.gameObject.SetActive(true);
				}
				recordingLabel.text = "Recording";
				recordingLabel.color = Color.red;
				break;
			}
		}
		if ((bool)ABS)
		{
			ABS.color = ((!inputs.ABS) ? Color.white : Color.yellow);
		}
		if ((bool)ESP)
		{
			ESP.color = ((!inputs.ESP) ? Color.white : Color.yellow);
		}
		if ((bool)Park)
		{
			Park.color = ((!inputs.Park) ? Color.white : Color.red);
		}
		if ((bool)Headlights)
		{
			Headlights.color = ((!inputs.Headlights) ? new Color(0.55f, 0.55f, 0.55f) : Color.white);
		}
		if ((bool)heatIndicator)
		{
			heatIndicator.color = ((!(RCC_SceneManager.Instance.activePlayerVehicle.engineHeat >= 100f)) ? new Color(0.1f, 0f, 0f) : Color.red);
		}
		if ((bool)fuelIndicator)
		{
			fuelIndicator.color = ((!(RCC_SceneManager.Instance.activePlayerVehicle.fuelTank < 10f)) ? new Color(0.1f, 0f, 0f) : Color.red);
		}
		if ((bool)rpmIndicator)
		{
			rpmIndicator.color = ((!(RCC_SceneManager.Instance.activePlayerVehicle.engineRPM >= RCC_SceneManager.Instance.activePlayerVehicle.maxEngineRPM - 500f)) ? new Color(0.1f, 0f, 0f) : Color.red);
		}
		if ((bool)leftIndicator && (bool)rightIndicator)
		{
			switch (inputs.indicators)
			{
			case RCC_CarControllerV3.IndicatorsOn.Left:
				leftIndicator.color = Color.white;
				rightIndicator.color = new Color(0.45f, 0.45f, 0.45f);
				break;
			case RCC_CarControllerV3.IndicatorsOn.Right:
				leftIndicator.color = new Color(0.45f, 0.45f, 0.45f);
				rightIndicator.color = Color.white;
				break;
			case RCC_CarControllerV3.IndicatorsOn.All:
				leftIndicator.color = Color.white;
				rightIndicator.color = Color.white;
				break;
			case RCC_CarControllerV3.IndicatorsOn.Off:
				leftIndicator.color = new Color(0.45f, 0.45f, 0.45f);
				rightIndicator.color = new Color(0.45f, 0.45f, 0.45f);
				break;
			}
		}
	}

	public void SetDisplayType(DisplayType _displayType)
	{
		displayType = _displayType;
	}
}
