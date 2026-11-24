using UnityEngine;

namespace EVP
{
	public class VehicleTelemetry : MonoBehaviour
	{
		public enum DataMode
		{
			TireSlipAndForce = 0,
			GroundMaterial = 1
		}

		public VehicleController target;

		public DataMode dataMode;

		public KeyCode toggleKey = KeyCode.Y;

		public bool show = true;

		public bool gizmos;

		public GUIStyle style = new GUIStyle();

		[HideInInspector]
		public Font defaultFont;

		private string m_telemetryText = string.Empty;

		private void OnEnable()
		{
			if (style.font == null)
			{
				style.font = defaultFont;
				style.normal.textColor = Color.white;
			}
			if (target == null)
			{
				target = GetComponent<VehicleController>();
			}
		}

		private void FixedUpdate()
		{
			if (target != null && show)
			{
				m_telemetryText = DoTelemetry();
			}
		}

		private void Update()
		{
			if (target != null && gizmos)
			{
				DrawGizmos();
			}
			if (!Input.GetKeyDown(toggleKey))
			{
				return;
			}
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				dataMode++;
				if (dataMode > DataMode.GroundMaterial)
				{
					dataMode = DataMode.TireSlipAndForce;
				}
			}
			else
			{
				show = !show;
			}
		}

		private void OnGUI()
		{
			if (target != null && show)
			{
				GUI.Box(new Rect(8f, 8f, 545f, 180f), "Telemetry");
				GUI.Label(new Rect(16f, 28f, 400f, 270f), m_telemetryText, style);
			}
		}

		private string DoTelemetry()
		{
			string text = string.Format("V: {0,5:0.0} m/s  {1,5:0.0} km/h {2,5:0.0} mph\n\n", target.speed, target.speed * 3.6f, target.speed * 2.237f);
			float suspensionForce = 0f;
			WheelData[] wheelData = target.wheelData;
			foreach (WheelData wd in wheelData)
			{
				text += GetWheelTelemetry(wd, ref suspensionForce);
			}
			text += string.Format("\n     ΣF:{0,6:0.}  Perceived mass:{1,7:0.0}\n               Rigidbody mass:{2,7:0.0}\n", suspensionForce, (0f - suspensionForce) / Physics.gravity.y, target.cachedRigidbody.mass);
			VehicleAudio component = target.GetComponent<VehicleAudio>();
			if (component != null)
			{
				text += string.Format("\nAudio gear/rpm:{0,2:0.} {1,5:0.}", component.simulatedGear, component.simulatedEngineRpm);
			}
			VehicleDamage component2 = target.GetComponent<VehicleDamage>();
			if (component2 != null)
			{
				text += string.Format("\nDamage mesh/collider/node:{0,5:0.00} {1,4:0.00} {2,4:0.00}  {3}", component2.meshDamage, component2.colliderDamage, component2.nodeDamage, (!component2.isRepairing) ? string.Empty : "REPAIRING");
			}
			if (target.debugText != string.Empty)
			{
				text = text + "\n\n" + target.debugText;
			}
			return text;
		}

		private string GetWheelTelemetry(WheelData wd, ref float suspensionForce)
		{
			bool flag = !(wd.collider.motorTorque > 0f);
			string text = string.Format("{0,-10}{1}{2,5:0.} rpm  ", wd.collider.gameObject.name, (!flag) ? ":" : "×", wd.angularVelocity * VehicleController.WToRpm);
			if (wd.grounded)
			{
				text += string.Format("C:{0,5:0.00}  ", wd.suspensionCompression);
				switch (dataMode)
				{
				case DataMode.TireSlipAndForce:
					text += string.Format("F:{0,5:0.}  ", wd.downforce);
					text += string.Format("Sx:{0,6:0.00} Sy:{1,6:0.00} ", wd.tireSlip.x, wd.tireSlip.y);
					text += string.Format("Fx:{0,5:0.} Fy:{1,5:0.}  ", wd.tireForce.x, wd.tireForce.y);
					break;
				case DataMode.GroundMaterial:
					text += string.Format("F:{0,4:0.0} %  ", wd.downforceRatio);
					text += string.Format("Slip:{0,4:0.0}  ", wd.combinedTireSlip);
					if (wd.groundMaterial != null)
					{
						text += string.Format("Grip:{0,4:0.0} Drag:{1,4:0.0}  [{2}]", wd.groundMaterial.grip, wd.groundMaterial.drag, (!(wd.groundMaterial.physicMaterial != null)) ? "no mat" : wd.groundMaterial.physicMaterial.name);
					}
					break;
				}
				suspensionForce += wd.hit.force;
			}
			else
			{
				text += string.Format("C: 0.--  ");
			}
			return text + "\n";
		}

		private void DrawGizmos()
		{
			CommonTools.DrawCrossMark(target.cachedTransform.TransformPoint(target.cachedRigidbody.centerOfMass), target.cachedTransform, Color.white);
			WheelData[] wheelData = target.wheelData;
			foreach (WheelData wd in wheelData)
			{
				DrawWheelGizmos(wd);
			}
			Vector3 pos = target.cachedTransform.TransformPoint(target.cachedRigidbody.centerOfMass + Vector3.forward * target.aeroAppPointOffset);
			CommonTools.DrawCrossMark(pos, target.cachedTransform, Color.cyan);
		}

		private void DrawWheelGizmos(WheelData wd)
		{
			RaycastHit hitInfo;
			if (wd.grounded && Physics.Raycast(wd.transform.TransformPoint(wd.collider.center), -wd.transform.up, out hitInfo, wd.collider.suspensionDistance + wd.collider.radius))
			{
				Debug.DrawLine(hitInfo.point, hitInfo.point + wd.transform.up * (wd.downforce / 10000f), (!(wd.suspensionCompression > 0.99f)) ? Color.white : Color.magenta);
				CommonTools.DrawCrossMark(wd.transform.position, wd.transform, Color.Lerp(Color.green, Color.gray, 0.5f));
				Vector3 vector = hitInfo.point + wd.transform.up * target.antiRoll * wd.forceDistance;
				if (wd.wheel.steer && wd.steerAngle != 0f && Mathf.Sign(wd.steerAngle) != Mathf.Sign(wd.tireSlip.x))
				{
					vector += wd.hit.forwardDir * target.steeringOverdrive;
				}
				CommonTools.DrawCrossMark(vector, wd.transform, Color.Lerp(Color.yellow, Color.gray, 0.5f));
				Vector3 val = wd.hit.forwardDir * wd.tireForce.y + wd.hit.sidewaysDir * wd.tireForce.x;
				Debug.DrawLine(vector, vector + CommonTools.Lin2Log(val) * 0.1f, Color.green);
				Vector3 val2 = wd.hit.forwardDir * wd.tireSlip.y + wd.hit.sidewaysDir * wd.tireSlip.x;
				Debug.DrawLine(hitInfo.point, hitInfo.point + CommonTools.Lin2Log(val2) * 0.5f, Color.cyan);
			}
		}
	}
}
