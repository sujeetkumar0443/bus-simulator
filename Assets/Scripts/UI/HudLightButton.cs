using UnityEngine;
using UnityEngine.EventSystems;

namespace BusSimulator
{
	// Finds the bus the player is driving, even if the RCC scene manager has not registered it.
	public static class PlayerBus
	{
		private static RCC_CarControllerV3 cached;

		private static float nextLookup;

		public static RCC_CarControllerV3 Get()
		{
			if (cached != null && cached.gameObject.activeInHierarchy)
			{
				return cached;
			}
			if (Time.unscaledTime < nextLookup)
			{
				return null;
			}
			nextLookup = Time.unscaledTime + 0.25f;
			cached = null;
			RCC_SceneManager manager = RCC_SceneManager.Instance;
			if (manager != null && manager.activePlayerVehicle != null && manager.activePlayerVehicle.gameObject.activeInHierarchy)
			{
				cached = manager.activePlayerVehicle;
				return cached;
			}
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player != null)
			{
				cached = player.GetComponent<RCC_CarControllerV3>();
			}
			if (cached == null)
			{
				cached = Object.FindObjectOfType<RCC_CarControllerV3>();
			}
			return cached;
		}
	}

	// Left / right / hazard indicators and headlights for the new HUD buttons.
	public class HudLightButton : MonoBehaviour, IPointerClickHandler
	{
		public enum Kind
		{
			Left = 0,
			Right = 1,
			Hazard = 2,
			Headlight = 3
		}

		public Kind kind;

		public void OnPointerClick(PointerEventData eventData)
		{
			RCC_CarControllerV3 car = PlayerBus.Get();
			if (car == null)
			{
				return;
			}
			switch (kind)
			{
			case Kind.Left:
				car.indicatorsOn = (car.indicatorsOn != RCC_CarControllerV3.IndicatorsOn.Left) ? RCC_CarControllerV3.IndicatorsOn.Left : RCC_CarControllerV3.IndicatorsOn.Off;
				break;
			case Kind.Right:
				car.indicatorsOn = (car.indicatorsOn != RCC_CarControllerV3.IndicatorsOn.Right) ? RCC_CarControllerV3.IndicatorsOn.Right : RCC_CarControllerV3.IndicatorsOn.Off;
				break;
			case Kind.Hazard:
				car.indicatorsOn = (car.indicatorsOn != RCC_CarControllerV3.IndicatorsOn.All) ? RCC_CarControllerV3.IndicatorsOn.All : RCC_CarControllerV3.IndicatorsOn.Off;
				break;
			case Kind.Headlight:
				if (car.lowBeamHeadLightsOn || car.highBeamHeadLightsOn)
				{
					car.lowBeamHeadLightsOn = false;
					car.highBeamHeadLightsOn = false;
				}
				else
				{
					car.lowBeamHeadLightsOn = true;
				}
				break;
			}
		}
	}
}
