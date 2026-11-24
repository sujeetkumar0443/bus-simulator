using UnityEngine;

namespace BusSimulator
{
	public class MainMenuController : MonoBehaviour
	{
		public SoundManager sound;

		public GameObject splash;

		public GameObject exitDialogue;

		public GameObject SettingDialoge;
 

		private void Update()
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				if (!exitDialogue.activeSelf)
				{
					exitDialogue.SetActive(true);
				}
				else
				{
					exitDialogue.SetActive(false);
				}
			}
		}

		public void ButtonReleased(GameObject obj)
		{
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		[System.Obsolete]
		public void PlayClicked(GameObject obj)
		{
			sound.ButtonClickSound();
			Application.LoadLevel(1);
		}

		public void MoreApps(GameObject obj)
		{
			sound.ButtonClickSound();
	 
		}

		public void Feedback(GameObject obj)
		{
			sound.ButtonClickSound();
		 
		}

		public void YesButton()
		{
			Application.Quit();
		}

		public void ExitButton()
		{
			exitDialogue.SetActive(true);
			AdManager.Instance.ShowInterstitial();
		}

		public void NoButton()
		{
			exitDialogue.SetActive(false);
		}

		public void SettingDialoge_Cancle()
		{
			sound.ButtonClickSound();
			SettingDialoge.SetActive(false);
		}

		public void SettngBtn()
		{
			sound.ButtonClickSound();
			SettingDialoge.SetActive(true);
		}

		public void OpenOtherUrl(string url)
		{
			sound.ButtonClickSound();
			Application.OpenURL(url);
		}
	}
}
