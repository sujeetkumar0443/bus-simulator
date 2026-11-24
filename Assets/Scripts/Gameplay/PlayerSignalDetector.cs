using UnityEngine;

public class PlayerSignalDetector : MonoBehaviour
{
	private RaycastHit hit;

	public float distance;

	public float distance1;

	private bool detctSignal;

	private bool detctSignal1;

	private void Start()
	{
	}

	private void Update()
	{
		if (Physics.Raycast(base.transform.position, base.transform.forward, out hit, distance))
		{
			if (hit.collider.tag == "sigstop" && !detctSignal)
			{
				detctSignal = true;
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
			detctSignal = false;
		}
		if (Physics.Raycast(base.transform.position, base.transform.forward, out hit, distance1))
		{
			if (hit.collider.tag == "sigstop" && !detctSignal1)
			{
				detctSignal1 = true;
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
		else if (!detctSignal)
		{
			Debug.Log("chalta reh");
			detctSignal1 = false;
		}
	}
}
