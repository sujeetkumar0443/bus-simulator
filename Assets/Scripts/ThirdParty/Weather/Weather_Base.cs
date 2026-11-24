using UnityEngine;

public class Weather_Base : MonoBehaviour
{
	protected Weather_Controller clWeatherController;

	[SerializeField]
	protected bool _bUseDifferentFadeTimes;

	[SerializeField]
	protected float _fFadeTime = 5f;

	[SerializeField]
	protected float _fSunriseFadeTime = 5f;

	[SerializeField]
	protected float _fDayFadeTime = 5f;

	[SerializeField]
	protected float _fSunsetFadeTime = 5f;

	[SerializeField]
	protected float _fNightFadeTime = 5f;

	[SerializeField]
	protected bool _bUseInit;

	protected float _fInitTimerStart;

	[SerializeField]
	protected float _fInitTimerEnd = 5f;

	protected bool _bGotAudioSource;

	[SerializeField]
	protected bool _bUsingSound;

	[SerializeField]
	protected AudioClip _adAmbientSound;

	protected bool _bTurnOffSoundAtExit;

	[SerializeField]
	protected float _fSoundVolume = 1f;

	protected float _fSoundVolumeIn;

	protected float _fSoundVolumeOut;

	[SerializeField]
	protected float _fTimeToFadeSound = 5f;

	[SerializeField]
	protected float _fLowestTemp;

	[SerializeField]
	protected float _fHighestTemp = 30f;

	[SerializeField]
	protected float _fSunrise_LightIntensity = 0.5f;

	[SerializeField]
	protected float _fDay_LightIntensity = 1f;

	[SerializeField]
	protected float _fSunset_LightIntensity = 0.5f;

	[SerializeField]
	protected float _fNight_LightIntensity = 0.3f;

	[SerializeField]
	protected Color _cSunrise_LightColor = Color.yellow;

	[SerializeField]
	protected Color _cDay_LightColor = Color.cyan;

	[SerializeField]
	protected Color _cSunset_LightColor = Color.red;

	[SerializeField]
	protected Color _cNight_LightColor = Color.grey;

	[SerializeField]
	protected float _fNight_MoonLightIntensity = 0.5f;

	[SerializeField]
	protected Color _cNight_MoonLightColor = Color.grey;

	[SerializeField]
	protected Color _cSunrise_SkyTintColor = Color.yellow;

	[SerializeField]
	protected Color _cDay_SkyTintColor = Color.cyan;

	[SerializeField]
	protected Color _cSunset_SkyTintColor = Color.red;

	[SerializeField]
	protected Color _cNight_SkyTintColor = Color.grey;

	[SerializeField]
	protected Color _cSunrise_SkyGroundColor = Color.yellow;

	[SerializeField]
	protected Color _cDay_SkyGroundColor = Color.cyan;

	[SerializeField]
	protected Color _cSunset_SkyGroundColor = Color.red;

	[SerializeField]
	protected Color _cNight_SkyGroundColor = Color.grey;

	[SerializeField]
	protected Color _cCloudColor = Color.white;

	[SerializeField]
	protected float _fFogAmount = 0.05f;

	[SerializeField]
	protected float _fFogMorningAmount = 0.05f;

	[SerializeField]
	protected bool _bUseMorningFog;

	[SerializeField]
	protected Color _cFogColor = Color.white;

	[SerializeField]
	protected GameObject _pSunriseParticle;

	[SerializeField]
	protected GameObject _pDayParticle;

	[SerializeField]
	protected GameObject _pSunsetParticle;

	[SerializeField]
	protected GameObject _pNightParticle;

	public bool GetSet_bUseDifferentFadeTimes
	{
		get
		{
			return _bUseDifferentFadeTimes;
		}
		set
		{
			_bUseDifferentFadeTimes = value;
		}
	}

	public float GetSet_fFadeTime
	{
		get
		{
			return _fFadeTime;
		}
		set
		{
			_fFadeTime = value;
		}
	}

	public float GetSet_fSunriseFadeTime
	{
		get
		{
			return _fSunriseFadeTime;
		}
		set
		{
			_fSunriseFadeTime = value;
		}
	}

	public float GetSet_fDayFadeTime
	{
		get
		{
			return _fDayFadeTime;
		}
		set
		{
			_fDayFadeTime = value;
		}
	}

	public float GetSet_fSunsetFadeTime
	{
		get
		{
			return _fSunsetFadeTime;
		}
		set
		{
			_fSunsetFadeTime = value;
		}
	}

	public float GetSet_fNightFadeTime
	{
		get
		{
			return _fNightFadeTime;
		}
		set
		{
			_fNightFadeTime = value;
		}
	}

	public bool GetSet_bUseInit
	{
		get
		{
			return _bUseInit;
		}
		set
		{
			_bUseInit = value;
		}
	}

	public float GetSet_fInitTimerEnd
	{
		get
		{
			return _fInitTimerEnd;
		}
		set
		{
			_fInitTimerEnd = value;
		}
	}

	public bool GetSet_bUsingSound
	{
		get
		{
			return _bUsingSound;
		}
		set
		{
			_bUsingSound = value;
		}
	}

	public AudioClip GetSet_adAmbientSound
	{
		get
		{
			return _adAmbientSound;
		}
		set
		{
			_adAmbientSound = value;
		}
	}

	public float GetSet_fSoundVolume
	{
		get
		{
			return _fSoundVolume;
		}
		set
		{
			_fSoundVolume = value;
		}
	}

	public float GetSet_fTimeToFadeSound
	{
		get
		{
			return _fTimeToFadeSound;
		}
		set
		{
			_fTimeToFadeSound = value;
		}
	}

	public float GetSet_fLowestTemp
	{
		get
		{
			return _fLowestTemp;
		}
		set
		{
			_fLowestTemp = value;
		}
	}

	public float GetSet_fHighestTemp
	{
		get
		{
			return _fHighestTemp;
		}
		set
		{
			_fHighestTemp = value;
		}
	}

	public float GetSet_fSunriseLightIntensity
	{
		get
		{
			return _fSunrise_LightIntensity;
		}
		set
		{
			_fSunrise_LightIntensity = value;
		}
	}

	public float GetSet_fDay_LightIntensity
	{
		get
		{
			return _fDay_LightIntensity;
		}
		set
		{
			_fDay_LightIntensity = value;
		}
	}

	public float GetSet_fSunset_LightIntensity
	{
		get
		{
			return _fSunset_LightIntensity;
		}
		set
		{
			_fSunset_LightIntensity = value;
		}
	}

	public float GetSet_fNight_LightIntensity
	{
		get
		{
			return _fNight_LightIntensity;
		}
		set
		{
			_fNight_LightIntensity = value;
		}
	}

	public Color GetSet_cSunrise_LightColor
	{
		get
		{
			return _cSunrise_LightColor;
		}
		set
		{
			_cSunrise_LightColor = value;
		}
	}

	public Color GetSet_cDay_LightColor
	{
		get
		{
			return _cDay_LightColor;
		}
		set
		{
			_cDay_LightColor = value;
		}
	}

	public Color GetSet_cSunset_LightColor
	{
		get
		{
			return _cSunset_LightColor;
		}
		set
		{
			_cSunset_LightColor = value;
		}
	}

	public Color GetSet_cNight_LightColor
	{
		get
		{
			return _cNight_LightColor;
		}
		set
		{
			_cNight_LightColor = value;
		}
	}

	public float GetSet_fNight_MoonLightIntensity
	{
		get
		{
			return _fNight_MoonLightIntensity;
		}
		set
		{
			_fNight_MoonLightIntensity = value;
		}
	}

	public Color GetSet_cNight_MoonLightColor
	{
		get
		{
			return _cNight_MoonLightColor;
		}
		set
		{
			_cNight_MoonLightColor = value;
		}
	}

	public Color GetSet_cSunrise_SkyTintColor
	{
		get
		{
			return _cSunrise_SkyTintColor;
		}
		set
		{
			_cSunrise_SkyTintColor = value;
		}
	}

	public Color GetSet_cDay_SkyTintColor
	{
		get
		{
			return _cDay_SkyTintColor;
		}
		set
		{
			_cDay_SkyTintColor = value;
		}
	}

	public Color GetSet_cSunset_SkyTintColor
	{
		get
		{
			return _cSunset_SkyTintColor;
		}
		set
		{
			_cSunset_SkyTintColor = value;
		}
	}

	public Color GetSet_cNight_SkyTintColor
	{
		get
		{
			return _cNight_SkyTintColor;
		}
		set
		{
			_cNight_SkyTintColor = value;
		}
	}

	public Color GetSet_cSunrise_SkyGroundColor
	{
		get
		{
			return _cSunrise_SkyGroundColor;
		}
		set
		{
			_cSunrise_SkyGroundColor = value;
		}
	}

	public Color GetSet_cDay_SkyGroundColor
	{
		get
		{
			return _cDay_SkyGroundColor;
		}
		set
		{
			_cDay_SkyGroundColor = value;
		}
	}

	public Color GetSet_cSunset_SkyGroundColor
	{
		get
		{
			return _cSunset_SkyGroundColor;
		}
		set
		{
			_cSunset_SkyGroundColor = value;
		}
	}

	public Color GetSet_cNight_SkyGroundColor
	{
		get
		{
			return _cNight_SkyGroundColor;
		}
		set
		{
			_cNight_SkyGroundColor = value;
		}
	}

	public Color GetSet_cCloudColor
	{
		get
		{
			return _cCloudColor;
		}
		set
		{
			_cCloudColor = value;
		}
	}

	public float GetSet_fFogAmount
	{
		get
		{
			return _fFogAmount;
		}
		set
		{
			_fFogAmount = value;
		}
	}

	public float GetSet_fFogMorning
	{
		get
		{
			return _fFogMorningAmount;
		}
		set
		{
			_fFogMorningAmount = value;
		}
	}

	public bool GetSet_bUseMorningFog
	{
		get
		{
			return _bUseMorningFog;
		}
		set
		{
			_bUseMorningFog = value;
		}
	}

	public Color GetSet_cFogColor
	{
		get
		{
			return _cFogColor;
		}
		set
		{
			_cFogColor = value;
		}
	}

	public GameObject GetSet_pSunriseParticle
	{
		get
		{
			return _pSunriseParticle;
		}
		set
		{
			_pSunriseParticle = value;
		}
	}

	public GameObject GetSet_pDayParticle
	{
		get
		{
			return _pDayParticle;
		}
		set
		{
			_pDayParticle = value;
		}
	}

	public GameObject GetSet_pSunsetParticle
	{
		get
		{
			return _pSunsetParticle;
		}
		set
		{
			_pSunsetParticle = value;
		}
	}

	public GameObject GetSet_pNightParticle
	{
		get
		{
			return _pNightParticle;
		}
		set
		{
			_pNightParticle = value;
		}
	}
	[System.Obsolete]
	public virtual void Init()
	{
		float getSet_fCurrTemp = Random.Range(_fLowestTemp, _fHighestTemp);
		GetComponent<Weather_Controller>().GetSet_fCurrTemp = getSet_fCurrTemp;
	}
	[System.Obsolete]
	public virtual void UpdateWeather()
	{
	}

	public virtual void TurnOnSound(GameObject gameobject)
	{
		if (gameobject != null)
		{
			if (gameobject.GetComponent<Weather_SoundFade>() != null && _bGotAudioSource)
			{
				gameobject.GetComponent<Weather_SoundFade>().FadeAudioIn(_fTimeToFadeSound, _fSoundVolumeIn);
				gameobject.GetComponent<AudioSource>().Play();
			}
			else if (gameobject.GetComponent<Weather_SoundFade>() == null && _bGotAudioSource)
			{
				Debug.LogError(string.Concat(gameobject.gameObject, " is missing the Weather_SoundFade component, we added it now"));
				gameobject.gameObject.AddComponent<Weather_SoundFade>();
				gameobject.GetComponent<Weather_SoundFade>().FadeAudioIn(_fTimeToFadeSound, _fSoundVolumeIn);
				gameobject.GetComponent<AudioSource>().Play();
			}
			else
			{
				Debug.Log("We got no audiosource for: " + gameobject);
			}
		}
		else
		{
			Debug.Log(string.Concat("There is no SoundEffect Gameobject attached to the WeatherController on type: ", clWeatherController.en_CurrWeather, " If you don't want to use sound on this weather type, set Using Ambient Sound to false!"));
		}
	}
	[System.Obsolete]
	public virtual void ExitWeatherEffect(GameObject gameobject)
	{
		if (gameobject.GetComponent<Weather_SoundFade>() != null && _bGotAudioSource)
		{
			gameobject.GetComponent<Weather_SoundFade>().FadeAudioOut(_fTimeToFadeSound, _fSoundVolumeOut);
		}
		else
		{
			Debug.LogError(string.Concat(gameobject, " is missing the Weather_SoundFade component!"));
		}
	}
}
