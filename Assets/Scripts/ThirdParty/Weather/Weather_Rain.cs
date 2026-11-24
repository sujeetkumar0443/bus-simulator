using UnityEngine;

public class Weather_Rain : Weather_Base
{
	[SerializeField]
	private GameObject _gPartRain;

	private float _fEndParticleTimerStart;

	private float _fEndParticleTimerEnd;

	public GameObject GetSet_gPartRain
	{
		get
		{
			return _gPartRain;
		}
		set
		{
			_gPartRain = value;
		}
	}

	private void Start()
	{
		clWeatherController = (Weather_Controller)GetComponent(typeof(Weather_Controller));
		if (!_bUseMorningFog)
		{
			_fFogMorningAmount = _fFogAmount;
		}
		_fSoundVolumeIn = _fSoundVolume;
		_fSoundVolumeOut = _fSoundVolume;
		_fEndParticleTimerStart = 0f;
		_fEndParticleTimerEnd = 5f;
		if (!_bUsingSound)
		{
			return;
		}
		if (_adAmbientSound != null)
		{
			if (_gPartRain.GetComponent<AudioSource>() != null)
			{
				_bGotAudioSource = true;
				_gPartRain.GetComponent<AudioSource>().clip = _adAmbientSound;
				_gPartRain.GetComponent<AudioSource>().volume = 0f;
				_gPartRain.GetComponent<AudioSource>().loop = true;
			}
			else
			{
				_gPartRain.AddComponent<AudioSource>();
				_gPartRain.GetComponent<AudioSource>().clip = _adAmbientSound;
				_gPartRain.GetComponent<AudioSource>().volume = 0f;
				_gPartRain.GetComponent<AudioSource>().loop = true;
				Debug.LogWarning(string.Concat("There was no AUDIOSOURCE on ", _gPartRain, " this is now added"));
				_bGotAudioSource = true;
			}
		}
		else
		{
			Debug.Log(string.Concat("There is no AMBIENT SOUND attached to the WeatherController on type: ", clWeatherController.en_CurrWeather, " If you don't want to use Ambient sound on this weather type, set Using Ambient Sound to false!"));
		}
	}
	[System.Obsolete]
	public override void Init()
	{
		base.Init();
		TurnOnRain();
		if (_gPartRain != null)
		{
			_gPartRain.GetComponent<ParticleSystem>().enableEmission = true;
		}
	}
	[System.Obsolete]
	private void Update()
	{
		UpdateWeather();
		if (_bUseInit)
		{
			_fInitTimerStart += Time.deltaTime;
			if (_fInitTimerStart >= _fInitTimerEnd)
			{
				Init();
				_fInitTimerStart = 0f;
				_bUseInit = false;
			}
		}
	}
	[System.Obsolete]
	public override void UpdateWeather()
	{
		if (!_bUseDifferentFadeTimes)
		{
			OneFadeTimeToRuleThemAll();
		}
		else
		{
			DifferentFadeTimes();
		}
	}
	[System.Obsolete]
	private void OneFadeTimeToRuleThemAll()
	{
		if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.SUNRISE)
		{
			clWeatherController.UpdateAllWeather(_fSunrise_LightIntensity, _cSunrise_LightColor, 0f, _cNight_MoonLightColor, _cSunrise_SkyTintColor, _cSunrise_SkyGroundColor, _cCloudColor, _fFogMorningAmount, _cFogColor, _fFadeTime);
			clWeatherController.DeactivateTimesetParticle(_pNightParticle);
			clWeatherController.ActivateTimesetParticle(_pSunriseParticle);
		}
		else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.DAY)
		{
			clWeatherController.UpdateAllWeather(_fDay_LightIntensity, _cDay_LightColor, 0f, _cNight_MoonLightColor, _cDay_SkyTintColor, _cDay_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fFadeTime);
			clWeatherController.DeactivateTimesetParticle(_pSunriseParticle);
			clWeatherController.ActivateTimesetParticle(_pDayParticle);
		}
		else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.SUNSET)
		{
			clWeatherController.UpdateAllWeather(_fSunset_LightIntensity, _cSunset_LightColor, 0f, _cNight_MoonLightColor, _cSunset_SkyTintColor, _cSunset_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fFadeTime);
			clWeatherController.DeactivateTimesetParticle(_pDayParticle);
			clWeatherController.ActivateTimesetParticle(_pSunsetParticle);
		}
		else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.NIGHT)
		{
			clWeatherController.UpdateAllWeather(_fNight_LightIntensity, _cNight_LightColor, _fNight_MoonLightIntensity, _cNight_MoonLightColor, _cNight_SkyTintColor, _cNight_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fFadeTime);
			clWeatherController.DeactivateTimesetParticle(_pSunsetParticle);
			clWeatherController.ActivateTimesetParticle(_pNightParticle);
		}
	}
	[System.Obsolete]
	private void DifferentFadeTimes()
	{
		if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.SUNRISE)
		{
			clWeatherController.UpdateAllWeather(_fSunrise_LightIntensity, _cSunrise_LightColor, 0f, _cNight_MoonLightColor, _cSunrise_SkyTintColor, _cSunrise_SkyGroundColor, _cCloudColor, _fFogMorningAmount, _cFogColor, _fSunriseFadeTime);
			clWeatherController.DeactivateTimesetParticle(_pNightParticle);
			clWeatherController.ActivateTimesetParticle(_pSunriseParticle);
		}
		else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.DAY)
		{
			clWeatherController.UpdateAllWeather(_fDay_LightIntensity, _cDay_LightColor, 0f, _cNight_MoonLightColor, _cDay_SkyTintColor, _cDay_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fDayFadeTime);
			clWeatherController.DeactivateTimesetParticle(_pSunriseParticle);
			clWeatherController.ActivateTimesetParticle(_pDayParticle);
		}
		else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.SUNSET)
		{
			clWeatherController.UpdateAllWeather(_fSunset_LightIntensity, _cSunset_LightColor, 0f, _cNight_MoonLightColor, _cSunset_SkyTintColor, _cSunset_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fSunsetFadeTime);
			clWeatherController.DeactivateTimesetParticle(_pDayParticle);
			clWeatherController.ActivateTimesetParticle(_pSunsetParticle);
		}
		else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.NIGHT)
		{
			clWeatherController.UpdateAllWeather(_fNight_LightIntensity, _cNight_LightColor, _fNight_MoonLightIntensity, _cNight_MoonLightColor, _cNight_SkyTintColor, _cNight_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fNightFadeTime);
			clWeatherController.DeactivateTimesetParticle(_pSunsetParticle);
			clWeatherController.ActivateTimesetParticle(_pNightParticle);
		}
	}

	private void TurnOnRain()
	{
		if (_gPartRain != null)
		{
			if (!_gPartRain.activeInHierarchy && _gPartRain != null)
			{
				_gPartRain.SetActive(true);
				if (_bUsingSound)
				{
					TurnOnSound(_gPartRain);
				}
			}
		}
		else
		{
			Debug.Log(string.Concat("We are missing rain particles on: ", base.gameObject, " For weather type: RAIN"));
		}
	}

	public override void TurnOnSound(GameObject gameobject)
	{
		base.TurnOnSound(gameobject);
		_bTurnOffSoundAtExit = true;
	}
	[System.Obsolete]
	public override void ExitWeatherEffect(GameObject gameobject)
	{
		clWeatherController.DeactivateTimesetParticle(_pSunriseParticle);
		clWeatherController.DeactivateTimesetParticle(_pDayParticle);
		clWeatherController.DeactivateTimesetParticle(_pSunsetParticle);
		clWeatherController.DeactivateTimesetParticle(_pNightParticle);
		if (!(_gPartRain != null) || !_gPartRain.activeInHierarchy)
		{
			return;
		}
		_fEndParticleTimerStart += Time.deltaTime;

		_gPartRain.GetComponent<ParticleSystem>().enableEmission = false;
		if (_bTurnOffSoundAtExit)
		{
			if (_bUsingSound)
			{
				base.ExitWeatherEffect(gameobject);
			}
			_bTurnOffSoundAtExit = false;
		}
		if (_fEndParticleTimerStart > _fEndParticleTimerEnd)
		{
			_fEndParticleTimerStart = 0f;
			_gPartRain.SetActive(false);
		}
	}
}
