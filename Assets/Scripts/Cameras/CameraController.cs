using UnityEngine;

public class CameraController : MonoBehaviour
{
	public SmoothFollowCamera smooth;

	private int camnum;

	private GameObject campositions;

	public RCC_Camera rccCam;

	private bool rotatecam;

	private int lastActiveCamMode;

	private void Start()
	{
	}

	public void SetDefaultCam(GameObject obj)
	{
		campositions = obj.transform.Find("campos").gameObject;
	}

	private void RotateCamAround()
	{
		while (true)
		{
			campositions.transform.GetChild(0).transform.RotateAround(campositions.transform.GetChild(0).transform.position, Vector3.up, 35f * Time.deltaTime);
		}
	}

	public void CamPosForPassengers(GameObject obj)
	{
		smooth.target = obj.transform;
	}

	public void RotateCam(GameObject obj)
	{
	}

	public void ResetCamPosition()
	{
		rccCam.gameObject.SetActive(true);
		rccCam.ChangeCamera(rccCam.lastCameraMode);
		campositions.transform.GetChild(0).gameObject.SetActive(false);
	}

	public void SetDoorCam()
	{
		rccCam.gameObject.SetActive(false);
		campositions.transform.GetChild(0).gameObject.SetActive(true);
	}

	public void cambtnpressed(GameObject obj)
	{
		obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
	}

	public void cambtnUnpressed(GameObject obj)
	{
		obj.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public void CamBtnClick()
	{
		if (camnum < campositions.transform.childCount - 1)
		{
			camnum++;
		}
		else
		{
			camnum = 0;
		}
		if (camnum == 1)
		{
			smooth.gameObject.SetActive(false);
			campositions.transform.GetChild(camnum).gameObject.SetActive(true);
			return;
		}
		smooth.distance = 7f;
		smooth.height = 3f;
		smooth.gameObject.SetActive(true);
		campositions.transform.GetChild(1).gameObject.SetActive(false);
		smooth.target = campositions.transform.GetChild(camnum).transform;
	}
}
