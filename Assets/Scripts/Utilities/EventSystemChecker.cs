using UnityEngine;
using UnityEngine.EventSystems;

namespace BusSimulator
{
	public class EventSystemChecker : MonoBehaviour
	{
		[System.Obsolete]
		private void Awake()
		{
			if (!Object.FindObjectOfType<EventSystem>())
			{
				GameObject gameObject = new GameObject("EventSystem");
				gameObject.AddComponent<EventSystem>();
				gameObject.AddComponent<StandaloneInputModule>().forceModuleActive = true;
			}
		}
	}
}
