using UnityEngine;

namespace BusSimulator
{
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("MiniMap/Marker Group")]
	public class MarkerGroup : MonoBehaviour
	{
		private RectTransform _rectTransform;

		public RectTransform MarkerGroupRect
		{
			get
			{
				if (!_rectTransform)
				{
					_rectTransform = GetComponent<RectTransform>();
				}
				return _rectTransform;
			}
		}
	}
}
