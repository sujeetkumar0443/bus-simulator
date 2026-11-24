using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("MiniMap/Map marker")]
public class MapMarker : MonoBehaviour
{
	public Sprite markerSprite;

	public float markerSize = 6.5f;

	public bool isActive = true;

	private Image markerImage;

	public Image MarkerImage
	{
		get
		{
			return markerImage;
		}
	}

	private void Start()
	{
		if (!markerSprite)
		{
			Debug.LogError(" Please, specify the marker sprite.");
		}
		GameObject gameObject = new GameObject("Marker");
		gameObject.AddComponent<Image>();
		MapCanvasController instance = MapCanvasController.Instance;
		if (!instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		gameObject.transform.SetParent(instance.MarkerGroup.MarkerGroupRect);
		markerImage = gameObject.GetComponent<Image>();
		markerImage.sprite = markerSprite;
		markerImage.rectTransform.localPosition = Vector3.zero;
		markerImage.rectTransform.localScale = Vector3.one;
		markerImage.rectTransform.sizeDelta = new Vector2(markerSize, markerSize);
		markerImage.gameObject.SetActive(false);
	}

	private void Update()
	{
		MapCanvasController instance = MapCanvasController.Instance;
		if ((bool)instance)
		{
			MapCanvasController.Instance.checkIn(this);
			markerImage.rectTransform.rotation = Quaternion.identity;
		}
	}

	private void OnDestroy()
	{
		if ((bool)markerImage)
		{
			Object.Destroy(markerImage.gameObject);
		}
	}

	public void show()
	{
		markerImage.gameObject.SetActive(true);
	}

	public void hide()
	{
		markerImage.gameObject.SetActive(false);
	}

	public bool isVisible()
	{
		return markerImage.gameObject.activeSelf;
	}

	public Vector3 getPosition()
	{
		return base.gameObject.transform.position;
	}

	public void setLocalPos(Vector3 pos)
	{
		markerImage.rectTransform.localPosition = pos;
	}

	public void setOpacity(float opacity)
	{
		markerImage.color = new Color(1f, 1f, 1f, opacity);
	}
}
