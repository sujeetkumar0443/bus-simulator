using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BusSimulator
{
	public class SpeedometerController : MonoBehaviour
	{
		public Image needleSprite;

		private float kph;

		public Text sppedText;

		private int speed;

		private void Start()
		{
			StartCoroutine(SpeedoMeter());
		}

		private IEnumerator SpeedoMeter()
		{
			while (true)
			{
				yield return new WaitForSeconds(0.05f);
				kph = GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
				needleSprite.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 0f - kph);
				speed = (int)kph;
				sppedText.text = speed.ToString();
			}
		}
	}
}
