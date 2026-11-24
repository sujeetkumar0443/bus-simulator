using UnityEngine;

public class hands : MonoBehaviour
{
	private Animator anim;

	private float ikWeightLeft1 = 1f;

	private float ikWeightRight1 = 1f;

	public GameObject cars;

	public GameObject steeringWheel;

	public Transform leftIkTarget;

	public Transform leftIkTarget2;

	public Transform leftIkTarget3;

	public Transform leftIkTarget4;

	public Transform leftIkTarget5;

	public Transform rightIkTarget;

	public Transform rightIkTarget2;

	public Transform rightIkTarget3;

	public Transform rightIkTarget4;

	public Transform rightIkTarget5;

	public Transform hintLeft;

	public Transform hintRight;

	private float steerangle;

	private float rightHandAngle;

	private float leftHandAngle;

	private float test;

	private Quaternion rightHandDefaulRot;

	private Vector3 rightHandDefaultPos;

	private Quaternion leftHandDefaulRot;

	private Vector3 leftHandDefaultPos;

	private Quaternion rightHandDefaulRot3;

	private Vector3 rightHandDefaultPos3;

	private Quaternion leftHandDefaulRot3;

	private Vector3 leftHandDefaultPos3;

	private Quaternion rightHandDefaulRot5;

	private Vector3 rightHandDefaultPos5;

	private Quaternion leftHandDefaulRot5;

	private Vector3 leftHandDefaultPos5;

	public bool isRight;

	private void Start()
	{
		anim = GetComponent<Animator>();
		rightHandDefaulRot = rightIkTarget.localRotation;
		rightHandDefaultPos = rightIkTarget.localPosition;
		leftHandDefaulRot = leftIkTarget.localRotation;
		leftHandDefaultPos = leftIkTarget.localPosition;
		rightHandDefaulRot3 = rightIkTarget3.localRotation;
		rightHandDefaultPos3 = rightIkTarget3.localPosition;
		leftHandDefaulRot3 = leftIkTarget3.localRotation;
		leftHandDefaultPos3 = leftIkTarget3.localPosition;
		rightHandDefaulRot5 = rightIkTarget5.localRotation;
		rightHandDefaultPos5 = rightIkTarget5.localPosition;
		leftHandDefaulRot5 = leftIkTarget5.localRotation;
		leftHandDefaultPos5 = leftIkTarget5.localPosition;
	}

	private void Update()
	{
		steerangle = Input.GetAxis("Horizontal") * 360f;
		float axis = Input.GetAxis("Horizontal");
		steeringWheel.transform.localEulerAngles = new Vector3(0f, axis * 100f, 0f);
		Debug.Log("steer angle = " + steerangle);
		if (steerangle < -340f)
		{
			steerangle = -340f;
		}
		if (steerangle > 340f)
		{
			steerangle = 340f;
		}
		rightHandAngle = 0f - steerangle - 40f;
		if (steerangle < -190f)
		{
			rightHandAngle = 0f - steerangle - 280f;
		}
		if (rightHandAngle < 0f)
		{
			rightHandAngle = 0f;
		}
		if (rightHandAngle > 60f)
		{
			rightHandAngle = 60f;
		}
		rightIkTarget.localRotation = rightHandDefaulRot * Quaternion.Euler(new Vector3(0f - rightHandAngle, 0f, rightHandAngle * 0.5f));
		rightIkTarget.localPosition = rightHandDefaultPos - new Vector3(rightHandAngle * 0.0013f, 0f, 0f);
		if (steerangle > 60f)
		{
			rightIkTarget.position = Vector3.Lerp(rightIkTarget.position, rightIkTarget2.position, steerangle / 30f - 2f);
			rightIkTarget.rotation = Quaternion.Lerp(rightIkTarget.rotation, rightIkTarget2.rotation, steerangle / 30f - 2f);
			anim.SetBool("rightOpen", true);
		}
		else
		{
			anim.SetBool("rightOpen", false);
		}
		if (steerangle > 90f)
		{
			rightIkTarget.position = Vector3.Lerp(rightIkTarget2.position, rightIkTarget3.position, steerangle / 30f - 3f);
			rightIkTarget.rotation = Quaternion.Lerp(rightIkTarget2.rotation, rightIkTarget3.rotation, steerangle / 30f - 3f);
		}
		if (steerangle > 110f)
		{
			anim.SetBool("rightOpen", false);
			rightIkTarget.localRotation = rightIkTarget3.localRotation * Quaternion.Euler(new Vector3(-60f, 0f, 30f));
			rightIkTarget.localPosition = rightIkTarget3.localPosition - new Vector3(0f, 0f, -0.078f);
		}
		if (steerangle < -110f)
		{
			anim.SetBool("rightOpen", true);
			rightIkTarget.position = Vector3.Lerp(rightIkTarget.position, rightIkTarget4.position, (0f - steerangle) / 50f - 2f);
			rightIkTarget.rotation = Quaternion.Lerp(rightIkTarget.rotation, rightIkTarget4.rotation, (0f - steerangle) / 50f - 2f);
		}
		if (steerangle < -150f)
		{
			rightIkTarget.position = Vector3.Lerp(rightIkTarget.position, rightIkTarget5.position, (0f - steerangle) / 50f - 3f);
			rightIkTarget.rotation = Quaternion.Lerp(rightIkTarget.rotation, rightIkTarget5.rotation, (0f - steerangle) / 50f - 3f);
		}
		if (steerangle < -190f)
		{
			anim.SetBool("rightOpen", false);
			rightIkTarget5.localRotation = rightHandDefaulRot5 * Quaternion.Euler(new Vector3(0f - rightHandAngle, 0f, rightHandAngle * 0.5f));
			rightIkTarget5.localPosition = rightHandDefaultPos5 - new Vector3(0f, 0f, rightHandAngle * 0.0016f);
		}
		if (steerangle > 200f)
		{
			rightHandAngle = steerangle - 260f;
			if (rightHandAngle > 0f)
			{
				rightHandAngle = 0f;
			}
			if (rightHandAngle < -60f)
			{
				rightHandAngle = -60f;
			}
			rightIkTarget.localRotation = rightHandDefaulRot3 * Quaternion.Euler(new Vector3(rightHandAngle, 0f, rightHandAngle * -0.5f));
			rightIkTarget.localPosition = rightHandDefaultPos3 - new Vector3(0f, 0f, rightHandAngle * 0.0013f);
		}
		leftHandAngle = steerangle - 40f;
		if (steerangle > 190f)
		{
			leftHandAngle = steerangle - 280f;
		}
		if (leftHandAngle < 0f)
		{
			leftHandAngle = 0f;
		}
		if (leftHandAngle > 60f)
		{
			leftHandAngle = 60f;
		}
		leftIkTarget.localRotation = leftHandDefaulRot * Quaternion.Euler(new Vector3(0f - leftHandAngle, 0f, leftHandAngle * -0.5f));
		leftIkTarget.localPosition = leftHandDefaultPos - new Vector3(leftHandAngle * -0.0013f, 0f, 0f);
		if (steerangle < -60f)
		{
			leftIkTarget.position = Vector3.Lerp(leftIkTarget.position, leftIkTarget2.position, (0f - steerangle) / 30f - 2f);
			leftIkTarget.rotation = Quaternion.Lerp(leftIkTarget.rotation, leftIkTarget2.rotation, (0f - steerangle) / 30f - 2f);
			anim.SetBool("leftOpen", true);
		}
		else
		{
			anim.SetBool("leftOpen", false);
		}
		if (steerangle < -90f)
		{
			leftIkTarget.position = Vector3.Lerp(leftIkTarget2.position, leftIkTarget3.position, (0f - steerangle) / 30f - 3f);
			leftIkTarget.rotation = Quaternion.Lerp(leftIkTarget2.rotation, leftIkTarget3.rotation, (0f - steerangle) / 30f - 3f);
		}
		if (steerangle < -110f)
		{
			anim.SetBool("leftOpen", false);
			leftIkTarget.localRotation = leftIkTarget3.localRotation * Quaternion.Euler(new Vector3(-60f, 0f, -30f));
			leftIkTarget.localPosition = leftIkTarget3.localPosition - new Vector3(0f, 0f, -0.078f);
		}
		if (steerangle > 110f)
		{
			anim.SetBool("leftOpen", true);
			leftIkTarget.position = Vector3.Lerp(leftIkTarget.position, leftIkTarget4.position, steerangle / 50f - 2f);
			leftIkTarget.rotation = Quaternion.Lerp(leftIkTarget.rotation, leftIkTarget4.rotation, steerangle / 50f - 2f);
		}
		if (steerangle > 150f)
		{
			leftIkTarget.position = Vector3.Lerp(leftIkTarget.position, leftIkTarget5.position, steerangle / 50f - 3f);
			leftIkTarget.rotation = Quaternion.Lerp(leftIkTarget.rotation, leftIkTarget5.rotation, steerangle / 50f - 3f);
		}
		if (steerangle > 190f)
		{
			anim.SetBool("leftOpen", false);
			leftIkTarget5.localRotation = leftHandDefaulRot5 * Quaternion.Euler(new Vector3(0f - leftHandAngle, 0f, leftHandAngle * -0.5f));
			leftIkTarget5.localPosition = leftHandDefaultPos5 - new Vector3(0f, 0f, leftHandAngle * 0.0016f);
		}
		if (steerangle < -200f)
		{
			leftHandAngle = steerangle + 260f;
			if (leftHandAngle > 60f)
			{
				leftHandAngle = 60f;
			}
			if (leftHandAngle < 0f)
			{
				leftHandAngle = 0f;
			}
			leftIkTarget.localRotation = leftHandDefaulRot3 * Quaternion.Euler(new Vector3(0f - leftHandAngle, 0f, leftHandAngle * -0.5f));
			leftIkTarget.localPosition = leftHandDefaultPos3 - new Vector3(0f, 0f, leftHandAngle * -0.0013f);
		}
	}

	private void FixedUpdate()
	{
	}

	private void OnAnimatorIK()
	{
		anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, ikWeightLeft1);
		anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, ikWeightRight1);
		anim.SetIKHintPosition(AvatarIKHint.LeftElbow, hintLeft.position);
		anim.SetIKHintPosition(AvatarIKHint.RightElbow, hintRight.position);
		anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeightLeft1);
		anim.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeightRight1);
		anim.SetIKPosition(AvatarIKGoal.LeftHand, leftIkTarget.position);
		anim.SetIKPosition(AvatarIKGoal.RightHand, rightIkTarget.position);
		anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeightLeft1);
		anim.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeightRight1);
		anim.SetIKRotation(AvatarIKGoal.LeftHand, leftIkTarget.rotation);
		anim.SetIKRotation(AvatarIKGoal.RightHand, rightIkTarget.rotation);
	}
}
