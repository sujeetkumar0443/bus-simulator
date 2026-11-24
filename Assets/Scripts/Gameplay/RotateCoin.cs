using UnityEngine;

namespace BusSimulator
{
	public class RotateCoin : MonoBehaviour
	{
		public float rotationvalue;

		private void Start()
		{
		}

		private void Update()
		{
			base.transform.Rotate(new Vector3(0f, 0f, rotationvalue * Time.timeScale));
		}
	}
}
