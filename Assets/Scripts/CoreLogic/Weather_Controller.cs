using UnityEngine;

public class Weather_Controller : MonoBehaviour
{
	public enum WeatherType
	{
		RANDOM = 0,
		SUN = 1,
		CLOUDY = 2,
		RAIN = 3,
		THUNDERSTORM = 4,
		SNOW = 5,
		NUMBEROFWEATHERTYPES = 6
	}

	private bool _bChangeWeather;

	private int _iNewWeather;

	private bool _bStartWeatherChange;

	private float _fTimeChangeWeatherStart;

	private float _fTimeChangeWeatherEnd;

	[SerializeField]
	private bool _bUsingProceduralSkybox;

	[SerializeField]
	private bool _bUseSun = true;

	[SerializeField]
	private bool _bUseCloudy = true;

	[SerializeField]
	private bool _bUseRain = true;

	[SerializeField]
	private bool _bUseThunderstorm = true;

	[SerializeField]
	private bool _bUseSnow = true;

	[SerializeField]
	private bool _bUseRandomWeather = true;

	[SerializeField]
	private bool _bUseRandomDaysWeather;

	private int _iAmountOfDaysToNewWeather;

	private int _iAmountOfDaysSinceLastWeather;

	[SerializeField]
	private int _iChangeWeatherAfterDays = 4;

	[SerializeField]
	private int _iMinAmountOfDaysToNewWeather = 1;

	[SerializeField]
	private int _iMaxAmountOfDaysToNewWeather = 10;

	private float _fCurrTemp;

	public GameObject gTimeOfDay;

	public Material matClouds;

	public Material matSkybox;

	public WeatherType en_CurrWeather;

	[HideInInspector]
	public WeatherType en_LastWeather;

	public bool Get_bChangeWeather
	{
		get
		{
			return _bChangeWeather;
		}
	}

	public bool Get_bStartWeatherChange
	{
		get
		{
			return _bStartWeatherChange;
		}
	}

	public float Get_fTimeChangeWeatherStart
	{
		get
		{
			return _fTimeChangeWeatherStart;
		}
	}

	public int Get_iAmountOfDaysToNewWeather
	{
		get
		{
			return _iAmountOfDaysToNewWeather;
		}
	}

	public bool GetSet_bUsingProceduralSkybox
	{
		get
		{
			return _bUsingProceduralSkybox;
		}
		set
		{
			_bUsingProceduralSkybox = value;
		}
	}

	public bool GetSet_bUseSun
	{
		get
		{
			return _bUseSun;
		}
		set
		{
			_bUseSun = value;
		}
	}

	public bool GetSet_bUseCloudy
	{
		get
		{
			return _bUseCloudy;
		}
		set
		{
			_bUseCloudy = value;
		}
	}

	public bool GetSet_bUseRain
	{
		get
		{
			return _bUseRain;
		}
		set
		{
			_bUseRain = value;
		}
	}

	public bool GetSet_bUseThunderstorm
	{
		get
		{
			return _bUseThunderstorm;
		}
		set
		{
			_bUseThunderstorm = value;
		}
	}

	public bool GetSet_bUseSnow
	{
		get
		{
			return _bUseSnow;
		}
		set
		{
			_bUseSnow = value;
		}
	}

	public bool GetSet_bUseRandomWeather
	{
		get
		{
			return _bUseRandomWeather;
		}
		set
		{
			_bUseRandomWeather = value;
		}
	}

	public bool GetSet_bUseRandomDaysWeather
	{
		get
		{
			return _bUseRandomDaysWeather;
		}
		set
		{
			_bUseRandomDaysWeather = value;
		}
	}

	public int GetSet_iChangeWeatherAfterDays
	{
		get
		{
			return _iChangeWeatherAfterDays;
		}
		set
		{
			_iChangeWeatherAfterDays = value;
		}
	}

	public int GetSet_iAmountOfDaysSinceLastWeather
	{
		get
		{
			return _iAmountOfDaysSinceLastWeather;
		}
		set
		{
			_iAmountOfDaysSinceLastWeather = value;
		}
	}

	public int GetSet_iMinAmountOfDaysToNewWeather
	{
		get
		{
			return _iMinAmountOfDaysToNewWeather;
		}
		set
		{
			_iMinAmountOfDaysToNewWeather = value;
		}
	}

	public int GetSet_iMaxAmountOfDaysToNewWeather
	{
		get
		{
			return _iMaxAmountOfDaysToNewWeather;
		}
		set
		{
			_iMaxAmountOfDaysToNewWeather = value;
		}
	}

	public float GetSet_fCurrTemp
	{
		get
		{
			return _fCurrTemp;
		}
		set
		{
			_fCurrTemp = value;
		}
	}
	[System.Obsolete]
	private void Start()
	{
		_fTimeChangeWeatherStart = 0f;
		_fTimeChangeWeatherEnd = 5f;
		if (_bUseRandomDaysWeather)
		{
			_iAmountOfDaysToNewWeather = Random.Range(_iMinAmountOfDaysToNewWeather, _iMaxAmountOfDaysToNewWeather);
		}
		else
		{
			_iAmountOfDaysToNewWeather = _iChangeWeatherAfterDays;
		}
		if ((en_CurrWeather == WeatherType.RANDOM && _bUseRandomWeather) || (en_CurrWeather == WeatherType.NUMBEROFWEATHERTYPES && _bUseRandomWeather))
		{
			PickRandomWeather();
		}
		else if ((en_CurrWeather == WeatherType.RANDOM && !_bUseRandomWeather) || (en_CurrWeather == WeatherType.NUMBEROFWEATHERTYPES && !_bUseRandomWeather))
		{
			Debug.LogWarning("You haven't picked which weather to use, we default to SUN if the weather type is still set to \"RANDOM or NUMBEROFWEATHERTYPES\" and \"Use random weather\" is off");
			ExitCurrentWeather(1);
		}
		else
		{
			EnterNewWeather((int)en_CurrWeather);
		}
	}
	[System.Obsolete]
	private void Update()
	{
		if (_bUseRandomWeather)
		{
			if (_bUseRandomDaysWeather)
			{
				if (_iAmountOfDaysSinceLastWeather >= _iAmountOfDaysToNewWeather)
				{
					_bChangeWeather = true;
					_iAmountOfDaysSinceLastWeather = 0;
					_iAmountOfDaysToNewWeather = Random.Range(_iMinAmountOfDaysToNewWeather, _iMaxAmountOfDaysToNewWeather);
				}
			}
			else if (_iAmountOfDaysSinceLastWeather >= _iChangeWeatherAfterDays)
			{
				_bChangeWeather = true;
				_iAmountOfDaysSinceLastWeather = 0;
			}
		}
		if (_bChangeWeather)
		{
			PickRandomWeather();
		}
		if (_bStartWeatherChange)
		{
			ExitCurrentWeather(_iNewWeather);
		}
	}

	private void PickRandomWeather()
	{
		int num = Random.Range(1, 6);
		if (num != (int)en_CurrWeather)
		{
			CheckIfWeatherTypeIsOn(num);
		}
		else
		{
			Debug.Log("We got the same weather no change will happen!");
		}
		_bChangeWeather = false;
	}

	private void CheckIfWeatherTypeIsOn(int NewWeatherType)
	{
		if (NewWeatherType == 1 && _bUseSun)
		{
			_iNewWeather = NewWeatherType;
			_bStartWeatherChange = true;
		}
		else if (NewWeatherType == 2 && _bUseCloudy)
		{
			_iNewWeather = NewWeatherType;
			_bStartWeatherChange = true;
		}
		else if (NewWeatherType == 3 && _bUseRain)
		{
			_iNewWeather = NewWeatherType;
			_bStartWeatherChange = true;
		}
		else if (NewWeatherType == 4 && _bUseThunderstorm)
		{
			_iNewWeather = NewWeatherType;
			_bStartWeatherChange = true;
		}
		else if (NewWeatherType == 5 && _bUseSnow)
		{
			_iNewWeather = NewWeatherType;
			_bStartWeatherChange = true;
		}
		else
		{
			Debug.Log("Weather type was not on, so we are trying again!");
			PickRandomWeather();
		}
	}

	private void ChangeWeatherToSun()
	{
		en_CurrWeather = WeatherType.SUN;
		GetComponent<Weather_Sun>().enabled = true;
		GetComponent<Weather_Sun>().GetSet_bUseInit = true;
	}

	private void ChangeWeatherToCloudy()
	{
		en_CurrWeather = WeatherType.CLOUDY;
		GetComponent<Weather_Cloudy>().enabled = true;
		GetComponent<Weather_Cloudy>().GetSet_bUseInit = true;
	}

	private void ChangeWeatherToRain()
	{
		en_CurrWeather = WeatherType.RAIN;
		GetComponent<Weather_Rain>().enabled = true;
		GetComponent<Weather_Rain>().GetSet_bUseInit = true;
	}

	private void ChangeWeatherToThunderstorm()
	{
		en_CurrWeather = WeatherType.THUNDERSTORM;
		GetComponent<Weather_Thunderstorm>().enabled = true;
		GetComponent<Weather_Thunderstorm>().GetSet_bUseInit = true;
	}

	private void ChangeWeatherToSnow()
	{
		en_CurrWeather = WeatherType.SNOW;
		GetComponent<Weather_Snow>().enabled = true;
		GetComponent<Weather_Snow>().GetSet_bUseInit = true;
	}
	[System.Obsolete]
	private void ExitCurrentWeather(int NewWeatherType)
	{
		if (en_CurrWeather == WeatherType.RANDOM)
		{
			en_LastWeather = WeatherType.RANDOM;
			EnterNewWeather(NewWeatherType);
			_fTimeChangeWeatherStart = 0f;
			_bStartWeatherChange = false;
		}
		else if (en_CurrWeather == WeatherType.SUN)
		{
			en_LastWeather = WeatherType.SUN;
			_fTimeChangeWeatherStart += Time.deltaTime;
			GetComponent<Weather_Sun>().ExitWeatherEffect(GetComponent<Weather_Sun>().GetSet_gSoundEffect);
			if (_fTimeChangeWeatherStart >= _fTimeChangeWeatherEnd)
			{
				GetComponent<Weather_Sun>().enabled = false;
				EnterNewWeather(NewWeatherType);
				_fTimeChangeWeatherStart = 0f;
				_bStartWeatherChange = false;
			}
		}
		else if (en_CurrWeather == WeatherType.CLOUDY)
		{
			en_LastWeather = WeatherType.CLOUDY;
			_fTimeChangeWeatherStart += Time.deltaTime;
			GetComponent<Weather_Cloudy>().ExitWeatherEffect(GetComponent<Weather_Cloudy>().GetSet_gSoundEffect);
			if (_fTimeChangeWeatherStart >= _fTimeChangeWeatherEnd)
			{
				GetComponent<Weather_Cloudy>().enabled = false;
				EnterNewWeather(NewWeatherType);
				_fTimeChangeWeatherStart = 0f;
				_bStartWeatherChange = false;
			}
		}
		else if (en_CurrWeather == WeatherType.RAIN)
		{
			en_LastWeather = WeatherType.RAIN;
			_fTimeChangeWeatherStart += Time.deltaTime;
			GetComponent<Weather_Rain>().ExitWeatherEffect(GetComponent<Weather_Rain>().GetSet_gPartRain);
			if (_fTimeChangeWeatherStart >= _fTimeChangeWeatherEnd)
			{
				GetComponent<Weather_Rain>().enabled = false;
				EnterNewWeather(NewWeatherType);
				_fTimeChangeWeatherStart = 0f;
				_bStartWeatherChange = false;
			}
		}
		else if (en_CurrWeather == WeatherType.THUNDERSTORM)
		{
			en_LastWeather = WeatherType.THUNDERSTORM;
			_fTimeChangeWeatherStart += Time.deltaTime;
			GetComponent<Weather_Thunderstorm>().ExitWeatherEffect(GetComponent<Weather_Thunderstorm>().GetSet_gPartRain);
			if (_fTimeChangeWeatherStart >= _fTimeChangeWeatherEnd)
			{
				GetComponent<Weather_Thunderstorm>().enabled = false;
				EnterNewWeather(NewWeatherType);
				_fTimeChangeWeatherStart = 0f;
				_bStartWeatherChange = false;
			}
		}
		else if (en_CurrWeather == WeatherType.SNOW)
		{
			en_LastWeather = WeatherType.SNOW;
			_fTimeChangeWeatherStart += Time.deltaTime;
			GetComponent<Weather_Snow>().ExitWeatherEffect(GetComponent<Weather_Snow>().GetSet_gPartSnow);
			if (_fTimeChangeWeatherStart >= _fTimeChangeWeatherEnd)
			{
				GetComponent<Weather_Snow>().enabled = false;
				EnterNewWeather(NewWeatherType);
				_fTimeChangeWeatherStart = 0f;
				_bStartWeatherChange = false;
			}
		}
	}

	private void EnterNewWeather(int NewWeather)
	{
		switch (NewWeather)
		{
		case 1:
			ChangeWeatherToSun();
			break;
		case 2:
			ChangeWeatherToCloudy();
			break;
		case 3:
			ChangeWeatherToRain();
			break;
		case 4:
			ChangeWeatherToThunderstorm();
			break;
		case 5:
			ChangeWeatherToSnow();
			break;
		}
	}

	public void UpdateAllWeather(float sunIntensity, Color sunLightColor, float moonIntensity, Color moonLightColor, Color skyTint, Color skyGround, Color cloudColor, float fogDensity, Color fogColor, float fadeTime)
	{
		gTimeOfDay.GetComponent<ToD_Base>().lSun.intensity = Mathf.Lerp(gTimeOfDay.GetComponent<ToD_Base>().lSun.intensity, sunIntensity, Time.deltaTime / fadeTime);
		gTimeOfDay.GetComponent<ToD_Base>().lSun.color = Color.Lerp(gTimeOfDay.GetComponent<ToD_Base>().lSun.color, sunLightColor, Time.deltaTime / fadeTime);
		if (gTimeOfDay.GetComponent<ToD_Base>().GetSet_bUseMoon)
		{
			gTimeOfDay.GetComponent<ToD_Base>().lMoon.intensity = Mathf.Lerp(gTimeOfDay.GetComponent<ToD_Base>().lMoon.intensity, moonIntensity, Time.deltaTime / fadeTime);
			gTimeOfDay.GetComponent<ToD_Base>().lMoon.color = Color.Lerp(gTimeOfDay.GetComponent<ToD_Base>().lMoon.color, moonLightColor, Time.deltaTime / fadeTime);
		}
		if (!_bUsingProceduralSkybox)
		{
			RenderSettings.skybox.SetColor("_Tint", Color.Lerp(RenderSettings.skybox.GetColor("_Tint"), skyTint, Time.deltaTime / fadeTime));
		}
		else
		{
			RenderSettings.skybox.SetColor("_SkyTint", Color.Lerp(RenderSettings.skybox.GetColor("_SkyTint"), skyTint, Time.deltaTime / fadeTime));
			RenderSettings.skybox.SetColor("_GroundColor", Color.Lerp(RenderSettings.skybox.GetColor("_GroundColor"), skyGround, Time.deltaTime / fadeTime));
		}
		if (matClouds != null)
		{
			matClouds.color = Color.Lerp(matClouds.color, cloudColor, Time.deltaTime / fadeTime);
		}
		else
		{
			Debug.LogWarning("We have no cloud material attached to:" + base.gameObject);
		}
		RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, fogDensity, Time.deltaTime / fadeTime);
		RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogColor, Time.deltaTime / fadeTime);
	}
	[System.Obsolete]
	public void ActivateTimesetParticle(GameObject CurrParticles)
	{
		if (!(CurrParticles != null))
		{
			return;
		}
		if ((bool)CurrParticles.gameObject.GetComponent<ParticleSystem>())
		{
			if (CurrParticles.gameObject.GetComponent<ParticleSystem>().enableEmission)
			{
				return;
			}
			CurrParticles.gameObject.GetComponent<ParticleSystem>().enableEmission = true;
			if (CurrParticles.transform.childCount != 0)
			{
				for (int i = 0; i < CurrParticles.transform.childCount; i++)
				{
					CurrParticles.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>().enableEmission = true;
				}
			}
		}
		else if (CurrParticles.transform.childCount != 0)
		{
			for (int j = 0; j < CurrParticles.transform.childCount; j++)
			{
				CurrParticles.transform.GetChild(j).gameObject.GetComponent<ParticleSystem>().enableEmission = true;
			}
		}
	}
	[System.Obsolete]
	public void DeactivateTimesetParticle(GameObject CurrParticles)
	{
		if (!(CurrParticles != null))
		{
			return;
		}
		if ((bool)CurrParticles.gameObject.GetComponent<ParticleSystem>())
		{
			if (!CurrParticles.gameObject.GetComponent<ParticleSystem>().enableEmission)
			{
				return;
			}
			CurrParticles.gameObject.GetComponent<ParticleSystem>().enableEmission = false;
			if (CurrParticles.transform.childCount != 0)
			{
				for (int i = 0; i < CurrParticles.transform.childCount; i++)
				{
					CurrParticles.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>().enableEmission = false;
				}
			}
		}
		else if (CurrParticles.transform.childCount != 0)
		{
			for (int j = 0; j < CurrParticles.transform.childCount; j++)
			{
				CurrParticles.transform.GetChild(j).gameObject.GetComponent<ParticleSystem>().enableEmission = false;
			}
		}
	}

	public void UseWeatherTypeDebug(int WeatherType)
	{
		if (WeatherType == 0)
		{
			_bChangeWeather = true;
			return;
		}
		_iNewWeather = WeatherType;
		_bStartWeatherChange = true;
	}
}
