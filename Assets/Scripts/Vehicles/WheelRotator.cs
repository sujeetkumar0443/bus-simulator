using UnityEngine;

namespace BusSimulator
{
	public class WheelRotator : MonoBehaviour
	{
		public float speed = 50f;

		private void Start()
		{
		}

		private void Update()
		{
			base.transform.Rotate(speed, 0f, 0f);
		}
	}
}
