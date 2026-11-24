using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace EVP
{
	[RequireComponent(typeof(Rigidbody))]
	public class VehicleController : MonoBehaviour
	{
		public enum BrakeMode
		{
			Slip = 0,
			Ratio = 1
		}

		public enum UpdateRate
		{
			OnUpdate = 0,
			OnFixedUpdate = 1,
			Disabled = 2
		}

		public enum PositionMode
		{
			Accurate = 0,
			Fast = 1
		}

		public delegate void OnImpact();

		public Wheel[] wheels = new Wheel[0];

		[Header("Handling & behavior")]
		public Transform centerOfMass;

		[Range(0f, 2f)]
		public float tireFriction = 1f;

		[Range(0f, 1f)]
		public float antiRoll = 0.2f;

		[Range(0f, 89f)]
		public float maxSteerAngle = 35f;

		public float aeroDrag;

		public float aeroDownforce = 1f;

		public float aeroAppPointOffset = -0.5f;

		[FormerlySerializedAs("maxSpeed")]
		[Header("Motor")]
		public float maxSpeedForward = 27.78f;

		public float maxSpeedReverse = 12f;

		public float maxDriveForce = 2000f;

		[Range(0.0001f, 0.9999f)]
		public float forceCurveShape = 0.5f;

		public float maxDriveSlip = 4f;

		public float driveForceToMaxSlip = 1000f;

		[Header("Brakes")]
		public float maxBrakeForce = 3000f;

		public float brakeForceToMaxSlip = 1000f;

		public BrakeMode brakeMode;

		public float maxBrakeSlip = 2f;

		[Range(0f, 1f)]
		public float maxBrakeRatio = 0.5f;

		public BrakeMode handbrakeMode;

		public float maxHandbrakeSlip = 10f;

		[Range(0f, 1f)]
		public float maxHandbrakeRatio = 1f;

		[Header("Driving aids")]
		public bool tcEnabled;

		[Range(0f, 1f)]
		public float tcRatio = 1f;

		public bool absEnabled;

		[Range(0f, 1f)]
		public float absRatio = 1f;

		public bool espEnabled;

		[Range(0f, 1f)]
		public float espRatio = 0.5f;

		[Range(-1f, 1f)]
		[Header("Vehicle controls")]
		public float steerInput;

		[Range(-1f, 1f)]
		public float throttleInput;

		[Range(0f, 1f)]
		public float brakeInput;

		[Range(0f, 1f)]
		public float handbrakeInput;

		[Header("Visual wheels")]
		public UpdateRate spinUpdateRate;

		public PositionMode wheelPositionMode;

		[Header("Optimization & Debug")]
		public bool disallowRuntimeChanges;

		public bool disableWheelHitCorrection;

		public bool disableSteerAngleCorrection;

		public bool showContactGizmos;

		[Range(0f, 0.5f)]
		[Header("Experimental")]
		public float sleepVelocity = 0.1f;

		[Range(0f, 1f)]
		public float steeringOverdrive;

		public float defaultGroundGrip = 1f;

		public float defaultGroundDrag;

		[NonSerialized]
		public bool processContacts;

		[NonSerialized]
		public float impactThreeshold = 0.6f;

		[NonSerialized]
		public float impactInterval = 0.2f;

		[NonSerialized]
		public float impactIntervalRandom = 0.4f;

		[NonSerialized]
		public float impactMinSpeed = 2f;

		[NonSerialized]
		public bool computeExtendedTireData;

		public OnImpact onImpact;

		public static VehicleController current;

		public static float RpmToW = (float)Math.PI / 30f;

		public static float WToRpm = 30f / (float)Math.PI;

		private Transform m_transform;

		private Rigidbody m_rigidbody;

		private GroundMaterialManager m_groundMaterialManager;

		[NonSerialized]
		public string debugText = string.Empty;

		private WheelData[] m_wheelData = new WheelData[0];

		private float m_speed;

		private float m_speedAngle;

		private bool m_usesHandbrake;

		private CommonTools.BiasLerpContext m_forceBiasCtx = new CommonTools.BiasLerpContext();

		private Collider[] m_colliders = new Collider[0];

		private int[] m_colLayers = new int[0];

		private WheelFrictionCurve m_colliderFriction;

		private int m_sumImpactCount;

		private Vector3 m_sumImpactPosition = Vector3.zero;

		private Vector3 m_sumImpactVelocity = Vector3.zero;

		private int m_sumImpactHardness;

		private float m_lastImpactTime;

		private Vector3 m_localDragPosition = Vector3.zero;

		private Vector3 m_localDragVelocity = Vector3.zero;

		private int m_localDragHardness;

		private float m_lastStrongImpactTime;

		private PhysicMaterial m_lastImpactedMaterial;

		private GroundMaterial m_impactedGroundMaterial;

		public Vector3 localImpactPosition
		{
			get
			{
				return m_sumImpactPosition;
			}
		}

		public Vector3 localImpactVelocity
		{
			get
			{
				return m_sumImpactVelocity;
			}
		}

		public bool isHardImpact
		{
			get
			{
				return m_sumImpactHardness >= 0;
			}
		}

		public Vector3 localDragPosition
		{
			get
			{
				return m_localDragPosition;
			}
		}

		public Vector3 localDragVelocity
		{
			get
			{
				return m_localDragVelocity;
			}
		}

		public bool isHardDrag
		{
			get
			{
				return m_localDragHardness >= 0;
			}
		}

		public WheelData[] wheelData
		{
			get
			{
				return m_wheelData;
			}
		}

		public float speed
		{
			get
			{
				return m_speed;
			}
		}

		public float speedAngle
		{
			get
			{
				return m_speedAngle;
			}
		}

		public Transform cachedTransform
		{
			get
			{
				return m_transform;
			}
		}

		public Rigidbody cachedRigidbody
		{
			get
			{
				return m_rigidbody;
			}
		}

		private void OnValidate()
		{
			maxDriveSlip = Mathf.Max(maxDriveSlip, 0f);
			maxBrakeSlip = Mathf.Max(maxBrakeSlip, 0f);
			maxHandbrakeSlip = Mathf.Max(maxHandbrakeSlip, 0f);
			maxDriveForce = Mathf.Max(maxDriveForce, 0f);
			maxBrakeForce = Mathf.Max(maxBrakeForce, 0f);
			driveForceToMaxSlip = Mathf.Max(driveForceToMaxSlip, 1f);
			brakeForceToMaxSlip = Mathf.Max(brakeForceToMaxSlip, 1f);
			maxSpeedForward = Mathf.Max(maxSpeedForward, 0f);
			maxSpeedReverse = Mathf.Max(maxSpeedReverse, 0f);
		}

		private void OnEnable()
		{
			m_transform = GetComponent<Transform>();
			m_rigidbody = GetComponent<Rigidbody>();
			m_groundMaterialManager = UnityEngine.Object.FindObjectOfType<GroundMaterialManager>();
			FindColliders();
			m_rigidbody.maxAngularVelocity = 14f;
			m_rigidbody.maxDepenetrationVelocity = 8f;
			if ((bool)centerOfMass)
			{
				m_rigidbody.centerOfMass = m_transform.InverseTransformPoint(centerOfMass.position);
			}
			if (wheels.Length == 0)
			{
				Debug.LogWarning("The wheels property is empty. You must configure wheels and WheelColliders first. Component is disabled.");
				base.enabled = false;
				return;
			}
			m_usesHandbrake = false;
			m_wheelData = new WheelData[wheels.Length];
			for (int i = 0; i < m_wheelData.Length; i++)
			{
				m_wheelData[i] = new WheelData();
				m_wheelData[i].wheel = wheels[i];
				if (wheels[i].wheelCollider == null)
				{
					Debug.LogError("A WheelCollider is missing in the list of wheels for this vehicle: " + base.gameObject.name);
				}
				m_wheelData[i].collider = wheels[i].wheelCollider;
				m_wheelData[i].transform = wheels[i].wheelCollider.transform;
				if (wheels[i].handbrake)
				{
					m_usesHandbrake = true;
				}
				UpdateWheelCollider(m_wheelData[i].collider);
				m_wheelData[i].forceDistance = GetWheelForceDistance(m_wheelData[i].collider);
			}
			WheelCollider componentInChildren = GetComponentInChildren<WheelCollider>();
			componentInChildren.ConfigureVehicleSubsteps(1000f, 1, 1);
			Wheel[] array = wheels;
			foreach (Wheel wheel in array)
			{
				SetupWheelCollider(wheel.wheelCollider);
				UpdateWheelCollider(wheel.wheelCollider);
			}
			m_lastImpactedMaterial = new PhysicMaterial();
		}

		private void Update()
		{
			if (spinUpdateRate == UpdateRate.Disabled)
			{
				WheelData[] array = m_wheelData;
				foreach (WheelData wd in array)
				{
					UpdateSteering(wd);
				}
			}
			else if (spinUpdateRate == UpdateRate.OnUpdate || wheelPositionMode == PositionMode.Accurate)
			{
				bool flag = m_rigidbody.interpolation != 0 && wheelPositionMode == PositionMode.Accurate;
				if (flag)
				{
					DisableCollidersRaycast();
				}
				WheelData[] array2 = m_wheelData;
				foreach (WheelData wd2 in array2)
				{
					UpdateSteering(wd2);
					UpdateTransform(wd2);
				}
				if (flag)
				{
					EnableCollidersRaycast();
				}
			}
			if (processContacts)
			{
				UpdateDragState(Vector3.zero, Vector3.zero, m_localDragHardness);
			}
		}

		private void FixedUpdate()
		{
			throttleInput = Mathf.Clamp(throttleInput, -1f, 1f);
			brakeInput = Mathf.Clamp01(brakeInput);
			handbrakeInput = Mathf.Clamp01(handbrakeInput);
			m_speed = Vector3.Dot(m_rigidbody.velocity, m_transform.forward);
			m_speedAngle = Vector3.Angle(m_rigidbody.velocity, m_transform.forward) * Mathf.Sign(m_speed);
			float referenceDownforce = ((!computeExtendedTireData) ? 1f : (m_rigidbody.mass * Physics.gravity.magnitude / (float)m_wheelData.Length));
			bool flag = spinUpdateRate == UpdateRate.OnFixedUpdate && wheelPositionMode == PositionMode.Fast;
			int num = 0;
			WheelData[] array = m_wheelData;
			foreach (WheelData wheelData in array)
			{
				if (!disallowRuntimeChanges)
				{
					UpdateWheelCollider(wheelData.collider);
				}
				if (flag)
				{
					UpdateSteering(wheelData);
				}
				UpdateSuspension(wheelData);
				UpdateLocalFrame(wheelData);
				UpdateGroundMaterial(wheelData);
				ComputeTireForces(wheelData);
				ApplyTireForces(wheelData);
				UpdateWheelSleep(wheelData);
				if (flag)
				{
					UpdateTransform(wheelData);
				}
				if (wheelData.grounded)
				{
					num++;
				}
				if (computeExtendedTireData)
				{
					ComputeExtendedTireData(wheelData, referenceDownforce);
				}
			}
			float sqrMagnitude = m_rigidbody.velocity.sqrMagnitude;
			Vector3 force = (0f - aeroDrag) * sqrMagnitude * m_rigidbody.velocity.normalized;
			Vector3 force2 = (0f - aeroDownforce) * sqrMagnitude * m_transform.up;
			Vector3 position = m_transform.position + m_transform.forward * aeroAppPointOffset;
			m_rigidbody.AddForceAtPosition(force, position);
			if (num > 0)
			{
				m_rigidbody.AddForceAtPosition(force2, position);
			}
			if (processContacts)
			{
				HandleImpacts();
			}
		}

		private void UpdateSteering(WheelData wd)
		{
			if (wd.wheel.steer)
			{
				if (espEnabled && m_speed > 0f)
				{
					float num = m_speed * espRatio;
					float a = Mathf.Asin(Mathf.Clamp01(3f / num)) * 57.29578f;
					a = Mathf.Max(a, m_speedAngle);
					wd.steerAngle = Mathf.Clamp(maxSteerAngle * steerInput, 0f - a, a);
				}
				else
				{
					wd.steerAngle = maxSteerAngle * steerInput;
				}
			}
			else
			{
				wd.steerAngle = 0f;
			}
			wd.collider.steerAngle = ((!disableSteerAngleCorrection) ? FixSteerAngle(wd, wd.steerAngle) : wd.steerAngle);
		}

		private float FixSteerAngle(WheelData wd, float inputSteerAngle)
		{
			Quaternion quaternion = Quaternion.AngleAxis(inputSteerAngle, wd.transform.up);
			Vector3 vector = quaternion * wd.transform.forward;
			Vector3 vector2 = vector - Vector3.Project(vector, m_transform.up);
			return Vector3.Angle(m_transform.forward, vector2) * Mathf.Sign(Vector3.Dot(m_transform.right, vector2));
		}

		private void UpdateSuspension(WheelData wd)
		{
			wd.grounded = wd.collider.GetGroundHit(out wd.hit);
			wd.origin = wd.transform.TransformPoint(wd.collider.center);
			RaycastHit hitInfo;
			if (wd.grounded && !disableWheelHitCorrection && Physics.Raycast(wd.origin, -wd.transform.up, out hitInfo, wd.collider.suspensionDistance + wd.collider.radius))
			{
				wd.hit.point = hitInfo.point;
				wd.hit.normal = hitInfo.normal;
			}
			if (wd.grounded)
			{
				wd.suspensionCompression = 1f - (0f - wd.transform.InverseTransformPoint(wd.hit.point).y - wd.collider.radius) / wd.collider.suspensionDistance;
				if (wd.hit.force < 0f)
				{
					wd.hit.force = 0f;
				}
				wd.downforce = wd.hit.force;
			}
			else
			{
				wd.suspensionCompression = 0f;
				wd.downforce = 0f;
			}
		}

		private void UpdateLocalFrame(WheelData wd)
		{
			if (!wd.grounded)
			{
				wd.hit.point = wd.origin - wd.transform.up * (wd.collider.suspensionDistance + wd.collider.radius);
				wd.hit.normal = wd.transform.up;
				wd.hit.collider = null;
			}
			Vector3 pointVelocity = m_rigidbody.GetPointVelocity(wd.hit.point);
			if (wd.hit.collider != null)
			{
				Rigidbody attachedRigidbody = wd.hit.collider.attachedRigidbody;
				if (attachedRigidbody != null)
				{
					pointVelocity -= attachedRigidbody.GetPointVelocity(wd.hit.point);
				}
			}
			wd.velocity = pointVelocity - Vector3.Project(pointVelocity, wd.hit.normal);
			wd.localVelocity.y = Vector3.Dot(wd.hit.forwardDir, wd.velocity);
			wd.localVelocity.x = Vector3.Dot(wd.hit.sidewaysDir, wd.velocity);
			if (!wd.grounded)
			{
				wd.localRigForce = Vector2.zero;
				return;
			}
			float num = Mathf.InverseLerp(1f, 0.25f, wd.velocity.sqrMagnitude);
			Vector2 zero = default(Vector2);
			if (num > 0f)
			{
				float num2 = Vector3.Dot(Vector3.up, wd.hit.normal);
				Vector3 rhs;
				if (num2 > 1E-06f)
				{
					Vector3 vector = Vector3.up * wd.hit.force / num2;
					rhs = vector - Vector3.Project(vector, wd.hit.normal);
				}
				else
				{
					rhs = Vector3.up * 100000f;
				}
				zero.y = Vector3.Dot(wd.hit.forwardDir, rhs);
				zero.x = Vector3.Dot(wd.hit.sidewaysDir, rhs);
				zero *= num;
			}
			else
			{
				zero = Vector2.zero;
			}
			float num3 = Mathf.Clamp(wd.hit.force / (0f - Physics.gravity.y), 0f, wd.collider.sprungMass) * 0.5f;
			Vector2 vector2 = (0f - num3) * wd.localVelocity / Time.deltaTime;
			wd.localRigForce = vector2 + zero;
		}

		private void UpdateGroundMaterial(WheelData wd)
		{
			if (wd.grounded)
			{
				UpdateGroundMaterialCached(wd.hit.collider.sharedMaterial, ref wd.lastPhysicMaterial, ref wd.groundMaterial);
			}
		}

		private void ComputeTireForces(WheelData wd)
		{
			float f = ((!wd.wheel.drive) ? 0f : throttleInput);
			float num = maxDriveSlip;
			if (Mathf.Sign(f) != Mathf.Sign(wd.localVelocity.y))
			{
				num -= wd.localVelocity.y * Mathf.Sign(f);
			}
			float num2 = 0f;
			float brakeRatio = 0f;
			float brakeSlip = 0f;
			if (wd.wheel.brake && wd.wheel.handbrake)
			{
				num2 = Mathf.Max(brakeInput, handbrakeInput);
				if (handbrakeInput >= brakeInput)
				{
					ComputeBrakeValues(wd, handbrakeMode, maxHandbrakeSlip, maxHandbrakeRatio, out brakeSlip, out brakeRatio);
				}
				else
				{
					ComputeBrakeValues(wd, brakeMode, maxBrakeSlip, maxBrakeRatio, out brakeSlip, out brakeRatio);
				}
			}
			else if (wd.wheel.brake)
			{
				num2 = brakeInput;
				ComputeBrakeValues(wd, brakeMode, maxBrakeSlip, maxBrakeRatio, out brakeSlip, out brakeRatio);
			}
			else if (wd.wheel.handbrake)
			{
				num2 = handbrakeInput;
				ComputeBrakeValues(wd, handbrakeMode, maxHandbrakeSlip, maxHandbrakeRatio, out brakeSlip, out brakeRatio);
			}
			float num3 = Mathf.Abs(f);
			if (num3 >= num2)
			{
				wd.finalInput = (num3 - num2) * Mathf.Sign(f);
				wd.isBraking = false;
			}
			else
			{
				wd.finalInput = num2 - num3;
				wd.isBraking = true;
			}
			float num4 = ((!wd.isBraking) ? ComputeDriveForce(wd.finalInput * maxDriveForce, maxDriveForce, wd.grounded) : (wd.finalInput * maxBrakeForce));
			if (wd.grounded)
			{
				if (tcEnabled)
				{
					num = Mathf.Lerp(num, 0.1f, tcRatio);
				}
				if (absEnabled && brakeInput > handbrakeInput)
				{
					brakeSlip = Mathf.Lerp(brakeSlip, 0.1f, absRatio);
					brakeRatio = Mathf.Lerp(brakeRatio, brakeRatio * 0.1f, absRatio);
				}
			}
			if (wd.grounded)
			{
				wd.tireSlip.x = wd.localVelocity.x;
				wd.tireSlip.y = wd.localVelocity.y - wd.angularVelocity * wd.collider.radius;
				float grip;
				float drag;
				if (wd.groundMaterial != null)
				{
					grip = wd.groundMaterial.grip;
					drag = wd.groundMaterial.drag;
				}
				else
				{
					grip = defaultGroundGrip;
					drag = defaultGroundDrag;
				}
				float num5 = tireFriction * wd.downforce * grip;
				float num6;
				if (wd.isBraking)
				{
					float max = Mathf.Max(Mathf.Abs(wd.localVelocity.y * brakeRatio), brakeSlip);
					num6 = Mathf.Clamp(Mathf.Abs(num4 * wd.tireSlip.x) / num5, 0f, max);
				}
				else
				{
					num6 = Mathf.Min(Mathf.Abs(num4 * wd.tireSlip.x) / num5, num);
					if (num4 != 0f && num6 < 0.1f)
					{
						num6 = 0.1f;
					}
				}
				if (Mathf.Abs(wd.tireSlip.y) < num6)
				{
					wd.tireSlip.y = num6 * Mathf.Sign(wd.tireSlip.y);
				}
				Vector2 vector = (0f - num5) * wd.tireSlip.normalized;
				vector.x = Mathf.Abs(vector.x);
				vector.y = Mathf.Abs(vector.y);
				wd.tireForce.x = Mathf.Clamp(wd.localRigForce.x, 0f - vector.x, vector.x);
				if (wd.isBraking)
				{
					float num7 = Mathf.Min(vector.y, num4);
					wd.tireForce.y = Mathf.Clamp(wd.localRigForce.y, 0f - num7, num7);
				}
				else
				{
					wd.tireForce.y = Mathf.Clamp(num4, 0f - vector.y, vector.y);
				}
				wd.dragForce = (0f - num5 * wd.localVelocity.magnitude * drag * 0.001f) * wd.localVelocity;
			}
			else
			{
				wd.tireSlip = Vector2.zero;
				wd.tireForce = Vector2.zero;
				wd.dragForce = Vector2.zero;
			}
			float num8 = ((!wd.isBraking) ? driveForceToMaxSlip : brakeForceToMaxSlip);
			float num9 = Mathf.Clamp01((Mathf.Abs(num4) - Mathf.Abs(wd.tireForce.y)) / num8);
			float num10 = ((!wd.isBraking) ? (num9 * num * Mathf.Sign(num4)) : Mathf.Clamp((0f - num9) * wd.localVelocity.y * brakeRatio, 0f - brakeSlip, brakeSlip));
			wd.angularVelocity = (wd.localVelocity.y + num10) / wd.collider.radius;
		}

		private void ApplyTireForces(WheelData wd)
		{
			if (wd.grounded)
			{
				if (!disallowRuntimeChanges)
				{
					wd.forceDistance = GetWheelForceDistance(wd.collider);
				}
				Vector3 position = wd.hit.point + wd.transform.up * antiRoll * wd.forceDistance;
				Vector3 vector = wd.hit.forwardDir * (wd.tireForce.y + wd.dragForce.y);
				Vector3 vector2 = wd.hit.sidewaysDir * (wd.tireForce.x + wd.dragForce.x);
				if (wd.wheel.steer && wd.steerAngle != 0f && Mathf.Sign(wd.steerAngle) != Mathf.Sign(wd.tireSlip.x))
				{
					position += wd.hit.forwardDir * steeringOverdrive;
				}
				m_rigidbody.AddForceAtPosition(vector, wd.hit.point);
				m_rigidbody.AddForceAtPosition(vector2, position);
				Rigidbody attachedRigidbody = wd.hit.collider.attachedRigidbody;
				if (attachedRigidbody != null && !attachedRigidbody.isKinematic)
				{
					attachedRigidbody.AddForceAtPosition(-vector, wd.hit.point);
					attachedRigidbody.AddForceAtPosition(-vector2, position);
				}
			}
		}

		private static float ComputeSlipAngle(Vector2 localVelocity)
		{
			return (!(localVelocity.magnitude > 0.01f)) ? 0f : Mathf.Atan2(localVelocity.x, Mathf.Abs(localVelocity.y));
		}

		private static float ComputeCombinedSlip(Vector2 localVelocity, Vector2 tireSlip)
		{
			float magnitude = localVelocity.magnitude;
			if (magnitude > 0.01f)
			{
				float num = tireSlip.x * localVelocity.x / magnitude;
				float y = tireSlip.y;
				return Mathf.Sqrt(num * num + y * y);
			}
			return tireSlip.magnitude;
		}

		private void ComputeExtendedTireData(WheelData wd, float referenceDownforce)
		{
			wd.combinedTireSlip = ComputeCombinedSlip(wd.localVelocity, wd.tireSlip);
			wd.downforceRatio = wd.hit.force / referenceDownforce;
		}

		private float ComputeDriveForce(float demandedForce, float maxForce, bool grounded)
		{
			float num = Mathf.Abs(m_speed);
			float num2 = ((!(m_speed >= 0f)) ? maxSpeedReverse : maxSpeedForward);
			if (num < num2)
			{
				if ((!(m_speed < 0f) || !(demandedForce > 0f)) && (!(m_speed > 0f) || !(demandedForce < 0f)))
				{
					maxForce *= CommonTools.BiasedLerp(1f - num / num2, forceCurveShape, m_forceBiasCtx);
				}
				return Mathf.Clamp(demandedForce, 0f - maxForce, maxForce);
			}
			float num3 = maxForce * Mathf.Max(1f - num / num2, -1f) * Mathf.Sign(m_speed);
			if ((m_speed < 0f && demandedForce > 0f) || (m_speed > 0f && demandedForce < 0f))
			{
				num3 = Mathf.Clamp(num3 + demandedForce, 0f - maxForce, maxForce);
			}
			return num3;
		}

		private void ComputeBrakeValues(WheelData wd, BrakeMode mode, float maxSlip, float maxRatio, out float brakeSlip, out float brakeRatio)
		{
			if (mode == BrakeMode.Slip)
			{
				brakeSlip = maxSlip;
				brakeRatio = 1f;
			}
			else
			{
				brakeSlip = Mathf.Abs(wd.localVelocity.y);
				brakeRatio = maxRatio;
			}
		}

		private void UpdateTransform(WheelData wd)
		{
			if (wd.wheel.wheelTransform != null)
			{
				wd.angularPosition = (wd.angularPosition + wd.angularVelocity * Time.deltaTime) % ((float)Math.PI * 2f);
				float num;
				if (wheelPositionMode != PositionMode.Fast)
				{
					num = ((!Physics.Raycast(wd.origin, -wd.transform.up, out wd.rayHit, wd.collider.suspensionDistance + wd.collider.radius)) ? (wd.collider.suspensionDistance + wd.collider.radius * 0.05f) : (wd.rayHit.distance - wd.collider.radius * 0.95f));
				}
				else
				{
					num = wd.collider.suspensionDistance * (1f - wd.suspensionCompression) + wd.collider.radius * 0.05f;
					wd.rayHit.point = wd.hit.point;
					wd.rayHit.normal = wd.hit.normal;
				}
				Vector3 position = wd.transform.position - wd.transform.up * num;
				wd.wheel.wheelTransform.position = position;
				wd.wheel.wheelTransform.rotation = wd.transform.rotation * Quaternion.Euler(wd.angularPosition * 57.29578f, wd.steerAngle, 0f);
			}
			else
			{
				wd.rayHit.point = wd.hit.point;
				wd.rayHit.normal = wd.hit.normal;
			}
		}

		private void UpdateGroundMaterialCached(PhysicMaterial colliderMaterial, ref PhysicMaterial cachedMaterial, ref GroundMaterial groundMaterial)
		{
			if (m_groundMaterialManager != null)
			{
				if (colliderMaterial != cachedMaterial)
				{
					cachedMaterial = colliderMaterial;
					groundMaterial = m_groundMaterialManager.GetGroundMaterial(colliderMaterial);
				}
			}
			else
			{
				groundMaterial = null;
			}
		}

		public void ResetVehicle()
		{
			Vector3 localEulerAngles = base.transform.localEulerAngles;
			m_rigidbody.MoveRotation(Quaternion.Euler(0f, localEulerAngles.y, 0f));
			m_rigidbody.MovePosition(m_rigidbody.position + Vector3.up * 1.6f);
			m_rigidbody.velocity = Vector3.zero;
			m_rigidbody.angularVelocity = Vector3.zero;
		}

		private void FindColliders()
		{
			m_colliders = GetComponentsInChildren<Collider>();
			m_colLayers = new int[m_colliders.Length];
			int i = 0;
			for (int num = m_colliders.Length; i < num; i++)
			{
				m_colLayers[i] = m_colliders[i].gameObject.layer;
			}
		}

		private void DisableCollidersRaycast()
		{
			Collider[] colliders = m_colliders;
			foreach (Collider collider in colliders)
			{
				collider.gameObject.layer = 2;
			}
		}

		private void EnableCollidersRaycast()
		{
			int i = 0;
			for (int num = m_colliders.Length; i < num; i++)
			{
				m_colliders[i].gameObject.layer = m_colLayers[i];
			}
		}

		private float GetWheelForceDistance(WheelCollider col)
		{
			return m_rigidbody.centerOfMass.y - m_transform.InverseTransformPoint(col.transform.position).y + col.radius + (1f - col.suspensionSpring.targetPosition) * col.suspensionDistance;
		}

		private void UpdateWheelCollider(WheelCollider col)
		{
			if (col.enabled)
			{
				JointSpring suspensionSpring = col.suspensionSpring;
				float num = (0f - col.sprungMass) * Physics.gravity.y;
				float num2 = num / suspensionSpring.spring;
				suspensionSpring.targetPosition = Mathf.Clamp01(num2 / col.suspensionDistance);
				float min = num / col.suspensionDistance;
				float max = num / (col.suspensionDistance * 0.1f);
				suspensionSpring.spring = Mathf.Clamp(suspensionSpring.spring, min, max);
				col.suspensionSpring = suspensionSpring;
			}
		}

		private void SetupWheelCollider(WheelCollider col)
		{
			m_colliderFriction.stiffness = 0f;
			col.sidewaysFriction = m_colliderFriction;
			col.forwardFriction = m_colliderFriction;
			col.motorTorque = 1E-05f;
		}

		private void UpdateWheelSleep(WheelData wd)
		{
			if (wd.localVelocity.magnitude < sleepVelocity && Time.time - m_lastStrongImpactTime > 0.2f && ((wd.isBraking && wd.finalInput > 0.01f) || (m_usesHandbrake && handbrakeInput > 0.1f)))
			{
				wd.collider.motorTorque = 0f;
			}
			else
			{
				wd.collider.motorTorque = 1E-05f;
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.relativeVelocity.magnitude > 4f)
			{
				m_lastStrongImpactTime = Time.time;
			}
			if (processContacts)
			{
				ProcessContacts(collision, true);
			}
		}

		private void OnCollisionStay(Collision collision)
		{
			if (processContacts)
			{
				ProcessContacts(collision, false);
			}
		}

		private void ProcessContacts(Collision col, bool forceImpact)
		{
			int num = 0;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			int num2 = 0;
			int num3 = 0;
			Vector3 zero3 = Vector3.zero;
			Vector3 zero4 = Vector3.zero;
			int num4 = 0;
			float num5 = impactMinSpeed * impactMinSpeed;
			ContactPoint[] contacts = col.contacts;
			for (int i = 0; i < contacts.Length; i++)
			{
				ContactPoint contactPoint = contacts[i];
				Collider otherCollider = contactPoint.otherCollider;
				int num6 = 0;
				UpdateGroundMaterialCached(otherCollider.sharedMaterial, ref m_lastImpactedMaterial, ref m_impactedGroundMaterial);
				if (m_impactedGroundMaterial != null)
				{
					num6 = ((m_impactedGroundMaterial.surfaceType == GroundMaterial.SurfaceType.Hard) ? 1 : (-1));
				}
				Vector3 pointVelocity = m_rigidbody.GetPointVelocity(contactPoint.point);
				if (otherCollider.attachedRigidbody != null)
				{
					pointVelocity -= otherCollider.attachedRigidbody.GetPointVelocity(contactPoint.point);
				}
				float num7 = Vector3.Dot(pointVelocity, contactPoint.normal);
				if (num7 < 0f - impactThreeshold || (forceImpact && col.relativeVelocity.sqrMagnitude > num5))
				{
					num++;
					zero += contactPoint.point;
					zero2 += col.relativeVelocity;
					num2 += num6;
					if (showContactGizmos)
					{
						Debug.DrawLine(contactPoint.point, contactPoint.point + CommonTools.Lin2Log(pointVelocity), Color.red);
					}
				}
				else if (num7 < impactThreeshold)
				{
					num3++;
					zero3 += contactPoint.point;
					zero4 += pointVelocity;
					num4 += num6;
					if (showContactGizmos)
					{
						Debug.DrawLine(contactPoint.point, contactPoint.point + CommonTools.Lin2Log(pointVelocity), Color.cyan);
					}
				}
				if (showContactGizmos)
				{
					Debug.DrawLine(contactPoint.point, contactPoint.point + contactPoint.normal * 0.25f, Color.yellow);
				}
			}
			if (num > 0)
			{
				float num8 = 1f / (float)num;
				zero *= num8;
				zero2 *= num8;
				m_sumImpactCount++;
				m_sumImpactPosition += m_transform.InverseTransformPoint(zero);
				m_sumImpactVelocity += m_transform.InverseTransformDirection(zero2);
				m_sumImpactHardness += num2;
			}
			if (num3 > 0)
			{
				float num9 = 1f / (float)num3;
				zero3 *= num9;
				zero4 *= num9;
				UpdateDragState(m_transform.InverseTransformPoint(zero3), m_transform.InverseTransformDirection(zero4), num4);
			}
		}

		private void HandleImpacts()
		{
			if (Time.time - m_lastImpactTime >= impactInterval && m_sumImpactCount > 0)
			{
				float num = 1f / (float)m_sumImpactCount;
				m_sumImpactPosition *= num;
				m_sumImpactVelocity *= num;
				if (onImpact != null)
				{
					current = this;
					onImpact();
					current = null;
				}
				if (showContactGizmos && localImpactVelocity.sqrMagnitude > 0.001f)
				{
					Debug.DrawLine(base.transform.TransformPoint(localImpactPosition), base.transform.TransformPoint(localImpactPosition) + CommonTools.Lin2Log(base.transform.TransformDirection(localImpactVelocity)), Color.red, 0.2f, false);
				}
				m_sumImpactCount = 0;
				m_sumImpactPosition = Vector3.zero;
				m_sumImpactVelocity = Vector3.zero;
				m_sumImpactHardness = 0;
				m_lastImpactTime = Time.time + impactInterval * UnityEngine.Random.Range(0f - impactIntervalRandom, impactIntervalRandom);
			}
		}

		private void UpdateDragState(Vector3 dragPosition, Vector3 dragVelocity, int dragHardness)
		{
			if (dragVelocity.sqrMagnitude > 0.001f)
			{
				m_localDragPosition = Vector3.Lerp(m_localDragPosition, dragPosition, 10f * Time.deltaTime);
				m_localDragVelocity = Vector3.Lerp(m_localDragVelocity, dragVelocity, 20f * Time.deltaTime);
				m_localDragHardness = dragHardness;
			}
			else
			{
				m_localDragVelocity = Vector3.Lerp(m_localDragVelocity, Vector3.zero, 10f * Time.deltaTime);
			}
			if (showContactGizmos && localDragVelocity.sqrMagnitude > 0.001f)
			{
				Debug.DrawLine(base.transform.TransformPoint(localDragPosition), base.transform.TransformPoint(localDragPosition) + CommonTools.Lin2Log(base.transform.TransformDirection(localDragVelocity)), Color.cyan, 0.05f, false);
			}
		}

		[ContextMenu("Adjust WheelColliders to their meshes")]
		private void AdjustWheelColliders()
		{
			Wheel[] array = wheels;
			foreach (Wheel wheel in array)
			{
				if (wheel.wheelCollider != null)
				{
					AdjustColliderToWheelMesh(wheel.wheelCollider, wheel.wheelTransform);
				}
			}
		}

		private static void AdjustColliderToWheelMesh(WheelCollider wheelCollider, Transform wheelTransform)
		{
			if (wheelTransform == null)
			{
				Debug.LogError(wheelCollider.gameObject.name + ": A Wheel transform is required");
				return;
			}
			wheelCollider.transform.position = wheelTransform.position + wheelTransform.up * wheelCollider.suspensionDistance * 0.5f;
			wheelCollider.transform.rotation = wheelTransform.rotation;
			MeshFilter[] componentsInChildren = wheelTransform.GetComponentsInChildren<MeshFilter>();
			if (componentsInChildren == null || componentsInChildren.Length == 0)
			{
				Debug.LogWarning(wheelTransform.gameObject.name + ": Couldn't calculate radius. There are no meshes in the Wheel transform or its children");
				return;
			}
			Bounds scaledBounds = GetScaledBounds(componentsInChildren[0]);
			int i = 1;
			for (int num = componentsInChildren.Length; i < num; i++)
			{
				Bounds scaledBounds2 = GetScaledBounds(componentsInChildren[i]);
				scaledBounds.Encapsulate(scaledBounds2.min);
				scaledBounds.Encapsulate(scaledBounds2.max);
			}
			if (Mathf.Abs(scaledBounds.extents.y - scaledBounds.extents.z) > 0.01f)
			{
				Debug.LogWarning(wheelTransform.gameObject.name + ": The Wheel mesh might not be a correct wheel. The calculated radius is different along forward and vertical axis.");
			}
			wheelCollider.radius = scaledBounds.extents.y;
		}

		private static Bounds GetScaledBounds(MeshFilter meshFilter)
		{
			Bounds bounds = meshFilter.sharedMesh.bounds;
			Vector3 lossyScale = meshFilter.transform.lossyScale;
			bounds.max = Vector3.Scale(bounds.max, lossyScale);
			bounds.min = Vector3.Scale(bounds.min, lossyScale);
			return bounds;
		}
	}
}
