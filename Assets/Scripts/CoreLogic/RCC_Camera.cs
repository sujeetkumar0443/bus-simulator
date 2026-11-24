using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Camera/RCC Camera")]
public class RCC_Camera : MonoBehaviour
{
	public enum CameraMode
	{
		TPS = 0,
		FPS = 1,
		WHEEL = 2,
		FIXED = 3,
		CINEMATIC = 4,
		TOP = 5
	}

	public delegate void onBCGCameraSpawned(GameObject BCGCamera);

	private RCC_Settings RCCSettingsInstance;

	public bool isRendering = true;

	public RCC_CarControllerV3 playerCar;

	private Rigidbody playerRigid;

	private float playerSpeed;

	private Vector3 playerVelocity = new Vector3(0f, 0f, 0f);

	public Camera thisCam;

	public GameObject pivot;

	public CameraMode cameraMode;

	public CameraMode lastCameraMode;

	public bool useTopCameraMode;

	public bool useHoodCameraMode = true;

	public bool useOrbitInTPSCameraMode;

	public bool useOrbitInHoodCameraMode;

	public bool useWheelCameraMode = true;

	public bool useFixedCameraMode;

	public bool useCinematicCameraMode;

	public bool useOrthoForTopCamera;

	public bool useOcclusion = true;

	public LayerMask occlusionLayerMask = -1;

	public bool useAutoChangeCamera;

	private float autoChangeCameraTimer;

	public Vector3 topCameraAngle = new Vector3(45f, 45f, 0f);

	private float distanceOffset;

	public float maximumZDistanceOffset = 10f;

	public float topCameraDistance = 100f;

	private Vector3 targetPosition;

	private Vector3 lastFollowerPosition = Vector3.zero;

	private Vector3 lastTargetPosition = Vector3.zero;

	private int direction = 1;

	private int lastDirection = 1;

	public float TPSDistance = 6f;

	public float TPSHeight = 2f;

	public float TPSHeightDamping = 10f;

	public float TPSRotationDamping = 5f;

	public float TPSTiltMaximum = 15f;

	public float TPSTiltMultiplier = 2f;

	private float TPSTiltAngle;

	public float TPSYawAngle;

	public float TPSPitchAngle = 7f;

	internal float targetFieldOfView = 60f;

	public float TPSMinimumFOV = 50f;

	public float TPSMaximumFOV = 70f;

	public float hoodCameraFOV = 60f;

	public float wheelCameraFOV = 60f;

	public float minimumOrtSize = 10f;

	public float maximumOrtSize = 20f;

	internal int cameraSwitchCount;

	private RCC_HoodCamera hoodCam;

	private RCC_WheelCamera wheelCam;

	private RCC_FixedCamera fixedCam;

	private RCC_CinematicCamera cinematicCam;

	private Vector3 collisionVector = Vector3.zero;

	private Vector3 collisionPos = Vector3.zero;

	private Quaternion collisionRot = Quaternion.identity;

	private float index;

	private Quaternion orbitRotation = Quaternion.identity;

	internal float orbitX;

	internal float orbitY;

	public float minOrbitY = -20f;

	public float maxOrbitY = 80f;

	public float orbitXSpeed = 7.5f;

	public float orbitYSpeed = 5f;

	private float orbitResetTimer;

	private Quaternion currentRotation = Quaternion.identity;

	private Quaternion wantedRotation = Quaternion.identity;

	private float currentHeight;

	private float wantedHeight;

	private RCC_Settings RCCSettings
	{
		get
		{
			if (RCCSettingsInstance == null)
			{
				RCCSettingsInstance = RCC_Settings.Instance;
			}
			return RCCSettingsInstance;
		}
	}

	public static event onBCGCameraSpawned OnBCGCameraSpawned;

	private void Awake()
	{
		thisCam = GetComponentInChildren<Camera>();
		switch (RCCSettings.behaviorType)
		{
		case RCC_Settings.BehaviorType.SemiArcade:
			break;
		case RCC_Settings.BehaviorType.Drift:
			break;
		case RCC_Settings.BehaviorType.Fun:
			break;
		case RCC_Settings.BehaviorType.Racing:
			break;
		case RCC_Settings.BehaviorType.Simulator:
			break;
		}
	}

	private void OnEnable()
	{
		if (RCC_Camera.OnBCGCameraSpawned != null)
		{
			RCC_Camera.OnBCGCameraSpawned(base.gameObject);
		}
		RCC_CarControllerV3.OnRCCPlayerCollision += RCC_CarControllerV3_OnRCCPlayerCollision;
		lastFollowerPosition = base.transform.position;
		lastTargetPosition = base.transform.position;
	}

	private void RCC_CarControllerV3_OnRCCPlayerCollision(RCC_CarControllerV3 RCC, Collision collision)
	{
		Collision(collision);
	}

	private void GetTarget()
	{
		if ((bool)playerCar)
		{
			if ((bool)playerCar.GetComponent<RCC_CameraConfig>())
			{
				TPSDistance = playerCar.GetComponent<RCC_CameraConfig>().distance;
				TPSHeight = playerCar.GetComponent<RCC_CameraConfig>().height;
			}
			playerRigid = playerCar.GetComponent<Rigidbody>();
			hoodCam = playerCar.GetComponentInChildren<RCC_HoodCamera>();
			wheelCam = playerCar.GetComponentInChildren<RCC_WheelCamera>();
			fixedCam = Object.FindObjectOfType<RCC_FixedCamera>();
			cinematicCam = Object.FindObjectOfType<RCC_CinematicCamera>();
			ResetCamera();
		}
	}

	public void SetTarget(GameObject player)
	{
		playerCar = player.GetComponent<RCC_CarControllerV3>();
		GetTarget();
	}

	public void RemoveTarget()
	{
		base.transform.SetParent(null);
		playerCar = null;
		playerRigid = null;
	}

	private void Update()
	{
		if (!isRendering)
		{
			if (thisCam.gameObject.activeInHierarchy)
			{
				thisCam.gameObject.SetActive(false);
			}
			return;
		}
		if (!thisCam.gameObject.activeInHierarchy)
		{
			thisCam.gameObject.SetActive(true);
		}
		if (!playerCar || !playerRigid)
		{
			GetTarget();
			return;
		}
		playerSpeed = Mathf.Lerp(playerSpeed, playerCar.speed, Time.deltaTime * 5f);
		playerVelocity = playerCar.transform.InverseTransformDirection(playerRigid.velocity);
		if (index > 0f)
		{
			index -= Time.deltaTime * 5f;
		}
		thisCam.fieldOfView = Mathf.Lerp(thisCam.fieldOfView, targetFieldOfView, Time.deltaTime * 5f);
	}

	private void LateUpdate()
	{
		if (!playerCar || !playerRigid || !playerCar.gameObject.activeSelf)
		{
			return;
		}
		switch (cameraMode)
		{
		case CameraMode.TPS:
			TPS();
			if (useOrbitInTPSCameraMode)
			{
				ORBIT();
			}
			break;
		case CameraMode.FPS:
			FPS();
			if (useOrbitInHoodCameraMode)
			{
				ORBIT();
			}
			break;
		case CameraMode.WHEEL:
			WHEEL();
			break;
		case CameraMode.FIXED:
			FIXED();
			break;
		case CameraMode.CINEMATIC:
			CINEMATIC();
			break;
		case CameraMode.TOP:
			TOP();
			break;
		}
		if (lastCameraMode != cameraMode)
		{
			ResetCamera();
		}
		lastCameraMode = cameraMode;
		autoChangeCameraTimer += Time.deltaTime;
		if (Input.GetKeyDown(RCCSettings.changeCameraKB))
		{
			ChangeCamera();
		}
		if (useAutoChangeCamera && autoChangeCameraTimer > 10f)
		{
			autoChangeCameraTimer = 0f;
			ChangeCamera();
		}
	}

	public void ChangeCamera()
	{
		cameraSwitchCount++;
		if (cameraSwitchCount >= 6)
		{
			cameraSwitchCount = 0;
		}
		switch (cameraSwitchCount)
		{
		case 0:
			cameraMode = CameraMode.TPS;
			HR_ExtraComponentHandling.instance.SetDriver(true);
			break;
		case 1:
			if (useHoodCameraMode && (bool)hoodCam)
			{
				cameraMode = CameraMode.FPS;
				HR_ExtraComponentHandling.instance.SetDriver(false);
			}
			else
			{
				ChangeCamera();
			}
			break;
		case 2:
			if (useWheelCameraMode && (bool)wheelCam)
			{
				cameraMode = CameraMode.WHEEL;
			}
			else
			{
				ChangeCamera();
			}
			break;
		case 3:
			if (useFixedCameraMode && (bool)fixedCam)
			{
				cameraMode = CameraMode.FIXED;
			}
			else
			{
				ChangeCamera();
			}
			break;
		case 4:
			if (useCinematicCameraMode && (bool)cinematicCam)
			{
				cameraMode = CameraMode.CINEMATIC;
			}
			else
			{
				ChangeCamera();
			}
			break;
		case 5:
			if (useTopCameraMode)
			{
				cameraMode = CameraMode.TOP;
			}
			else
			{
				ChangeCamera();
			}
			break;
		}
	}

	public void ChangeCamera(CameraMode mode)
	{
		cameraMode = mode;
	}

	private void FPS()
	{
	}

	private void WHEEL()
	{
		if (useOcclusion && Occluding(playerCar.transform.position))
		{
			ChangeCamera(CameraMode.TPS);
		}
	}

	private void TPS()
	{
		if (lastDirection != playerCar.direction)
		{
			direction = playerCar.direction;
			orbitX = 0f;
			orbitY = 0f;
		}
		lastDirection = playerCar.direction;
		wantedRotation = playerCar.transform.rotation * Quaternion.AngleAxis((float)((direction != 1) ? 180 : 0) + ((!useOrbitInTPSCameraMode) ? 0f : orbitX), Vector3.up);
		wantedRotation *= Quaternion.AngleAxis((!useOrbitInTPSCameraMode) ? 0f : orbitY, Vector3.right);
		if (Input.GetKey(KeyCode.B))
		{
			wantedRotation *= Quaternion.AngleAxis(180f, Vector3.up);
		}
		wantedHeight = playerCar.transform.position.y + TPSHeight;
		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, TPSHeightDamping * Time.fixedDeltaTime);
		if (Time.time > 1f)
		{
			currentRotation = Quaternion.Lerp(currentRotation, wantedRotation, TPSRotationDamping * Time.deltaTime);
		}
		else
		{
			currentRotation = wantedRotation;
		}
		TPSTiltAngle = Mathf.Lerp(0f, TPSTiltMaximum * Mathf.Clamp(0f - playerVelocity.x, -1f, 1f), Mathf.Abs(playerVelocity.x) / 50f);
		TPSTiltAngle *= TPSTiltMultiplier;
		targetPosition = playerCar.transform.position;
		targetPosition -= currentRotation * Vector3.forward * (TPSDistance * Mathf.Lerp(1f, 0.75f, playerRigid.velocity.magnitude * 3.6f / 100f));
		targetPosition += Vector3.up * (TPSHeight * Mathf.Lerp(1f, 0.75f, playerRigid.velocity.magnitude * 3.6f / 100f));
		base.transform.position = targetPosition;
		thisCam.transform.localPosition = Vector3.Lerp(thisCam.transform.localPosition, new Vector3(TPSTiltAngle / 10f, 0f, 0f), Time.deltaTime * 3f);
		base.transform.LookAt(playerCar.transform);
		base.transform.eulerAngles = new Vector3(currentRotation.eulerAngles.x + TPSPitchAngle * Mathf.Lerp(1f, 0.75f, playerRigid.velocity.magnitude * 3.6f / 100f), base.transform.eulerAngles.y, 0f - Mathf.Clamp(TPSTiltAngle, 0f - TPSTiltMaximum, TPSTiltMaximum) + TPSYawAngle);
		lastFollowerPosition = base.transform.position;
		lastTargetPosition = targetPosition;
		collisionPos = Vector3.Lerp(new Vector3(collisionPos.x, collisionPos.y, collisionPos.z), Vector3.zero, Time.unscaledDeltaTime * 5f);
		if (Time.deltaTime != 0f)
		{
			collisionRot = Quaternion.Lerp(collisionRot, Quaternion.identity, Time.deltaTime * 5f);
		}
		pivot.transform.localPosition = Vector3.Lerp(pivot.transform.localPosition, collisionPos, Time.deltaTime * 10f);
		pivot.transform.localRotation = Quaternion.Lerp(pivot.transform.localRotation, collisionRot, Time.deltaTime * 10f);
		targetFieldOfView = Mathf.Lerp(TPSMinimumFOV, TPSMaximumFOV, Mathf.Abs(playerSpeed) / 150f);
		targetFieldOfView += 5f * Mathf.Cos(index);
		if (useOcclusion)
		{
			OccludeRay(playerCar.transform.position);
		}
	}

	private void FIXED()
	{
		if (fixedCam.transform.parent != null)
		{
			fixedCam.transform.SetParent(null);
		}
		if (useOcclusion && Occluding(playerCar.transform.position))
		{
			fixedCam.ChangePosition();
		}
	}

	private void TOP()
	{
		if ((bool)playerCar && (bool)playerRigid)
		{
			targetPosition = playerCar.transform.position;
			targetPosition += playerCar.transform.rotation * Vector3.forward * distanceOffset;
			thisCam.orthographic = useOrthoForTopCamera;
			distanceOffset = Mathf.Lerp(0f, maximumZDistanceOffset, Mathf.Abs(playerSpeed) / 100f);
			targetFieldOfView = Mathf.Lerp(minimumOrtSize, maximumOrtSize, Mathf.Abs(playerSpeed) / 100f);
			thisCam.orthographicSize = targetFieldOfView;
			base.transform.position = SmoothApproach(lastFollowerPosition, lastTargetPosition, targetPosition, Mathf.Clamp(0.1f, Mathf.Abs(playerSpeed / 2f), float.PositiveInfinity));
			base.transform.rotation = Quaternion.Euler(topCameraAngle);
			lastFollowerPosition = base.transform.position;
			lastTargetPosition = targetPosition;
			pivot.transform.localPosition = new Vector3(0f, 0f, 0f - topCameraDistance);
		}
	}

	private void ORBIT()
	{
		orbitY = Mathf.Clamp(orbitY, minOrbitY, maxOrbitY);
		orbitRotation = Quaternion.Euler(orbitY, orbitX, 0f);
		if (playerSpeed > 10f && Mathf.Abs(orbitX) > 1f)
		{
			orbitResetTimer += Time.deltaTime;
		}
		if (playerSpeed > 10f && orbitResetTimer >= 2f)
		{
			orbitX = 0f;
			orbitY = 0f;
			orbitResetTimer = 0f;
		}
	}

	public void OnDrag(PointerEventData pointerData)
	{
		orbitX += pointerData.delta.x * orbitXSpeed * 0.02f;
		orbitY -= pointerData.delta.y * orbitYSpeed * 0.02f;
		orbitResetTimer = 0f;
	}

	private void CINEMATIC()
	{
		if (cinematicCam.transform.parent != null)
		{
			cinematicCam.transform.SetParent(null);
		}
		targetFieldOfView = cinematicCam.targetFOV;
		if (useOcclusion && Occluding(playerCar.transform.position))
		{
			ChangeCamera(CameraMode.TPS);
		}
	}

	public void Collision(Collision collision)
	{
		if (base.enabled && isRendering && cameraMode == CameraMode.TPS)
		{
			Vector3 relativeVelocity = collision.relativeVelocity;
			relativeVelocity *= 1f - Mathf.Abs(Vector3.Dot(base.transform.up, collision.contacts[0].normal));
			float num = Mathf.Abs(Vector3.Dot(collision.contacts[0].normal, relativeVelocity.normalized));
			if (relativeVelocity.magnitude * num >= 5f)
			{
				collisionVector = base.transform.InverseTransformDirection(relativeVelocity) / 30f;
				collisionPos -= collisionVector * 5f;
				collisionRot = Quaternion.Euler(new Vector3((0f - collisionVector.z) * 10f, (0f - collisionVector.y) * 10f, (0f - collisionVector.x) * 10f));
				targetFieldOfView = thisCam.fieldOfView - Mathf.Clamp(collision.relativeVelocity.magnitude, 0f, 15f);
				index = Mathf.Clamp(relativeVelocity.magnitude * num * 50f, 0f, 10f);
			}
		}
	}

	private void ResetCamera()
	{
		if ((bool)fixedCam)
		{
			fixedCam.canTrackNow = false;
		}
		TPSTiltAngle = 0f;
		collisionPos = Vector3.zero;
		collisionRot = Quaternion.identity;
		thisCam.transform.localPosition = Vector3.zero;
		thisCam.transform.localRotation = Quaternion.identity;
		pivot.transform.localPosition = collisionPos;
		pivot.transform.localRotation = collisionRot;
		lastFollowerPosition = base.transform.position;
		lastTargetPosition = targetPosition;
		orbitX = 0f;
		orbitY = 0f;
		thisCam.orthographic = false;
		switch (cameraMode)
		{
		case CameraMode.TPS:
			base.transform.SetParent(null);
			targetFieldOfView = TPSMaximumFOV;
			break;
		case CameraMode.FPS:
			base.transform.SetParent(hoodCam.transform, false);
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			targetFieldOfView = hoodCameraFOV;
			hoodCam.FixShake();
			break;
		case CameraMode.WHEEL:
			base.transform.SetParent(wheelCam.transform, false);
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			targetFieldOfView = wheelCameraFOV;
			break;
		case CameraMode.FIXED:
			base.transform.SetParent(fixedCam.transform, false);
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			targetFieldOfView = 60f;
			fixedCam.canTrackNow = true;
			break;
		case CameraMode.CINEMATIC:
			base.transform.SetParent(cinematicCam.pivot.transform, false);
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			targetFieldOfView = 30f;
			break;
		case CameraMode.TOP:
			base.transform.SetParent(null);
			targetFieldOfView = minimumOrtSize;
			pivot.transform.localPosition = Vector3.zero;
			pivot.transform.localRotation = Quaternion.identity;
			targetPosition = playerCar.transform.position;
			targetPosition += playerCar.transform.rotation * Vector3.forward * distanceOffset;
			base.transform.position = playerCar.transform.position;
			lastFollowerPosition = playerCar.transform.position;
			lastTargetPosition = targetPosition;
			break;
		}
	}

	private Vector3 SmoothApproach(Vector3 pastPosition, Vector3 pastTargetPosition, Vector3 targetPosition, float delta)
	{
		if (Time.timeScale == 0f || float.IsNaN(delta) || float.IsInfinity(delta) || delta == 0f || pastPosition == Vector3.zero || pastTargetPosition == Vector3.zero || targetPosition == Vector3.zero)
		{
			return base.transform.position;
		}
		float num = Time.deltaTime * delta + 1E-05f;
		Vector3 vector = (targetPosition - pastTargetPosition) / num;
		Vector3 vector2 = pastPosition - pastTargetPosition + vector;
		Vector3 vector3 = targetPosition - vector + vector2 * Mathf.Exp(0f - num);
		if (vector3 != Vector3.negativeInfinity && vector3 != Vector3.positiveInfinity && vector3 != Vector3.zero)
		{
			return vector3;
		}
		return base.transform.position;
	}

	public void ToggleCamera(bool state)
	{
		isRendering = state;
	}

	private void OccludeRay(Vector3 targetFollow)
	{
		RaycastHit hitInfo = default(RaycastHit);
		if (Physics.Linecast(targetFollow, base.transform.position, out hitInfo, occlusionLayerMask) && !hitInfo.collider.isTrigger && !hitInfo.transform.IsChildOf(playerCar.transform))
		{
			Vector3 position = new Vector3(hitInfo.point.x + hitInfo.normal.x * 0.2f, hitInfo.point.y + hitInfo.normal.y * 0.2f, hitInfo.point.z + hitInfo.normal.z * 0.2f);
			base.transform.position = position;
		}
	}

	private bool Occluding(Vector3 targetFollow)
	{
		RaycastHit hitInfo = default(RaycastHit);
		if (Physics.Linecast(targetFollow, base.transform.position, out hitInfo, ~(1 << LayerMask.NameToLayer(RCCSettingsInstance.RCCLayer))) && !hitInfo.collider.isTrigger && !hitInfo.transform.IsChildOf(playerCar.transform))
		{
			return true;
		}
		return false;
	}

	private void OnDisable()
	{
		RCC_CarControllerV3.OnRCCPlayerCollision -= RCC_CarControllerV3_OnRCCPlayerCollision;
	}
}
