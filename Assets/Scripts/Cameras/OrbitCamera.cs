using UnityEngine;

namespace BusSimulator
{
	public class OrbitCamera : MonoBehaviour
	{
		public Transform _target;

		public float _distance = 20f;

		public float _zoomStep = 1f;

		public float _xSpeed = 1f;

		public float _ySpeed = 1f;

		private float _x;

		private float _y;

		private Vector3 _distanceVector;

		private void Start()
		{
			_distanceVector = new Vector3(0f, 0f, 0f - _distance);
			Vector2 vector = base.transform.localEulerAngles;
			_x = vector.x;
			_y = vector.y;
			Rotate(_x, _y);
		}

		private void LateUpdate()
		{
			if ((bool)_target)
			{
				RotateControls();
				Zoom();
			}
		}

		private void RotateControls()
		{
			if (Input.GetButton("Fire1"))
			{
				_x += Input.GetAxis("Mouse X") * _xSpeed;
				_y += (0f - Input.GetAxis("Mouse Y")) * _ySpeed;
				Rotate(_x, _y);
			}
		}

		private void Rotate(float x, float y)
		{
			Quaternion quaternion = Quaternion.Euler(y, x, 0f);
			Vector3 position = quaternion * _distanceVector + _target.position;
			base.transform.rotation = quaternion;
			base.transform.position = position;
		}

		private void Zoom()
		{
			if (Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				ZoomOut();
			}
			else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				ZoomIn();
			}
		}

		private void ZoomIn()
		{
			_distance -= _zoomStep;
			_distanceVector = new Vector3(0f, 0f, 0f - _distance);
			Rotate(_x, _y);
		}

		private void ZoomOut()
		{
			_distance += _zoomStep;
			_distanceVector = new Vector3(0f, 0f, 0f - _distance);
			Rotate(_x, _y);
		}
	}
}
