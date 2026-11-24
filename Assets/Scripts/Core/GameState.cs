using UnityEngine;

namespace BusSimulator
{
	public class GameState : MonoBehaviour
	{
		public static bool gameOver;

		public static bool LevelOver;

		public static bool collectAllCoins;

		public static bool CompleteBeforeTime;

		public static bool vehicleparked;

		public static bool starttime;

		public static bool tiltcontrol;

		public static bool steercontrol;

		private void Start()
		{
			steercontrol = false;
			tiltcontrol = false;
			collectAllCoins = false;
			CompleteBeforeTime = false;
			vehicleparked = false;
			gameOver = false;
			LevelOver = false;
			starttime = false;
		}
	}
}
