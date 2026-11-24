using System.Collections;
using UnityEngine;

public class Weather_Thunderstorm : Weather_Base
{
	[SerializeField]
	private GameObject _gPartRain;

	private GameObject _gLighting;

	public Color _lighteneingColor;

	private float _fEndParticleTimerStart;

	private float _fEndParticleTimerEnd;

	private float _fTimerForNextLighting;

	private float _fTimeForNextLighting;

	[SerializeField]
	private float _fRandomThunderTimeMin = 30f;

	[SerializeField]
	private float _fRandomThunderTimeMax = 240f;

	[SerializeField]
	private float _fLightningIntensity = 5f;

	[SerializeField]
	private float _fLightningRange = 500f;

	[SerializeField]
	private float _fTimeBeforeLightningLightTurnsOff = 0.4f;

	[SerializeField]
	private AudioClip _adThunderSound;

	[SerializeField]
	private float _fLightningVolume = 1f;

	[SerializeField]
	private float _fLightningRangeFromWeatherMasterMin = 10f;

	[SerializeField]
	private float _fLightningRangeFromWeatherMasterMax = 10f;

	[SerializeField]
	private float _fLightningHeight = 50f;

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

	public float GetSet_RandomThunderTimeMin
	{
		get
		{
			return _fRandomThunderTimeMin;
		}
		set
		{
			_fRandomThunderTimeMin = value;
		}
	}

	public float GetSet_RandomThunderTimeMax
	{
		get
		{
			return _fRandomThunderTimeMax;
		}
		set
		{
			_fRandomThunderTimeMax = value;
		}
	}

	public float GetSet_fLightningIntensity
	{
		get
		{
			return _fLightningIntensity;
		}
		set
		{
			_fLightningIntensity = value;
		}
	}

	public float GetSet_fLightningRange
	{
		get
		{
			return _fLightningRange;
		}
		set
		{
			_fLightningRange = value;
		}
	}

	public float GetSet_fTimeBeforeLightningLightTurnsOff
	{
		get
		{
			return _fTimeBeforeLightningLightTurnsOff;
		}
		set
		{
			_fTimeBeforeLightningLightTurnsOff = value;
		}
	}

	public AudioClip GetSet_adThunderSound
	{
		get
		{
			return _adThunderSound;
		}
		set
		{
			_adThunderSound = value;
		}
	}

	public float GetSet_fLightningVolume
	{
		get
		{
			return _fLightningVolume;
		}
		set
		{
			_fLightningVolume = value;
		}
	}

	public float GetSet_fLightningRangeFromWeatherMasterMin
	{
		get
		{
			return _fLightningRangeFromWeatherMasterMin;
		}
		set
		{
			_fLightningRangeFromWeatherMasterMin = value;
		}
	}

	public float GetSet_fLightningRangeFromWeatherMasterMax
	{
		get
		{
			return _fLightningRangeFromWeatherMasterMax;
		}
		set
		{
			_fLightningRangeFromWeatherMasterMax = value;
		}
	}

	public float GetSet_fLightningHeight
	{
		get
		{
			return _fLightningHeight;
		}
		set
		{
			_fLightningHeight = value;
		}
	}

	public float Get_TimerForNextLighting
	{
		get
		{
			return _fTimerForNextLighting;
		}
	}

	public float Get_TimeBeforeNextLighting
	{
		get
		{
			return _fTimeForNextLighting;
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
		if (_bUsingSound)
		{
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
		_fTimeForNextLighting = Random.Range(_fRandomThunderTimeMin + 500f, _fRandomThunderTimeMax + 1000f);
		_fTimerForNextLighting = 0f;
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
		_gLighting = GameObject.FindGameObjectWithTag("stormlight");
		_gLighting.AddComponent<AudioSource>();
		_gLighting.GetComponent<AudioSource>().maxDistance = 3000f;
		_gLighting.GetComponent<Light>().range = _fLightningRange;
		_gLighting.GetComponent<Light>().intensity = 0f;
		if (_gLighting == null)
		{
			Object.Instantiate(_gLighting, new Vector3(0f, _fLightningHeight, 0f), new Quaternion(0f, 0f, 0f, 0f));
		}
		_fTimeForNextLighting = Random.Range(_fRandomThunderTimeMin, _fRandomThunderTimeMax);
		_fTimerForNextLighting = 0f;
	}
	[System.Obsolete]
	private void Update()
	{
		UpdateWeather();
		UpdateThunder();
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

	public void UpdateThunder()
	{
		_fTimerForNextLighting += Time.deltaTime;
		if (_fTimerForNextLighting >= _fTimeForNextLighting)
		{
			Lightning();
		}
	}

	private void Lightning()
	{
		if (!(_gLighting != null))
		{
			return;
		}
		if (_bUsingSound)
		{
			if (_adThunderSound != null)
			{
				_gLighting.GetComponent<Animation>().Play();
				_gLighting.GetComponent<AudioSource>().clip = _adThunderSound;
				_gLighting.GetComponent<AudioSource>().Play();
			}
			else
			{
				Debug.Log(string.Concat("You have no thunder sound attached to: ", base.gameObject, " Is this by choice?"));
			}
		}
		_fTimerForNextLighting = 0f;
		_fTimeForNextLighting = Random.Range(_fRandomThunderTimeMin, _fRandomThunderTimeMax);
	}

	private IEnumerator ControlLightingLightOnOff()
	{
		_gLighting.GetComponent<Light>().color = Color.cyan;
		if (_gLighting != null)
		{
			_gLighting.GetComponent<Light>().intensity = _fLightningIntensity;
			yield return new WaitForSeconds(_fTimeBeforeLightningLightTurnsOff);
		}
		if (_gLighting != null)
		{
			_gLighting.GetComponent<Light>().intensity = 0f;
			yield return null;
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
			Debug.Log(string.Concat("We are missing rain particles on: ", base.gameObject, " For weather type: THUNDERSTORM"));
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
		_fTimerForNextLighting = 0f;
		_fTimeForNextLighting = 0f;
		if (_gLighting != null)
		{
			Object.Destroy(_gLighting);
		}
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
