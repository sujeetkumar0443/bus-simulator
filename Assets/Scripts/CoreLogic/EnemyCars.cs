using UnityEngine;

public class EnemyCars : MonoBehaviour
{
	public int EnemyHealth = 5;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			EnemyHealth--;
			if (EnemyHealth <= 0)
			{
				base.gameObject.transform.Find("Fire").gameObject.SetActive(true);
			}
		}
	}
}
