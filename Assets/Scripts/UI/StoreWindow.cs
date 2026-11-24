using UnityEngine;
using UnityEngine.UI;

namespace BusSimulator
{
	public class StoreWindow : MonoBehaviour
	{
		public string[] itemIds;

		public int[] itemValues;

		public Text CashText;

		public string CashPreferenceName = "Cash";

		private void OnEnable()
		{
		}

		private void OnDisable()
		{
		}

		public void StoreItemsPurchase(string itemname, string itemId)
		{
			for (int i = 0; i < itemIds.Length; i++)
			{
				if (itemId == itemIds[i])
				{
					PlayerPrefs.SetInt(CashPreferenceName, PlayerPrefs.GetInt(CashPreferenceName) + itemValues[i]);
					CashText.text = PlayerPrefs.GetInt(CashPreferenceName).ToString();
					break;
				}
			}
		}
	}
}
