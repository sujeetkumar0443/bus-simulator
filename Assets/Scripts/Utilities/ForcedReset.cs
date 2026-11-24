using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

namespace BusSimulator
{
	[RequireComponent(typeof(Image))]
	public class ForcedReset : MonoBehaviour
	{
		private void Update()
		{
			if (CrossPlatformInputManager.GetButtonDown("ResetObject"))
			{
				SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
			}
		}
	}
}
