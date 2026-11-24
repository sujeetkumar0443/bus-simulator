using UnityEngine;
using UnityEngine.UI;

public class PassengerWalker : MonoBehaviour
{
	private GameObject temp;

	private bool done;

	public GameObject dollar;

	public Text cashtext;

	private RaycastHit hit;

	private void Start()
	{
	}

	private void Update()
	{
		temp = GameObject.FindGameObjectWithTag("door");
		base.gameObject.GetComponent<Animation>().Play("Move_Walk225");
		base.transform.position = Vector3.MoveTowards(base.transform.position, temp.transform.position, 0.06f);
		Debug.DrawRay(new Vector3(base.transform.position.x, base.transform.position.y + 1.5f, base.transform.position.z), base.transform.forward);
		if (Physics.Raycast(new Vector3(base.transform.position.x, base.transform.position.y + 1.5f, base.transform.position.z), base.transform.forward, out hit, 1f) && hit.transform.tag == "Player")
		{
			base.gameObject.SetActive(false);
			ParkingChecker.passengerloadcount++;
			if (!dollar.GetComponent<Animation>().isPlaying)
			{
				dollar.GetComponent<Animation>().Play();
			}
			PlayerPrefs.SetInt("cash", PlayerPrefs.GetInt("cash") + 1000);
			cashtext.text = PlayerPrefs.GetInt("cash").ToString();
		}
	}
}
