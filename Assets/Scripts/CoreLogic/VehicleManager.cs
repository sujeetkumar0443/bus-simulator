using UnityEngine;

public class VehicleManager : MonoBehaviour
{
	private float rand;

	public GameObject vehiclePos;

	public GameObject levelmanager;

	public CameraController campos;

	public GameObject selectedVehicle;

	public GameControl controls;

	public GameObject buspos;

	private int vehiclenum;

	public static int vNo;

	public bl_MiniMap miniMap;

	public GameObject levels;

	private GameObject busPosition;

	public static VehicleManager vehicleManager;

	private GameObject selectedVehicleAudio;

	private void Start()
	{
		vehicleManager = this;
		vehiclenum = PlayerPrefs.GetInt("vehicle");
		selectedVehicle = base.transform.GetChild(PlayerPrefs.GetInt("vehicle")).gameObject;
		if (!ModeSelection.FreeMoode)
		{
			busPosition = levels.transform.GetChild(LevelManager.LevelNumber - 1).transform.GetChild(0).gameObject;
			vNo = PlayerPrefs.GetInt("vehicle");
			base.transform.GetChild(PlayerPrefs.GetInt("vehicle")).gameObject.transform.position = busPosition.transform.position;
			base.transform.GetChild(PlayerPrefs.GetInt("vehicle")).gameObject.transform.rotation = busPosition.transform.rotation;
		}
		else
		{
			busPosition = levels.transform.GetChild(0).transform.GetChild(0).gameObject;
			base.transform.GetChild(PlayerPrefs.GetInt("vehicle")).gameObject.transform.position = busPosition.transform.position;
			base.transform.GetChild(PlayerPrefs.GetInt("vehicle")).gameObject.transform.rotation = busPosition.transform.rotation;
		}
		selectedVehicle.SetActive(true);
		selectedVehicle.tag = "Player";
		selectedVehicle.SetActive(true);
		campos.SetDefaultCam(selectedVehicle);
		miniMap.m_Target = selectedVehicle;
		if (PlayerPrefs.GetInt("sounds") != 1)
		{
		}
	}

	public void NextVehicle()
	{
		if (vehiclenum < base.transform.childCount - 1)
		{
			vehiclenum++;
		}
		else
		{
			vehiclenum = 0;
		}
		selectedVehicle = base.transform.GetChild(vehiclenum).gameObject;
		selectedVehicle.SetActive(true);
		selectedVehicle.tag = "Player";
		campos.SetDefaultCam(selectedVehicle);
		controls.setDefaultControls(selectedVehicle);
	}

	public void StopVehicle(bool isStop)
	{
		selectedVehicle.GetComponent<Rigidbody>().isKinematic = isStop;
	}

	public void PauseVehicleSounds(bool isOn)
	{
	}

	private void OnDisable()
	{
		Object.Destroy(vehicleManager);
	}
}
