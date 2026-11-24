using UnityEngine;
using UnityEngine.UI;

namespace BusSimulator
{
	[RequireComponent(typeof(Image))]
	[AddComponentMenu("MiniMap/Inner map")]
	public class InnerMap : MonoBehaviour
	{
		private RectTransform _innerMapRect;

		public RectTransform InnerMapRect
		{
			get
			{
				if (!_innerMapRect)
				{
					_innerMapRect = GetComponent<RectTransform>();
				}
				return _innerMapRect;
			}
		}

		public float getMapRadius()
		{
			Vector3[] array = new Vector3[4];
			InnerMapRect.GetLocalCorners(array);
			if (Mathf.Abs(array[0].x) < Mathf.Abs(array[0].y))
			{
				return Mathf.Abs(array[0].x);
			}
			return Mathf.Abs(array[0].y);
		}
	}
}
