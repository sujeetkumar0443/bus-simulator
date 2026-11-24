using UnityEngine;

namespace BusSimulator
{
	public class SmoothFollowCamera : MonoBehaviour
	{
		public Transform target;

		public float distance = 15f;

		public float height = 5f;

		public float heightDamping = 3f;

		public float rotationDamping = 3f;

		public bool cameraSwitch;

		private GameObject vehicle;

		public Transform[] cameraSwitchView;

		private int Switch;

		private void Start()
		{
		}

		public void CamBtn()
		{
			cameraSwitch = true;
		}

		private void Update()
		{
			if (!target)
			{
				return;
			}
			if (cameraSwitch)
			{
				cameraSwitch = false;
				Switch++;
				if (Switch > cameraSwitchView.Length - 1)
				{
					Switch = 0;
					target = cameraSwitchView[Switch].transform;
				}
				else
				{
					target = cameraSwitchView[Switch].transform;
				}
			}
			float y = target.eulerAngles.y;
			float b = target.position.y + height;
			float y2 = base.transform.eulerAngles.y;
			float y3 = base.transform.position.y;
			y2 = Mathf.LerpAngle(y2, y, rotationDamping * Time.deltaTime);
			y3 = Mathf.Lerp(y3, b, heightDamping * Time.deltaTime);
			Quaternion quaternion = Quaternion.Euler(0f, y2, 0f);
			Vector3 position = target.position;
			position -= quaternion * Vector3.forward * distance;
			position.y = y3;
			base.transform.position = position;
			base.transform.LookAt(target);
		}
	}
}
