using UnityEngine;
using UnityEngine.UI;

namespace BusSimulator
{
	// Shows indicator / headlight state on the new HUD icons: bright (blinking for indicators) when on, dim when off.
	[DefaultExecutionOrder(300)]
	public class HudStateTint : MonoBehaviour
	{
		public enum Kind
		{
			Left = 0,
			Right = 1,
			Headlight = 2
		}

		public Kind kind;

		private Image image;

		private void Awake()
		{
			image = GetComponent<Image>();
		}

		private void LateUpdate()
		{
			if (image == null)
			{
				return;
			}
			RCC_SceneManager manager = RCC_SceneManager.Instance;
			if (manager == null || manager.activePlayerVehicle == null)
			{
				return;
			}
			RCC_CarControllerV3 car = manager.activePlayerVehicle;
			bool on = false;
			bool blink = false;
			switch (kind)
			{
			case Kind.Left:
				on = car.indicatorsOn == RCC_CarControllerV3.IndicatorsOn.Left || car.indicatorsOn == RCC_CarControllerV3.IndicatorsOn.All;
				blink = true;
				break;
			case Kind.Right:
				on = car.indicatorsOn == RCC_CarControllerV3.IndicatorsOn.Right || car.indicatorsOn == RCC_CarControllerV3.IndicatorsOn.All;
				blink = true;
				break;
			case Kind.Headlight:
				on = car.lowBeamHeadLightsOn || car.highBeamHeadLightsOn;
				break;
			}
			float b = 0.45f;
			if (on)
			{
				b = 1f;
				if (blink)
				{
					b = Mathf.Lerp(0.6f, 1f, Mathf.Sin(Time.unscaledTime * 9f) * 0.5f + 0.5f);
				}
			}
			image.color = new Color(b, b, b, 1f);
		}
	}
}
