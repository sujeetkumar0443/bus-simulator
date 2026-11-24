using UnityEngine;

namespace BusSimulator
{
	// Sweeps a soft light streak across a masked UI button every few seconds.
	public class ShineEffect : MonoBehaviour
	{
		public RectTransform shine;

		public float sweepDuration = 0.7f;

		public float pauseDuration = 2.3f;

		private RectTransform area;

		private void Awake()
		{
			area = shine.parent as RectTransform;
			shine.gameObject.SetActive(false);
		}

		private void Update()
		{
			float cycle = sweepDuration + pauseDuration;
			float t = Time.unscaledTime % cycle;
			if (t > sweepDuration)
			{
				if (shine.gameObject.activeSelf)
				{
					shine.gameObject.SetActive(false);
				}
				return;
			}
			if (!shine.gameObject.activeSelf)
			{
				shine.gameObject.SetActive(true);
			}
			float half = area.rect.width * 0.5f + shine.rect.width;
			shine.anchoredPosition = new Vector2(Mathf.Lerp(0f - half, half, t / sweepDuration), 0f);
		}
	}
}
