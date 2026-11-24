using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Main/RCC Scene Manager")]
public class RCC_SceneManager : MonoBehaviour
{
	public enum RecordMode
	{
		Neutral = 0,
		Play = 1,
		Record = 2
	}

	private static RCC_SceneManager instance;

	public RCC_CarControllerV3 activePlayerVehicle;

	public RCC_Camera activePlayerCamera;

	public RCC_UIDashboardDisplay activePlayerCanvas;

	public Camera activeMainCamera;

	public bool registerFirstVehicleAsPlayer = true;

	public bool disableUIWhenNoPlayerVehicle;

	internal RCC_Recorder recorder;

	public RecordMode recordMode;

	private float orgTimeScale = 1f;

	public List<RCC_CarControllerV3> allVehicles = new List<RCC_CarControllerV3>();

	public static RCC_SceneManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType<RCC_SceneManager>();
				if (instance == null)
				{
					GameObject gameObject = new GameObject("_RCCSceneManager");
					instance = gameObject.AddComponent<RCC_SceneManager>();
				}
			}
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		RCC_Camera.OnBCGCameraSpawned += RCC_Camera_OnBCGCameraSpawned;
		RCC_CarControllerV3.OnRCCPlayerSpawned += RCC_CarControllerV3_OnRCCSpawned;
		RCC_AICarController.OnRCCAISpawned += RCC_AICarController_OnRCCAISpawned;
		RCC_CarControllerV3.OnRCCPlayerDestroyed += RCC_CarControllerV3_OnRCCPlayerDestroyed;
		RCC_AICarController.OnRCCAIDestroyed += RCC_AICarController_OnRCCAIDestroyed;
		activePlayerCanvas = Object.FindObjectOfType<RCC_UIDashboardDisplay>();
		orgTimeScale = Time.timeScale;
		recorder = base.gameObject.GetComponent<RCC_Recorder>();
		if (!recorder)
		{
			recorder = base.gameObject.AddComponent<RCC_Recorder>();
		}
	}

	private void RCC_CarControllerV3_OnRCCSpawned(RCC_CarControllerV3 RCC)
	{
		if (!allVehicles.Contains(RCC))
		{
			allVehicles.Add(RCC);
		}
		if (registerFirstVehicleAsPlayer)
		{
			RegisterPlayer(RCC);
		}
	}

	private void RCC_AICarController_OnRCCAISpawned(RCC_AICarController RCCAI)
	{
		if (!allVehicles.Contains(RCCAI.carController))
		{
			allVehicles.Add(RCCAI.carController);
		}
	}

	private void RCC_Camera_OnBCGCameraSpawned(GameObject BCGCamera)
	{
		activePlayerCamera = BCGCamera.GetComponent<RCC_Camera>();
	}

	private void RCC_CarControllerV3_OnRCCPlayerDestroyed(RCC_CarControllerV3 RCC)
	{
		if (allVehicles.Contains(RCC))
		{
			allVehicles.Remove(RCC);
		}
	}

	private void RCC_AICarController_OnRCCAIDestroyed(RCC_AICarController RCCAI)
	{
		if (allVehicles.Contains(RCCAI.carController))
		{
			allVehicles.Remove(RCCAI.carController);
		}
	}

	private void Update()
	{
		if (disableUIWhenNoPlayerVehicle && (bool)activePlayerCanvas)
		{
			CheckCanvas();
		}
		if (Input.GetKeyDown(RCC_Settings.Instance.recordKB))
		{
			recorder.Record();
		}
		if (Input.GetKeyDown(RCC_Settings.Instance.playbackKB))
		{
			recorder.Play();
		}
		if (Input.GetKey(RCC_Settings.Instance.slowMotionKB))
		{
			Time.timeScale = 0.2f;
		}
		if (Input.GetKeyUp(RCC_Settings.Instance.slowMotionKB))
		{
			Time.timeScale = orgTimeScale;
		}
		activeMainCamera = Camera.current;
		switch (recorder.mode)
		{
		case RCC_Recorder.Mode.Neutral:
			recordMode = RecordMode.Neutral;
			break;
		case RCC_Recorder.Mode.Play:
			recordMode = RecordMode.Play;
			break;
		case RCC_Recorder.Mode.Record:
			recordMode = RecordMode.Record;
			break;
		}
	}

	public void RegisterPlayer(RCC_CarControllerV3 playerVehicle)
	{
		activePlayerVehicle = playerVehicle;
		if ((bool)activePlayerCamera)
		{
			activePlayerCamera.SetTarget(activePlayerVehicle.gameObject);
		}
		if ((bool)Object.FindObjectOfType<RCC_CustomizerExample>())
		{
			Object.FindObjectOfType<RCC_CustomizerExample>().CheckUIs();
		}
	}

	public void RegisterPlayer(RCC_CarControllerV3 playerVehicle, bool isControllable)
	{
		activePlayerVehicle = playerVehicle;
		activePlayerVehicle.SetCanControl(isControllable);
		if ((bool)activePlayerCamera)
		{
			activePlayerCamera.SetTarget(activePlayerVehicle.gameObject);
		}
		if ((bool)Object.FindObjectOfType<RCC_CustomizerExample>())
		{
			Object.FindObjectOfType<RCC_CustomizerExample>().CheckUIs();
		}
	}

	public void RegisterPlayer(RCC_CarControllerV3 playerVehicle, bool isControllable, bool engineState)
	{
		activePlayerVehicle = playerVehicle;
		activePlayerVehicle.SetCanControl(isControllable);
		activePlayerVehicle.SetEngine(engineState);
		if ((bool)activePlayerCamera)
		{
			activePlayerCamera.SetTarget(activePlayerVehicle.gameObject);
		}
		if ((bool)Object.FindObjectOfType<RCC_CustomizerExample>())
		{
			Object.FindObjectOfType<RCC_CustomizerExample>().CheckUIs();
		}
	}

	public void DeRegisterPlayer()
	{
		if ((bool)activePlayerVehicle)
		{
			activePlayerVehicle.SetCanControl(false);
		}
		activePlayerVehicle = null;
		if ((bool)activePlayerCamera)
		{
			activePlayerCamera.RemoveTarget();
		}
	}

	public void CheckCanvas()
	{
		if (!activePlayerVehicle || !activePlayerVehicle.gameObject.activeInHierarchy || !activePlayerVehicle.enabled)
		{
			activePlayerCanvas.SetDisplayType(RCC_UIDashboardDisplay.DisplayType.Off);
		}
		else if (activePlayerCanvas.displayType != RCC_UIDashboardDisplay.DisplayType.Customization)
		{
			activePlayerCanvas.displayType = RCC_UIDashboardDisplay.DisplayType.Full;
		}
	}

	private void OnDisable()
	{
		RCC_Camera.OnBCGCameraSpawned -= RCC_Camera_OnBCGCameraSpawned;
		RCC_CarControllerV3.OnRCCPlayerSpawned -= RCC_CarControllerV3_OnRCCSpawned;
		RCC_AICarController.OnRCCAISpawned -= RCC_AICarController_OnRCCAISpawned;
		RCC_CarControllerV3.OnRCCPlayerDestroyed -= RCC_CarControllerV3_OnRCCPlayerDestroyed;
		RCC_AICarController.OnRCCAIDestroyed -= RCC_AICarController_OnRCCAIDestroyed;
	}
}
