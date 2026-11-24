using UnityEngine;

public class PlayerSignalDetector : MonoBehaviour
{
	private RaycastHit hit;

	public float distance;

	public float distance1;

	private bool detectedSignal;

	private bool detectedSignal1;

	private void Start()
	{
	}

	private void Update()
	{
		if (Physics.Raycast(base.transform.position, base.transform.forward, out hit, distance))
		{
			if (hit.collider.tag == "sigstop" && !detectedSignal)
			{
				detectedSignal = true;
				Debug.Log("stop now");
				if (hit.collider.gameObject.transform.parent.transform.GetChild(0).gameObject.activeSelf)
				{
					Debug.Log("ruk jao");
				}
				else if (hit.collider.gameObject.transform.parent.transform.GetChild(1).gameObject.activeSelf)
				{
					Debug.Log("ready to go");
				}
				else
				{
					Debug.Log("gooooooooo");
				}
			}
		}
		else
		{
			Debug.Log("chalta reh");
			detectedSignal = false;
		}
		if (Physics.Raycast(base.transform.position, base.transform.forward, out hit, distance1))
		{
			if (hit.collider.tag == "sigstop" && !detectedSignal1)
			{
				detectedSignal1 = true;
				if (hit.collider.gameObject.transform.parent.transform.GetChild(0).gameObject.activeSelf)
				{
					Debug.Log("gameover");
				}
				else if (hit.collider.gameObject.transform.parent.transform.GetChild(1).gameObject.activeSelf)
				{
					Debug.Log("gameover not");
				}
				else
				{
					Debug.Log("gameover not");
				}
			}
		}
		else if (!detectedSignal)
		{
			Debug.Log("chalta reh");
			detectedSignal1 = false;
		}
	}
}
