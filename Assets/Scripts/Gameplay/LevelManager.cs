using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
	public static int LevelNumber;

	public static int LevelTotalCoin;

	public GameObject objectivedialoge;

	public GameObject Civilians;

	private int rand;

	public GameObject parkingpos;

	public static int passenger;

	public Text cashtext;

	public Sprite redpoint;

	public static GameObject lastparking;

	public static int TotalsParking;

	public PathFinding pathTarget;

	private IEnumerator objectivedisappear()
	{
		yield return new WaitForSeconds(2f);
	}

	private void Awake()
	{
		LevelNumber = PlayerPrefs.GetInt("levelno");
	}

	private void Start()
	{
		lastparking = null;
		passenger = 0;
		cashtext.text = PlayerPrefs.GetInt("cash").ToString();
		if (!ModeSelectionController.FreeMoode)
		{
			objectivedialoge.SetActive(true);
			StartCoroutine(objectivedisappear());
			base.transform.GetChild(LevelNumber - 1).gameObject.SetActive(true);
			TotalsParking = 2;
		}
		else
		{
			objectivedialoge.SetActive(true);
			objectivedialoge.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = "Enjoy the free ride and improve your driving skills.";
			StartCoroutine(objectivedisappear());
		}
	}
}
