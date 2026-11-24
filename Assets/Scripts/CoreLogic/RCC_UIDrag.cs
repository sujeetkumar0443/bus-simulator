using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/RCC UI Drag Handler")]
public class RCC_UIDrag : MonoBehaviour, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	

	public void OnDrag(PointerEventData data)
	{
		 
		RCC_SceneManager.Instance.activePlayerCamera.OnDrag(data);
	}

	public void OnEndDrag(PointerEventData data)
	{
		 
	}
}
