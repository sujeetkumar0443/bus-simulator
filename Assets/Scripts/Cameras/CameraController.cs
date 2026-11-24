using UnityEngine;

public class CameraController : MonoBehaviour
{
	public SmoothFollowCamera smooth;

	private int cameraIndex;

	private GameObject cameraPositions;

	public RCC_Camera rccCam;

	private bool rotateCamera;

	private int lastActiveCamMode;

	private void Start()
	{
	}

	public void SetDefaultCam(GameObject obj)
	{
		cameraPositions = obj.transform.Find("campos").gameObject;
	}

	private void RotateCamAround()
	{
		while (true)
		{
			cameraPositions.transform.GetChild(0).transform.RotateAround(cameraPositions.transform.GetChild(0).transform.position, Vector3.up, 35f * Time.deltaTime);
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
		cameraPositions.transform.GetChild(0).gameObject.SetActive(false);
	}

	public void SetDoorCam()
	{
		rccCam.gameObject.SetActive(false);
		cameraPositions.transform.GetChild(0).gameObject.SetActive(true);
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
		if (cameraIndex < cameraPositions.transform.childCount - 1)
		{
			cameraIndex++;
		}
		else
		{
			cameraIndex = 0;
		}
		if (cameraIndex == 1)
		{
			smooth.gameObject.SetActive(false);
			cameraPositions.transform.GetChild(cameraIndex).gameObject.SetActive(true);
			return;
		}
		smooth.distance = 7f;
		smooth.height = 3f;
		smooth.gameObject.SetActive(true);
		cameraPositions.transform.GetChild(1).gameObject.SetActive(false);
		smooth.target = cameraPositions.transform.GetChild(cameraIndex).transform;
	}
}
