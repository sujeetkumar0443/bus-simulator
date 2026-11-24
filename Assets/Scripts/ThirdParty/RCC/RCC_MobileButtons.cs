using UnityEngine;
using BusSimulator;

[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/Mobile/RCC UI Mobile Buttons")]
public class RCC_MobileButtons : MonoBehaviour
{
	private RCC_Settings RCCSettingsInstance;

	public RCC_UIController gasButton;

	public RCC_UIController gradualGasButton;

	public RCC_UIController brakeButton;

	public RCC_UIController leftButton;

	public RCC_UIController rightButton;

	public RCC_UISteeringWheelController steeringWheel;

	public RCC_UIController handbrakeButton;

	public RCC_UIController NOSButton;

	public RCC_UIController NOSButtonSteeringWheel;

	public RCC_UIDrag drag;

	public GameObject gearButton;

	public RCC_UIJoystick joystick;

	private float gasInput;

	private float brakeInput;

	private float leftInput;

	private float rightInput;

	private float steeringWheelInput;

	private float handbrakeInput;

	private float NOSInput = 1f;

	private float gyroInput;

	private float joystickInput;

	private bool canUseNos;

	private Vector3 orgBrakeButtonPos;

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

	private void Start()
	{
		if (RCCSettings.controllerType != RCC_Settings.ControllerType.Mobile)
		{
			if ((bool)gasButton)
			{
				gasButton.gameObject.SetActive(false);
			}
			if ((bool)gradualGasButton)
			{
				gradualGasButton.gameObject.SetActive(false);
			}
			if ((bool)leftButton)
			{
				leftButton.gameObject.SetActive(false);
			}
			if ((bool)rightButton)
			{
				rightButton.gameObject.SetActive(false);
			}
			if ((bool)brakeButton)
			{
				brakeButton.gameObject.SetActive(false);
			}
			if ((bool)steeringWheel)
			{
				steeringWheel.gameObject.SetActive(false);
			}
			if ((bool)handbrakeButton)
			{
				handbrakeButton.gameObject.SetActive(false);
			}
			if ((bool)NOSButton)
			{
				NOSButton.gameObject.SetActive(false);
			}
			if ((bool)NOSButtonSteeringWheel)
			{
				NOSButtonSteeringWheel.gameObject.SetActive(false);
			}
			if ((bool)gearButton)
			{
				gearButton.gameObject.SetActive(false);
			}
			if ((bool)joystick)
			{
				joystick.gameObject.SetActive(false);
			}
			base.enabled = false;
		}
		else
		{
			orgBrakeButtonPos = brakeButton.transform.position;
		}
	}

	private void Update()
	{
		switch (RCCSettings.mobileController)
		{
		case RCC_Settings.MobileController.TouchScreen:
			gyroInput = 0f;
			if ((bool)steeringWheel && steeringWheel.gameObject.activeInHierarchy)
			{
				steeringWheel.gameObject.SetActive(false);
			}
			if ((bool)NOSButton && NOSButton.gameObject.activeInHierarchy != canUseNos)
			{
				NOSButton.gameObject.SetActive(canUseNos);
			}
			if ((bool)joystick && joystick.gameObject.activeInHierarchy)
			{
				joystick.gameObject.SetActive(false);
			}
			if (!leftButton.gameObject.activeInHierarchy)
			{
				brakeButton.transform.position = orgBrakeButtonPos;
				leftButton.gameObject.SetActive(true);
			}
			if (!rightButton.gameObject.activeInHierarchy)
			{
				rightButton.gameObject.SetActive(true);
			}
			break;
		case RCC_Settings.MobileController.Gyro:
			gyroInput = Input.acceleration.x * RCCSettings.gyroSensitivity * PlayerPrefs.GetFloat("tiltsens");
			brakeButton.transform.position = rightButton.transform.position;
			if (steeringWheel.gameObject.activeInHierarchy)
			{
				steeringWheel.gameObject.SetActive(false);
			}
			if ((bool)NOSButton && NOSButton.gameObject.activeInHierarchy != canUseNos)
			{
				NOSButton.gameObject.SetActive(canUseNos);
			}
			if ((bool)joystick && joystick.gameObject.activeInHierarchy)
			{
				joystick.gameObject.SetActive(false);
			}
			if (leftButton.gameObject.activeInHierarchy)
			{
				leftButton.gameObject.SetActive(false);
			}
			if (rightButton.gameObject.activeInHierarchy)
			{
				rightButton.gameObject.SetActive(false);
			}
			break;
		case RCC_Settings.MobileController.SteeringWheel:
			gyroInput = 0f;
			if (!steeringWheel.gameObject.activeInHierarchy)
			{
				steeringWheel.gameObject.SetActive(true);
				brakeButton.transform.position = orgBrakeButtonPos;
			}
			if ((bool)NOSButton && NOSButton.gameObject.activeInHierarchy)
			{
				NOSButton.gameObject.SetActive(false);
			}
			if ((bool)NOSButtonSteeringWheel && NOSButtonSteeringWheel.gameObject.activeInHierarchy != canUseNos)
			{
				NOSButtonSteeringWheel.gameObject.SetActive(canUseNos);
			}
			if ((bool)joystick && joystick.gameObject.activeInHierarchy)
			{
				joystick.gameObject.SetActive(false);
			}
			if (leftButton.gameObject.activeInHierarchy)
			{
				leftButton.gameObject.SetActive(false);
			}
			if (rightButton.gameObject.activeInHierarchy)
			{
				rightButton.gameObject.SetActive(false);
			}
			break;
		case RCC_Settings.MobileController.Joystick:
			gyroInput = 0f;
			if ((bool)steeringWheel && steeringWheel.gameObject.activeInHierarchy)
			{
				steeringWheel.gameObject.SetActive(false);
			}
			if ((bool)NOSButton && NOSButton.gameObject.activeInHierarchy != canUseNos)
			{
				NOSButton.gameObject.SetActive(canUseNos);
			}
			if ((bool)joystick && !joystick.gameObject.activeInHierarchy)
			{
				joystick.gameObject.SetActive(true);
				brakeButton.transform.position = orgBrakeButtonPos;
			}
			if (leftButton.gameObject.activeInHierarchy)
			{
				leftButton.gameObject.SetActive(false);
			}
			if (rightButton.gameObject.activeInHierarchy)
			{
				rightButton.gameObject.SetActive(false);
			}
			break;
		}
		gasInput = GetInput(gasButton) + GetInput(gradualGasButton);
		brakeInput = GetInput(brakeButton);
		leftInput = GetInput(leftButton);
		rightInput = GetInput(rightButton);
		handbrakeInput = GetInput(handbrakeButton);
		NOSInput = Mathf.Clamp((GetInput(NOSButton) + GetInput(NOSButtonSteeringWheel)) * 2.5f, 1f, 2.5f);
		if ((bool)steeringWheel)
		{
			steeringWheelInput = steeringWheel.input;
		}
		if ((bool)joystick)
		{
			joystickInput = joystick.inputHorizontal;
		}
		if ((bool)RCC_SceneManager.Instance.activePlayerVehicle)
		{
			canUseNos = RCC_SceneManager.Instance.activePlayerVehicle.useNOS;
			if (RCC_SceneManager.Instance.activePlayerVehicle.canControl && !RCC_SceneManager.Instance.activePlayerVehicle.externalController)
			{
				RCC_SceneManager.Instance.activePlayerVehicle.gasInput = gasInput;
				RCC_SceneManager.Instance.activePlayerVehicle.brakeInput = brakeInput;
				RCC_SceneManager.Instance.activePlayerVehicle.steerInput = 0f - leftInput + rightInput + steeringWheelInput + gyroInput + joystickInput;
				RCC_SceneManager.Instance.activePlayerVehicle.handbrakeInput = handbrakeInput;
				RCC_SceneManager.Instance.activePlayerVehicle.boostInput = NOSInput;
			}
		}
	}

	private float GetInput(RCC_UIController button)
	{
		if (button == null)
		{
			return 0f;
		}
		return button.input;
	}

	public void SetMobileController(int index)
	{
		switch (index)
		{
		case 0:
			RCC.SetMobileController(RCC_Settings.MobileController.TouchScreen);
			break;
		case 1:
			RCC.SetMobileController(RCC_Settings.MobileController.Gyro);
			break;
		case 2:
			RCC.SetMobileController(RCC_Settings.MobileController.SteeringWheel);
			break;
		case 3:
			RCC.SetMobileController(RCC_Settings.MobileController.Joystick);
			break;
		}
	}

	public void SetQuality(int index)
	{
		QualitySettings.SetQualityLevel(index);
	}

	public void ChangeCamera()
	{
		if ((bool)RCC_SceneManager.Instance.activePlayerCamera)
		{
			RCC_SceneManager.Instance.activePlayerCamera.ChangeCamera();
		}
	}
}
