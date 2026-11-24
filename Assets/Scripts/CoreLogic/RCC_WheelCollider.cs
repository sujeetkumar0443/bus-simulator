using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Main/RCC Wheel Collider")]
[RequireComponent(typeof(WheelCollider))]
public class RCC_WheelCollider : MonoBehaviour
{
	private RCC_Settings RCCSettingsInstance;

	private RCC_GroundMaterials RCCGroundMaterialsInstance;

	private WheelCollider _wheelCollider;

	private RCC_CarControllerV3 carController;

	private Rigidbody rigid;

	private List<RCC_WheelCollider> allWheelColliders = new List<RCC_WheelCollider>();

	public Transform wheelModel;

	private float wheelRotation;

	public float camber;

	internal float wheelRPMToSpeed;

	private RCC_Skidmarks skidmarks;

	private float startSlipValue = 0.25f;

	private int lastSkidmark = -1;

	private float wheelSlipAmountForward;

	private float wheelSlipAmountSideways;

	internal float totalSlip;

	public WheelFrictionCurve forwardFrictionCurve;

	public WheelFrictionCurve sidewaysFrictionCurve;

	private AudioSource audioSource;

	private AudioClip audioClip;

	internal List<ParticleSystem> allWheelParticles = new List<ParticleSystem>();

	internal ParticleSystem.EmissionModule emission;

	internal float tractionHelpedSidewaysStiffness = 1f;

	private float minForwardStiffness = 0.75f;

	private float maxForwardStiffness = 1f;

	private float minSidewaysStiffness = 0.75f;

	private float maxSidewaysStiffness = 1f;

	private TerrainData mTerrainData;

	private int alphamapWidth;

	private int alphamapHeight;

	private float[,,] mSplatmapData;

	private float mNumTextures;

	public float compression;

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

	private RCC_GroundMaterials RCCGroundMaterials
	{
		get
		{
			if (RCCGroundMaterialsInstance == null)
			{
				RCCGroundMaterialsInstance = RCC_GroundMaterials.Instance;
			}
			return RCCGroundMaterialsInstance;
		}
	}

	public WheelCollider wheelCollider
	{
		get
		{
			if (_wheelCollider == null)
			{
				_wheelCollider = GetComponent<WheelCollider>();
			}
			return _wheelCollider;
		}
	}

	private RCC_GroundMaterials physicsMaterials
	{
		get
		{
			return RCCGroundMaterials;
		}
	}

	private RCC_GroundMaterials.GroundMaterialFrictions[] physicsFrictions
	{
		get
		{
			return RCCGroundMaterials.frictions;
		}
	}

	private void Start()
	{
		carController = GetComponentInParent<RCC_CarControllerV3>();
		rigid = carController.GetComponent<Rigidbody>();
		allWheelColliders = carController.GetComponentsInChildren<RCC_WheelCollider>().ToList();
		GetTerrainData();
		if (!RCCSettings.dontUseSkidmarks)
		{
			if ((bool)Object.FindObjectOfType(typeof(RCC_Skidmarks)))
			{
				skidmarks = Object.FindObjectOfType(typeof(RCC_Skidmarks)) as RCC_Skidmarks;
			}
			else
			{
				skidmarks = Object.Instantiate(RCCSettings.skidmarksManager, Vector3.zero, Quaternion.identity);
			}
		}
		if (RCCSettings.useFixedWheelColliders)
		{
			wheelCollider.mass = rigid.mass / 20f;
		}
		forwardFrictionCurve = wheelCollider.forwardFriction;
		sidewaysFrictionCurve = wheelCollider.sidewaysFriction;
		switch (RCCSettings.behaviorType)
		{
		case RCC_Settings.BehaviorType.SemiArcade:
			forwardFrictionCurve = SetFrictionCurves(forwardFrictionCurve, 0.2f, 2f, 2f, 2f);
			sidewaysFrictionCurve = SetFrictionCurves(sidewaysFrictionCurve, 0.25f, 2f, 2f, 2f);
			wheelCollider.forceAppPointDistance = Mathf.Clamp(wheelCollider.forceAppPointDistance, 0.35f, 1f);
			break;
		case RCC_Settings.BehaviorType.Drift:
			forwardFrictionCurve = SetFrictionCurves(forwardFrictionCurve, 0.3f, 1f, 0.8f, 1f);
			sidewaysFrictionCurve = SetFrictionCurves(sidewaysFrictionCurve, 0.3f, 1f, 0.5f, 1f);
			wheelCollider.forceAppPointDistance = Mathf.Clamp(wheelCollider.forceAppPointDistance, 0.1f, 1f);
			if (carController._wheelTypeChoise == RCC_CarControllerV3.WheelType.FWD)
			{
				Debug.LogWarning("Current behavior mode is ''Drift'', but your vehicle named " + carController.name + " was FWD. You have to use RWD, AWD, or BIASED to rear wheels. Setting it to *RWD* now. ");
				carController._wheelTypeChoise = RCC_CarControllerV3.WheelType.RWD;
			}
			break;
		case RCC_Settings.BehaviorType.Fun:
			forwardFrictionCurve = SetFrictionCurves(forwardFrictionCurve, 0.2f, 2f, 2f, 2f);
			sidewaysFrictionCurve = SetFrictionCurves(sidewaysFrictionCurve, 0.25f, 2f, 2f, 2f);
			wheelCollider.forceAppPointDistance = Mathf.Clamp(wheelCollider.forceAppPointDistance, 0.35f, 1f);
			break;
		case RCC_Settings.BehaviorType.Racing:
			forwardFrictionCurve = SetFrictionCurves(forwardFrictionCurve, 0.2f, 1f, 0.8f, 0.75f);
			sidewaysFrictionCurve = SetFrictionCurves(sidewaysFrictionCurve, 0.3f, 1f, 0.25f, 0.75f);
			wheelCollider.forceAppPointDistance = Mathf.Clamp(wheelCollider.forceAppPointDistance, 0.25f, 1f);
			break;
		case RCC_Settings.BehaviorType.Simulator:
			forwardFrictionCurve = SetFrictionCurves(forwardFrictionCurve, 0.2f, 1f, 0.8f, 0.75f);
			sidewaysFrictionCurve = SetFrictionCurves(sidewaysFrictionCurve, 0.25f, 1f, 0.5f, 0.75f);
			wheelCollider.forceAppPointDistance = Mathf.Clamp(wheelCollider.forceAppPointDistance, 0f, 1f);
			break;
		}
		wheelCollider.forwardFriction = forwardFrictionCurve;
		wheelCollider.sidewaysFriction = sidewaysFrictionCurve;
		if (PlayerPrefs.GetInt("sounds") != 1)
		{
			if (RCCSettings.useSharedAudioSources)
			{
				if (!carController.transform.Find("All Audio Sources/Skid Sound AudioSource"))
				{
					audioSource = RCC_CreateAudioSource.NewAudioSource(carController.gameObject, "Skid Sound AudioSource", 5f, 50f, 0f, audioClip, true, true, false);
				}
				else
				{
					audioSource = carController.transform.Find("All Audio Sources/Skid Sound AudioSource").GetComponent<AudioSource>();
				}
			}
			else
			{
				audioSource = RCC_CreateAudioSource.NewAudioSource(carController.gameObject, "Skid Sound AudioSource", 5f, 50f, 0f, audioClip, true, true, false);
				audioSource.transform.position = base.transform.position;
			}
		}
		if (!RCCSettings.dontUseAnyParticleEffects)
		{
			for (int i = 0; i < RCCGroundMaterials.frictions.Length; i++)
			{
				GameObject gameObject = Object.Instantiate(RCCGroundMaterials.frictions[i].groundParticles, base.transform.position, base.transform.rotation);
				emission = gameObject.GetComponent<ParticleSystem>().emission;
				emission.enabled = false;
				gameObject.transform.SetParent(base.transform, false);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				allWheelParticles.Add(gameObject.GetComponent<ParticleSystem>());
			}
		}
		GameObject gameObject2 = new GameObject("Pivot_" + wheelModel.transform.name);
		gameObject2.transform.position = RCC_GetBounds.GetBoundsCenter(wheelModel.transform);
		gameObject2.transform.rotation = base.transform.rotation;
		gameObject2.transform.SetParent(wheelModel.transform.parent, true);
		wheelModel.SetParent(gameObject2.transform, true);
		wheelModel = gameObject2.transform;
	}

	private void Update()
	{
		if (carController.enabled && !carController.isSleeping)
		{
			WheelAlign();
		}
	}

	private void FixedUpdate()
	{
		if (!carController.enabled || carController.isSleeping)
		{
			return;
		}
		wheelRPMToSpeed = wheelCollider.rpm * wheelCollider.radius / 2.9f * rigid.transform.lossyScale.y;
		switch (carController._wheelTypeChoise)
		{
		case RCC_CarControllerV3.WheelType.FWD:
			if (this == carController.FrontLeftWheelCollider || this == carController.FrontRightWheelCollider)
			{
				ApplyMotorTorque(carController.engineTorque);
			}
			break;
		case RCC_CarControllerV3.WheelType.RWD:
			if (this == carController.RearLeftWheelCollider || this == carController.RearRightWheelCollider)
			{
				ApplyMotorTorque(carController.engineTorque);
			}
			break;
		case RCC_CarControllerV3.WheelType.AWD:
			ApplyMotorTorque(carController.engineTorque);
			break;
		case RCC_CarControllerV3.WheelType.BIASED:
			if (this == carController.FrontLeftWheelCollider || this == carController.FrontRightWheelCollider)
			{
				ApplyMotorTorque(carController.engineTorque * (100f - carController.biasedWheelTorque) / 100f);
			}
			if (this == carController.RearLeftWheelCollider || this == carController.RearRightWheelCollider)
			{
				ApplyMotorTorque(carController.engineTorque * carController.biasedWheelTorque / 100f);
			}
			break;
		}
		if (carController.ExtraRearWheelsCollider.Length > 0 && carController.applyEngineTorqueToExtraRearWheelColliders)
		{
			for (int i = 0; i < carController.ExtraRearWheelsCollider.Length; i++)
			{
				if (this == carController.ExtraRearWheelsCollider[i])
				{
					ApplyMotorTorque(carController.engineTorque);
				}
			}
		}
		if (this == carController.FrontLeftWheelCollider || this == carController.FrontRightWheelCollider)
		{
			ApplySteering();
		}
		if (carController.handbrakeInput > 0.5f)
		{
			if (this == carController.RearLeftWheelCollider || this == carController.RearRightWheelCollider)
			{
				ApplyBrakeTorque(carController.brakeTorque * 1f * carController.handbrakeInput);
			}
		}
		else if (this == carController.FrontLeftWheelCollider || this == carController.FrontRightWheelCollider)
		{
			ApplyBrakeTorque(carController.brakeTorque * Mathf.Clamp(carController._brakeInput, 0f, 1f));
		}
		else
		{
			ApplyBrakeTorque(carController.brakeTorque * (Mathf.Clamp(carController._brakeInput, 0f, 1f) / 2f));
		}
		if (carController.ESP && carController.handbrakeInput < 0.5f)
		{
			if (carController.underSteering)
			{
				if (this == carController.FrontLeftWheelCollider)
				{
					ApplyBrakeTorque(carController.brakeTorque * carController.ESPStrength * Mathf.Clamp(0f - carController.rearSlip, 0f, float.PositiveInfinity));
				}
				if (this == carController.FrontRightWheelCollider)
				{
					ApplyBrakeTorque(carController.brakeTorque * carController.ESPStrength * Mathf.Clamp(carController.rearSlip, 0f, float.PositiveInfinity));
				}
			}
			if (carController.overSteering)
			{
				if (this == carController.RearLeftWheelCollider)
				{
					ApplyBrakeTorque(carController.brakeTorque * carController.ESPStrength * Mathf.Clamp(0f - carController.frontSlip, 0f, float.PositiveInfinity));
				}
				if (this == carController.RearRightWheelCollider)
				{
					ApplyBrakeTorque(carController.brakeTorque * carController.ESPStrength * Mathf.Clamp(carController.frontSlip, 0f, float.PositiveInfinity));
				}
			}
		}
		if (!carController.isSleeping)
		{
			Frictions();
			SkidMarks();
			if (PlayerPrefs.GetInt("sounds") != 1)
			{
				Audio();
			}
		}
	}

	public void WheelAlign()
	{
		if (!wheelModel)
		{
			Debug.LogError(base.transform.name + " wheel of the " + carController.transform.name + " is missing wheel model. This wheel is disabled");
			base.enabled = false;
			return;
		}
		WheelHit hit;
		bool groundHit = wheelCollider.GetGroundHit(out hit);
		float num = compression;
		compression = Mathf.Lerp(b: (!groundHit) ? wheelCollider.suspensionDistance : (1f - (Vector3.Dot(base.transform.position - hit.point, base.transform.up) - wheelCollider.radius * base.transform.lossyScale.y) / wheelCollider.suspensionDistance), a: compression, t: Time.deltaTime * 50f);
		wheelModel.position = base.transform.position;
		wheelModel.position += base.transform.up * (compression - 1f) * wheelCollider.suspensionDistance;
		wheelRotation += wheelCollider.rpm * 6f * Time.deltaTime;
		wheelModel.rotation = base.transform.rotation * Quaternion.Euler(wheelRotation, wheelCollider.steerAngle, base.transform.rotation.z);
		if (base.transform.localPosition.x > 0f)
		{
			base.transform.localRotation = Quaternion.identity * Quaternion.AngleAxis(camber, Vector3.forward);
		}
		else
		{
			base.transform.localRotation = Quaternion.identity * Quaternion.AngleAxis(0f - camber, Vector3.forward);
		}
		float num2 = (0f - wheelCollider.transform.InverseTransformPoint(hit.point).y - wheelCollider.radius * base.transform.lossyScale.y) / wheelCollider.suspensionDistance;
		Debug.DrawLine(hit.point, hit.point + base.transform.up * (hit.force / rigid.mass), (!((double)num2 <= 0.0)) ? Color.white : Color.magenta);
		Debug.DrawLine(hit.point, hit.point - base.transform.forward * hit.forwardSlip * 2f, Color.green);
		Debug.DrawLine(hit.point, hit.point - base.transform.right * hit.sidewaysSlip * 2f, Color.red);
	}

	private void SkidMarks()
	{
		WheelHit hit;
		wheelCollider.GetGroundHit(out hit);
		wheelSlipAmountForward = Mathf.Abs(hit.forwardSlip);
		wheelSlipAmountSideways = Mathf.Abs(hit.sidewaysSlip);
		totalSlip = Mathf.Lerp(totalSlip, wheelSlipAmountSideways + wheelSlipAmountForward, Time.fixedDeltaTime * 3f) / 1f;
		if (!skidmarks)
		{
			return;
		}
		if (wheelSlipAmountSideways + wheelSlipAmountForward > startSlipValue)
		{
			Vector3 pos = hit.point + 2f * rigid.velocity * Time.deltaTime;
			if (rigid.velocity.magnitude > 1f)
			{
				lastSkidmark = skidmarks.AddSkidMark(pos, hit.normal, totalSlip, lastSkidmark);
			}
			else
			{
				lastSkidmark = -1;
			}
		}
		else
		{
			lastSkidmark = -1;
		}
	}

	private void Frictions()
	{
		int groundMaterialIndex = GetGroundMaterialIndex();
		WheelHit hit;
		wheelCollider.GetGroundHit(out hit);
		float handbrakeInput = carController.handbrakeInput;
		handbrakeInput = (((!(this == carController.RearLeftWheelCollider) && !(this == carController.RearRightWheelCollider)) || !(handbrakeInput > 0.75f)) ? 1f : 0.75f);
		forwardFrictionCurve.stiffness = physicsFrictions[groundMaterialIndex].forwardStiffness;
		sidewaysFrictionCurve.stiffness = physicsFrictions[groundMaterialIndex].sidewaysStiffness * handbrakeInput * tractionHelpedSidewaysStiffness;
		if (RCCSettings.behaviorType == RCC_Settings.BehaviorType.Drift)
		{
			Drift();
		}
		wheelCollider.forwardFriction = forwardFrictionCurve;
		wheelCollider.sidewaysFriction = sidewaysFrictionCurve;
		wheelCollider.wheelDampingRate = physicsFrictions[groundMaterialIndex].damp;
		audioClip = physicsFrictions[groundMaterialIndex].groundSound;
		if (RCCSettings.dontUseAnyParticleEffects)
		{
			return;
		}
		for (int i = 0; i < allWheelParticles.Count; i++)
		{
			if (totalSlip > physicsFrictions[groundMaterialIndex].slip)
			{
				if (i != groundMaterialIndex)
				{
					ParticleSystem.EmissionModule emissionModule = allWheelParticles[i].emission;
					emissionModule.enabled = false;
				}
				else
				{
					ParticleSystem.EmissionModule emissionModule2 = allWheelParticles[i].emission;
					emissionModule2.enabled = true;
				}
			}
			else
			{
				ParticleSystem.EmissionModule emissionModule3 = allWheelParticles[i].emission;
				emissionModule3.enabled = false;
			}
		}
	}

	private void Drift()
	{
		WheelHit hit;
		wheelCollider.GetGroundHit(out hit);
		Vector3 vector = base.transform.InverseTransformDirection(rigid.velocity);
		float num = vector.x * vector.x / 10f;
		num += Mathf.Abs(hit.forwardSlip * hit.forwardSlip) * 0.5f;
		if (wheelCollider == carController.FrontLeftWheelCollider.wheelCollider || wheelCollider == carController.FrontRightWheelCollider.wheelCollider)
		{
			forwardFrictionCurve.extremumValue = Mathf.Clamp(1f - num, 0.1f, maxForwardStiffness);
			forwardFrictionCurve.asymptoteValue = Mathf.Clamp(0.75f - num / 2f, 0.1f, minForwardStiffness);
		}
		else
		{
			forwardFrictionCurve.extremumValue = Mathf.Clamp(1f - num, 1f, maxForwardStiffness);
			forwardFrictionCurve.asymptoteValue = Mathf.Clamp(0.75f - num / 2f, 1.2f, minForwardStiffness);
		}
		if (wheelCollider == carController.FrontLeftWheelCollider.wheelCollider || wheelCollider == carController.FrontRightWheelCollider.wheelCollider)
		{
			sidewaysFrictionCurve.extremumValue = Mathf.Clamp(1f - num / 1f, 0.5f, maxSidewaysStiffness);
			sidewaysFrictionCurve.asymptoteValue = Mathf.Clamp(0.75f - num / 2f, 0.5f, minSidewaysStiffness);
		}
		else
		{
			sidewaysFrictionCurve.extremumValue = Mathf.Clamp(1f - num, 0.5f, maxSidewaysStiffness);
			sidewaysFrictionCurve.asymptoteValue = Mathf.Clamp(0.75f - num / 2f, 0.5f, minSidewaysStiffness);
		}
	}

	private void Audio()
	{
		if (RCCSettings.useSharedAudioSources && isSkidding())
		{
			return;
		}
		if (totalSlip > startSlipValue)
		{
			if (audioSource.clip != audioClip)
			{
				audioSource.clip = audioClip;
			}
			if (!audioSource.isPlaying)
			{
				audioSource.Play();
			}
			if (rigid.velocity.magnitude > 1f)
			{
				audioSource.volume = Mathf.Lerp(0f, 0.2f, totalSlip);
				audioSource.pitch = Mathf.Lerp(1f, 0.8f, audioSource.volume);
			}
			else
			{
				audioSource.volume = 0f;
			}
		}
		else
		{
			audioSource.volume = 0f;
			if (audioSource.volume <= 0.05f && audioSource.isPlaying)
			{
				audioSource.Stop();
			}
		}
	}

	private bool isSkidding()
	{
		for (int i = 0; i < allWheelColliders.Count; i++)
		{
			if (allWheelColliders[i].totalSlip > startSlipValue)
			{
				return true;
			}
		}
		return false;
	}

	private void ApplyMotorTorque(float torque)
	{
		if (carController.TCS)
		{
			WheelHit hit;
			wheelCollider.GetGroundHit(out hit);
			if (Mathf.Abs(wheelCollider.rpm) >= 100f)
			{
				if (hit.forwardSlip > startSlipValue)
				{
					carController.TCSAct = true;
					torque -= Mathf.Clamp(torque * hit.forwardSlip * carController.TCSStrength, 0f, carController.engineTorque);
				}
				else
				{
					carController.TCSAct = false;
					torque += Mathf.Clamp(torque * hit.forwardSlip * carController.TCSStrength, 0f - carController.engineTorque, 0f);
				}
			}
			else
			{
				carController.TCSAct = false;
			}
		}
		if (OverTorque())
		{
			torque = 0f;
		}
		wheelCollider.motorTorque = torque * (1f - carController.clutchInput) * carController._boostInput * carController._gasInput * (carController.engineTorqueCurve[carController.currentGear].Evaluate(wheelRPMToSpeed * (float)carController.direction) * (float)carController.direction);
	}

	public void ApplySteering()
	{
		wheelCollider.steerAngle = Mathf.Clamp(carController.steerAngle * carController._steerInput, 0f - carController.steerAngle, carController.steerAngle);
	}

	private void ApplyBrakeTorque(float brake)
	{
		if (carController.ABS && carController.handbrakeInput <= 0.1f)
		{
			WheelHit hit;
			wheelCollider.GetGroundHit(out hit);
			if (Mathf.Abs(hit.forwardSlip) * Mathf.Clamp01(brake) >= carController.ABSThreshold)
			{
				carController.ABSAct = true;
				brake = 0f;
			}
			else
			{
				carController.ABSAct = false;
			}
		}
		wheelCollider.brakeTorque = brake;
	}

	private bool OverTorque()
	{
		if (carController.speed > carController.maxspeed || !carController.engineRunning)
		{
			return true;
		}
		return false;
	}

	private void GetTerrainData()
	{
		if ((bool)Terrain.activeTerrain)
		{
			mTerrainData = Terrain.activeTerrain.terrainData;
			alphamapWidth = mTerrainData.alphamapWidth;
			alphamapHeight = mTerrainData.alphamapHeight;
			mSplatmapData = mTerrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
			mNumTextures = mSplatmapData.Length / (alphamapWidth * alphamapHeight);
		}
	}

	private Vector3 ConvertToSplatMapCoordinate(Vector3 playerPos)
	{
		Vector3 result = default(Vector3);
		Terrain activeTerrain = Terrain.activeTerrain;
		Vector3 position = activeTerrain.transform.position;
		result.x = (playerPos.x - position.x) / activeTerrain.terrainData.size.x * (float)activeTerrain.terrainData.alphamapWidth;
		result.z = (playerPos.z - position.z) / activeTerrain.terrainData.size.z * (float)activeTerrain.terrainData.alphamapHeight;
		return result;
	}

	private int GetGroundMaterialIndex()
	{
		bool flag = false;
		WheelHit hit;
		wheelCollider.GetGroundHit(out hit);
		if (hit.point == Vector3.zero)
		{
			return 0;
		}
		int result = 0;
		for (int i = 0; i < physicsFrictions.Length; i++)
		{
			if (hit.collider.sharedMaterial == physicsFrictions[i].groundMaterial)
			{
				flag = true;
				result = i;
			}
		}
		if (!flag && physicsMaterials.useTerrainSplatMapForGroundFrictions && hit.collider.sharedMaterial == RCCGroundMaterials.terrainPhysicMaterial)
		{
			Vector3 position = base.transform.position;
			Vector3 vector = ConvertToSplatMapCoordinate(position);
			float num = 0f;
			for (int j = 0; (float)j < mNumTextures; j++)
			{
				if (num < mSplatmapData[(int)vector.z, (int)vector.x, j])
				{
					result = j;
				}
			}
		}
		return result;
	}

	public WheelFrictionCurve SetFrictionCurves(WheelFrictionCurve curve, float extremumSlip, float extremumValue, float asymptoteSlip, float asymptoteValue)
	{
		WheelFrictionCurve result = curve;
		result.extremumSlip = extremumSlip;
		result.extremumValue = extremumValue;
		result.asymptoteSlip = asymptoteSlip;
		result.asymptoteValue = asymptoteValue;
		return result;
	}
}
