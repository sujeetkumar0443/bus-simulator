using UnityEngine;

public class TrafficControler : MonoBehaviour
{
	private GameObject instance;

	private AudioSource[] audioSource;

	private void Start()
	{
		instance = base.gameObject;
		Invoke("ActiveNow", 2f);
		if (PlayerPrefs.GetInt("sounds") != 1)
		{
		}
	}

	private void ActiveNow()
	{
		audioSource = instance.GetComponents<AudioSource>();
		SetValues(false);
	}

	private void SetValues(bool isActive)
	{
		if (isActive)
		{
			instance.GetComponent<Rigidbody>().isKinematic = false;
			for (int i = 0; i < audioSource.Length; i++)
			{
				audioSource[i].enabled = true;
			}
		}
		else
		{
			instance.GetComponent<Rigidbody>().isKinematic = true;
			for (int j = 0; j < audioSource.Length; j++)
			{
				audioSource[j].enabled = false;
			}
		}
	}

	private void OnTriggerEnter(Collider coll)
	{
		if (coll.gameObject.tag == "Player")
		{
			SetValues(true);
		}
	}

	private void OnTriggerExit(Collider coll)
	{
		if (coll.gameObject.tag == "Player")
		{
			SetValues(false);
		}
	}
}
