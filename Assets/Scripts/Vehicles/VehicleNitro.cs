using UnityEngine;

namespace BusSimulator
{
	public class VehicleNitro : MonoBehaviour
	{
		public enum Mode
		{
			Acceleration = 0,
			Impulse = 1
		}

		public Mode mode;

		public float value = 10f;

		public float maxVelocity = 50f;

		public KeyCode key = KeyCode.N;

		private Rigidbody m_rigidbody;

		private void OnEnable()
		{
			m_rigidbody = GetComponent<Rigidbody>();
		}

		private void Update()
		{
			if (mode == Mode.Impulse && Input.GetKeyDown(key) && m_rigidbody.velocity.magnitude < maxVelocity)
			{
				m_rigidbody.AddRelativeForce(Vector3.forward * value, ForceMode.VelocityChange);
			}
		}

		private void FixedUpdate()
		{
			if (mode == Mode.Acceleration && Input.GetKey(key) && m_rigidbody.velocity.magnitude < maxVelocity)
			{
				m_rigidbody.AddRelativeForce(Vector3.forward * value, ForceMode.Acceleration);
			}
		}
	}
}
