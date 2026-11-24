using UnityEngine;
using UnityEngine.UI;

namespace BusSimulator
{
	// Keeps a Text showing the player's saved cash.
	public class CashLabel : MonoBehaviour
	{
		private Text label;

		private int shown = -1;

		private void Awake()
		{
			label = GetComponent<Text>();
		}

		private void OnEnable()
		{
			Refresh();
		}

		private void Update()
		{
			Refresh();
		}

		private void Refresh()
		{
			if (label == null)
			{
				return;
			}
			int cash = PlayerPrefs.GetInt("cash");
			if (cash != shown)
			{
				shown = cash;
				label.text = cash.ToString();
			}
		}
	}
}
