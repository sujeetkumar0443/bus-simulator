using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public AudioSource source1;

	public AudioSource source2;

	public AudioClip buttonclick;

	public AudioClip HornSoundclip;

	public AudioClip screamSoundClip;

	public AudioClip blinksound;

	public AudioClip[] radiosounds;

	private int count;

	public AudioClip busdoorOpen;

	private void OnEnable()
	{
		GameplayUIController.BusDoorEvent = (Action<bool>)Delegate.Combine(GameplayUIController.BusDoorEvent, new Action<bool>(BusDoorOpenSound));
	}

	private void OnDisable()
	{
		GameplayUIController.BusDoorEvent = (Action<bool>)Delegate.Remove(GameplayUIController.BusDoorEvent, new Action<bool>(BusDoorOpenSound));
	}

	private void Start()
	{
		Debug.Log("sounds in soundmanager : " + PlayerPrefs.GetInt("sounds"));
		Debug.Log("music in soundmanager : " + PlayerPrefs.GetInt("music"));
		AudioListener.volume = 1f;
		if (PlayerPrefs.GetInt("sounds") == 1)
		{
			PauseSound(false);
		}
		if (PlayerPrefs.GetInt("music") == 1)
		{
			PauseMusic(false);
		}
	}

	public void PauseSound(bool isStop)
	{
		source2.enabled = isStop;
	}

	public void PauseMusic(bool isStop)
	{
		source1.enabled = isStop;
	}

	public void pauseSounds(bool isPause)
	{
		source1.enabled = isPause;
		source2.enabled = isPause;
	}

	public void ButtonClickSound()
	{
		if (PlayerPrefs.GetInt("sounds") != 1)
		{
			source2.PlayOneShot(buttonclick);
		}
	}

	public void HornSound()
	{
		if (PlayerPrefs.GetInt("sounds") != 1)
		{
			source2.clip = HornSoundclip;
			source2.Play();
		}
	}

	public void HornSoundoff()
	{
		if (PlayerPrefs.GetInt("sounds") != 1)
		{
			source2.clip = HornSoundclip;
			source2.Stop();
		}
	}

	public void signalBlinkSounds()
	{
		if (PlayerPrefs.GetInt("sounds") != 1)
		{
			source2.PlayOneShot(blinksound);
		}
	}

	public void RadioSounds()
	{
		if (PlayerPrefs.GetInt("music") != 1)
		{
			if (count < radiosounds.Length)
			{
				source1.clip = radiosounds[count];
				source1.Play();
				count++;
			}
			else
			{
				count = 0;
			}
		}
	}

	public void BusDoorOpenSound(bool temp)
	{
		if (PlayerPrefs.GetInt("sounds") != 1)
		{
			source2.PlayOneShot(busdoorOpen);
		}
	}

	public void BusHitSound()
	{
		if (PlayerPrefs.GetInt("sounds") != 1 && !source2.isPlaying)
		{
			CancelInvoke("ResetVolume");
			source2.volume = 0.35f;
			source2.PlayOneShot(screamSoundClip);
			Invoke("ResetVolume", 1f);
		}
	}

	private void ResetVolume()
	{
		source2.volume = 1f;
	}
}
