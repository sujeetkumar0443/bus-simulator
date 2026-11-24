using UnityEngine;

namespace EVP
{
	public class VehicleStandardInput : MonoBehaviour
	{
		public VehicleController target;

		public bool continuousForwardAndReverse = true;

		public string steerAxis = "Horizontal";

		public string throttleAndBrakeAxis = "Vertical";

		public string handbrakeAxis = "Jump";

		public KeyCode resetVehicleKey = KeyCode.Return;

		private bool m_doReset;

		public float steerInput;

		public float forwardInput;

		public float reverseInput;

		public float handbrakeInput;

		public SteeringWheel steerWheel;

		private void OnEnable()
		{
			if (target == null)
			{
				target = GetComponent<VehicleController>();
			}
		}

		private void Update()
		{
			if (!(target == null))
			{
				if (GameState.tiltcontrol)
				{
					steerInput = Input.acceleration.x * (float)PlayerPrefs.GetInt("transmission");
				}
				else if (GameState.steercontrol)
				{
					steerInput = steerWheel.GetClampedValue();
				}
				if (Input.GetKeyDown(resetVehicleKey))
				{
					m_doReset = true;
				}
			}
		}

		private void FixedUpdate()
		{
			if (target == null)
			{
				return;
			}
			if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
			{
				if (!GameState.tiltcontrol)
				{
					steerInput = Mathf.Clamp(Input.GetAxis(steerAxis), -1f, 1f);
				}
				else
				{
					Debug.Log("tilt controls");
				}
			}
			float throttleInput = 0f;
			float brakeInput = 0f;
			if (continuousForwardAndReverse)
			{
				float num = 0.1f;
				float num2 = 0.1f;
				if (target.speed > num)
				{
					throttleInput = forwardInput;
					brakeInput = reverseInput;
				}
				else if (reverseInput > num2)
				{
					throttleInput = 0f - reverseInput;
					brakeInput = 0f;
				}
				else if (forwardInput > num2)
				{
					if (target.speed < 0f - num)
					{
						throttleInput = 0f;
						brakeInput = forwardInput;
					}
					else
					{
						throttleInput = forwardInput;
						brakeInput = 0f;
					}
				}
			}
			else if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
			{
				throttleInput = forwardInput;
				brakeInput = reverseInput;
			}
			else
			{
				throttleInput = 0f - reverseInput;
				brakeInput = 0f;
			}
			target.steerInput = steerInput;
			target.throttleInput = throttleInput;
			target.brakeInput = brakeInput;
			target.handbrakeInput = handbrakeInput;
			if (m_doReset)
			{
				target.ResetVehicle();
				m_doReset = false;
			}
		}
	}
}
