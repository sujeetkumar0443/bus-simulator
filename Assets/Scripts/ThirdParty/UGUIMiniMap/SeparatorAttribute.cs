using UnityEngine;

namespace UGUIMiniMap
{
	public class SeparatorAttribute : PropertyAttribute
	{
		public readonly string title;

		public SeparatorAttribute()
		{
			title = string.Empty;
		}

		public SeparatorAttribute(string _title)
		{
			title = _title;
		}
	}
}
