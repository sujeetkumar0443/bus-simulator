using UnityEngine;
using UnityEngine.EventSystems;

namespace BusSimulator
{
	// Makes a HUD button visibly "press in" while a finger or mouse is held on it.
	[DefaultExecutionOrder(200)]
	public class PressFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
	{
		public RectTransform target;

		public float pressedScale = 0.88f;

		public float speed = 14f;

		private bool pressed;

		private float current = 1f;

		private Vector3 baseScale = Vector3.one;

		private RectTransform Target
		{
			get
			{
				return target != null ? target : (RectTransform)transform;
			}
		}

		private void Awake()
		{
			baseScale = Target.localScale;
		}

		private void OnEnable()
		{
			pressed = false;
			current = 1f;
			Target.localScale = baseScale;
		}

		private void OnDisable()
		{
			pressed = false;
			current = 1f;
			Target.localScale = baseScale;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			pressed = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			pressed = false;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			pressed = false;
		}

		private void LateUpdate()
		{
			float goal = pressed ? pressedScale : 1f;
			current = Mathf.MoveTowards(current, goal, speed * Time.unscaledDeltaTime);
			Target.localScale = baseScale * current;
		}
	}
}
