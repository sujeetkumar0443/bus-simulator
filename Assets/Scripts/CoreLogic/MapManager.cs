using UnityEngine;

public class MapManager : MonoBehaviour
{
	public GameObject mapCamera;

	public GameObject MainCamera;

	public GameObject MiniMapCanvas;

	public GameObject CancelBtn;

	public GameObject mainCanvas;

	public GameObject mapHint;

	public void zoom()
	{
		mapCamera.SetActive(true);
		MainCamera.SetActive(false);
		MiniMapCanvas.SetActive(false);
		mainCanvas.SetActive(false);
		Time.timeScale = 0f;
		CancelBtn.SetActive(true);
		mapHint.SetActive(true);
	}

	public void Cancle()
	{
		Time.timeScale = 1f;
		mapCamera.SetActive(false);
		MainCamera.SetActive(true);
		MiniMapCanvas.SetActive(true);
		mapHint.SetActive(false);
		CancelBtn.SetActive(false);
		mainCanvas.SetActive(true);
	}
}
