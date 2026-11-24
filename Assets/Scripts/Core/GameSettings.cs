using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
	public SoundManager sound;

	public GameObject soundActive;

	public GameObject soundInActive;

	public GameObject musicActive;

	public GameObject musicInActive;

	public Slider tiltslider;

	public GameObject Manual;

	public GameObject Automatic;

	public GameObject ShadowOn;

	public GameObject ShadowOff;

	private void Start()
	{
		SetDefaultPrefs();
		if (PlayerPrefs.GetInt("sounds") == 1)
		{
			soundActive.SetActive(false);
			soundInActive.SetActive(true);
			sound.PauseSound(false);
		}
		else
		{
			soundActive.SetActive(true);
			soundInActive.SetActive(false);
			sound.PauseSound(true);
		}
		if (PlayerPrefs.GetInt("music") == 1)
		{
			musicActive.SetActive(false);
			musicInActive.SetActive(true);
			sound.PauseMusic(false);
		}
		else
		{
			musicActive.SetActive(true);
			musicInActive.SetActive(false);
			sound.PauseMusic(true);
		}
		if (PlayerPrefs.GetInt("shadows") == 1)
		{
			ShadowOn.SetActive(true);
			ShadowOff.SetActive(false);
			QualitySettings.SetQualityLevel(4);
		}
		else
		{
			ShadowOn.SetActive(false);
			ShadowOff.SetActive(true);
			QualitySettings.SetQualityLevel(3);
		}
		tiltslider.value = PlayerPrefs.GetFloat("tiltsens");
		if (PlayerPrefs.GetInt("transmission") == 1)
		{
			Automatic.SetActive(true);
			Manual.SetActive(false);
		}
		else
		{
			Automatic.SetActive(false);
			Manual.SetActive(true);
		}
	}

	private void SetDefaultPrefs()
	{
		if (!PlayerPrefs.HasKey("sounds"))
		{
			PlayerPrefs.SetInt("sounds", 0);
		}
		if (!PlayerPrefs.HasKey("music"))
		{
			PlayerPrefs.SetInt("music", 0);
		}
		if (!PlayerPrefs.HasKey("tiltsens"))
		{
			PlayerPrefs.SetFloat("tiltsens", 1f);
		}
		if (!PlayerPrefs.HasKey("transmission"))
		{
			PlayerPrefs.SetInt("transmission", 1);
		}
		if (!PlayerPrefs.HasKey("shadows"))
		{
			PlayerPrefs.SetInt("shadows", 0);
			QualitySettings.SetQualityLevel(3);
		}
	}

	public void SoundNext()
	{
		Debug.Log("sound pref in fun: " + PlayerPrefs.GetInt("sounds"));
		if (PlayerPrefs.GetInt("sounds") == 1)
		{
			soundActive.SetActive(true);
			soundInActive.SetActive(false);
			PlayerPrefs.SetInt("sounds", 0);
			sound.PauseSound(true);
		}
		else
		{
			soundActive.SetActive(false);
			soundInActive.SetActive(true);
			PlayerPrefs.SetInt("sounds", 1);
			sound.PauseSound(false);
		}
	}

	public void MusicNext()
	{
		Debug.Log("music pref in fun: " + PlayerPrefs.GetInt("music"));
		if (PlayerPrefs.GetInt("music") == 1)
		{
			musicActive.SetActive(true);
			musicInActive.SetActive(false);
			PlayerPrefs.SetInt("music", 0);
			sound.PauseMusic(true);
		}
		else
		{
			musicActive.SetActive(false);
			musicInActive.SetActive(true);
			PlayerPrefs.SetInt("music", 1);
			sound.PauseMusic(false);
		}
	}

	public void TiltSensitivity()
	{
		PlayerPrefs.SetFloat("tiltsens", tiltslider.value);
	}

	public void TransmissionControl()
	{
		Debug.Log("trans pref in fun: " + PlayerPrefs.GetInt("transmission"));
		if (Manual.activeSelf)
		{
			Automatic.SetActive(true);
			Manual.SetActive(false);
			PlayerPrefs.SetInt("transmission", 1);
		}
		else
		{
			Automatic.SetActive(false);
			Manual.SetActive(true);
			PlayerPrefs.SetInt("transmission", 2);
		}
	}

	public void ShadowNext()
	{
		Debug.Log("shadows pref in fun: " + PlayerPrefs.GetInt("shadows"));
		if (PlayerPrefs.GetInt("shadows") == 1)
		{
			ShadowOn.SetActive(false);
			ShadowOff.SetActive(true);
			QualitySettings.SetQualityLevel(3);
			PlayerPrefs.SetInt("shadows", 0);
		}
		else
		{
			ShadowOn.SetActive(true);
			ShadowOff.SetActive(false);
			QualitySettings.SetQualityLevel(4);
			PlayerPrefs.SetInt("shadows", 1);
		}
	}
}
