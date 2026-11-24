using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("MiniMap/Map canvas controller")]
public class MapCanvasController : MonoBehaviour
{
	private static MapCanvasController _instance;

	public Transform playerTransform;

	public float radarDistance = 10f;

	public bool hideOutOfRadius = true;

	public bool useOpacity = true;

	public float maxRadarDistance = 10f;

	public bool rotateMap;

	public float scale = 1f;

	public float minimalOpacity = 0.3f;

	private RectTransform mapRect;

	private InnerMap innerMap;

	private MapArrow mapArrow;

	private MarkerGroup markerGroup;

	private float innerMapRadius;

	public static MapCanvasController Instance
	{
		get
		{
			if (!_instance)
			{
				MapCanvasController[] array = Object.FindObjectsOfType<MapCanvasController>();
				if (array.Length != 0)
				{
					if (array.Length == 1)
					{
						_instance = array[0];
					}
					else
					{
						Debug.LogError("You have more than one MapCanvasController in the scene.");
					}
				}
				else
				{
					Debug.LogError("You should add Map prefab to your canvas");
				}
			}
			return _instance;
		}
	}

	public InnerMap InnerMapComponent
	{
		get
		{
			return innerMap;
		}
	}

	public MarkerGroup MarkerGroup
	{
		get
		{
			return markerGroup;
		}
	}

	private void Awake()
	{
		if (!playerTransform)
		{
			Debug.LogError("You must specify the player transform");
		}
		mapRect = GetComponent<RectTransform>();
		innerMap = GetComponentInChildren<InnerMap>();
		if (!innerMap)
		{
			Debug.LogError("InnerMap component is missing from children");
		}
		mapArrow = GetComponentInChildren<MapArrow>();
		if (!mapArrow)
		{
			Debug.LogError("MapArrow component is missing from children");
		}
		markerGroup = GetComponentInChildren<MarkerGroup>();
		if (!markerGroup)
		{
			Debug.LogError("MerkerGroup component is missing. It must be a child of InnerMap");
		}
		innerMapRadius = innerMap.getMapRadius();
	}

	private void Update()
	{
		if ((bool)playerTransform)
		{
			if (rotateMap)
			{
				mapRect.rotation = Quaternion.Euler(new Vector3(0f, 0f, playerTransform.eulerAngles.y));
				mapArrow.rotate(Quaternion.identity);
			}
			else
			{
				mapArrow.rotate(Quaternion.Euler(new Vector3(0f, 0f, 0f - playerTransform.eulerAngles.y)));
			}
		}
	}

	public void checkIn(MapMarker marker)
	{
		if (!playerTransform)
		{
			return;
		}
		float num = radarDistance * scale;
		float num2 = maxRadarDistance * scale;
		if (marker.isActive)
		{
			float num3 = DistanceToPlayer(marker.getPosition());
			float num4 = 1f;
			if (num3 > num)
			{
				if (hideOutOfRadius)
				{
					if (marker.isVisible())
					{
						marker.hide();
					}
					return;
				}
				if (num3 > num2)
				{
					if (marker.isVisible())
					{
						marker.hide();
					}
					return;
				}
				if (useOpacity)
				{
					float num5 = num2 - num;
					if (num5 <= 0f)
					{
						Debug.LogError("Max radar distance should be bigger than radar distance");
						return;
					}
					float num6 = num3 - num;
					num4 = 1f - num6 / num5;
					if (num4 < minimalOpacity)
					{
						num4 = minimalOpacity;
					}
				}
				num3 = num;
			}
			if (!marker.isVisible())
			{
				marker.show();
			}
			Vector3 vector = marker.getPosition() - playerTransform.position;
			Vector3 localPos = new Vector3(vector.x, vector.z, 0f);
			localPos.Normalize();
			float num7 = marker.markerSize / 2f;
			float num8 = num3 / num * (innerMapRadius - num7);
			localPos *= num8;
			marker.setLocalPos(localPos);
			marker.setOpacity(num4);
		}
		else if (marker.isVisible())
		{
			marker.hide();
		}
	}

	private float DistanceToPlayer(Vector3 other)
	{
		return Vector2.Distance(new Vector2(playerTransform.position.x, playerTransform.position.z), new Vector2(other.x, other.z));
	}
}
