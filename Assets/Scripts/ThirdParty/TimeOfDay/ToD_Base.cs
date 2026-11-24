using UnityEngine;

public class ToD_Base : MonoBehaviour
{
	public enum Timeset
	{
		SUNRISE = 0,
		DAY = 1,
		SUNSET = 2,
		NIGHT = 3
	}

	[SerializeField]
	private bool _bUseMoon = true;

	[SerializeField]
	private bool _bUseWeather = true;

	[SerializeField]
	private float _fSecondInAFullDay = 60f;

	[SerializeField]
	private float _fTimeMultiplier = 1f;

	[SerializeField]
	[Range(0f, 1f)]
	private float _fCurrentTimeOfDay;

	private const float ONEHOURLENGTH = 1f / 24f;

	[SerializeField]
	private int _iStartHour;

	private float _fStartingHour;

	[SerializeField]
	private int _iSunriseStart;

	[SerializeField]
	private int _iDayStart;

	[SerializeField]
	private int _iSunsetStart;

	[SerializeField]
	private int _iNightStart;

	private float _fStartingSunrise;

	private float _fStartingDay;

	private float _fStartingSunset;

	private float _fStartingNight;

	private float _fCurrentHour;

	private float _fCurrentMinute;

	private int _iAmountOfDaysPlayed;

	public GameObject gWeatherMaster;

	public Light lSun;

	public Light lMoon;

	[HideInInspector]
	public Timeset enCurrTimeset;

	public float Get_fCurrentTimeOfDay
	{
		get
		{
			return _fCurrentTimeOfDay;
		}
	}

	public float Get_fCurrentHour
	{
		get
		{
			return _fCurrentHour;
		}
	}

	public float Get_fCurrentMinute
	{
		get
		{
			return _fCurrentMinute;
		}
	}

	public int Get_iAmountOfDaysPlayed
	{
		get
		{
			return _iAmountOfDaysPlayed;
		}
	}

	public bool GetSet_bUseMoon
	{
		get
		{
			return _bUseMoon;
		}
		set
		{
			_bUseMoon = value;
		}
	}

	public bool GetSet_bUseWeather
	{
		get
		{
			return _bUseWeather;
		}
		set
		{
			_bUseWeather = value;
		}
	}

	public float GetSet_fSecondInAFullDay
	{
		get
		{
			return _fSecondInAFullDay;
		}
		set
		{
			_fSecondInAFullDay = value;
		}
	}

	public float GetSet_fTimeMultiplier
	{
		get
		{
			return _fTimeMultiplier;
		}
		set
		{
			_fTimeMultiplier = value;
		}
	}

	public int GetSet_iStartHour
	{
		get
		{
			return _iStartHour;
		}
		set
		{
			_iStartHour = value;
		}
	}

	public int GetSet_iSunriseStart
	{
		get
		{
			return _iSunriseStart;
		}
		set
		{
			_iSunriseStart = value;
		}
	}

	public int GetSet_iDayStart
	{
		get
		{
			return _iDayStart;
		}
		set
		{
			_iDayStart = value;
		}
	}

	public int GetSet_iSunsetStart
	{
		get
		{
			return _iSunsetStart;
		}
		set
		{
			_iSunsetStart = value;
		}
	}

	public int GetSet_iNightStart
	{
		get
		{
			return _iNightStart;
		}
		set
		{
			_iNightStart = value;
		}
	}

	private void Start()
	{
		_fStartingHour = 1f / 24f * (float)_iStartHour;
		_fCurrentTimeOfDay = _fStartingHour;
		_fStartingSunrise = 1f / 24f * (float)_iSunriseStart;
		_fStartingDay = 1f / 24f * (float)_iDayStart;
		_fStartingSunset = 1f / 24f * (float)_iSunsetStart;
		_fStartingNight = 1f / 24f * (float)_iNightStart;
		_iAmountOfDaysPlayed = 0;
		_fCurrentHour = 0f;
		_fCurrentMinute = 0f;
	}

	private void Update()
	{
		UpdateSunAndMoon();
		UpdateTimeset();
		_fCurrentTimeOfDay += Time.deltaTime / _fSecondInAFullDay * _fTimeMultiplier;
		_fCurrentHour = 24f * _fCurrentTimeOfDay;
		_fCurrentMinute = 60f * (_fCurrentHour - Mathf.Floor(_fCurrentHour));
		if (_fCurrentTimeOfDay >= 1f)
		{
			_fCurrentTimeOfDay = 0f;
			_iAmountOfDaysPlayed++;
			if (_bUseWeather)
			{
				gWeatherMaster.GetComponent<Weather_Controller>().GetSet_iAmountOfDaysSinceLastWeather++;
			}
		}
	}

	private void UpdateSunAndMoon()
	{
		lSun.transform.localRotation = Quaternion.Euler(_fCurrentTimeOfDay * 360f - 90f, 170f, 0f);
		if (_bUseMoon)
		{
			lMoon.transform.localRotation = Quaternion.Euler(_fCurrentTimeOfDay * 360f - 270f, 170f, 0f);
		}
	}

	private void UpdateTimeset()
	{
		if (_fCurrentTimeOfDay >= _fStartingSunrise && _fCurrentTimeOfDay <= _fStartingDay && enCurrTimeset != 0)
		{
			SetCurrentTimeset(Timeset.SUNRISE);
		}
		else if (_fCurrentTimeOfDay >= _fStartingDay && _fCurrentTimeOfDay <= _fStartingSunset && enCurrTimeset != Timeset.DAY)
		{
			SetCurrentTimeset(Timeset.DAY);
		}
		else if (_fCurrentTimeOfDay >= _fStartingSunset && _fCurrentTimeOfDay <= _fStartingNight && enCurrTimeset != Timeset.SUNSET)
		{
			SetCurrentTimeset(Timeset.SUNSET);
		}
		else if (_fCurrentTimeOfDay >= _fStartingNight || (_fCurrentTimeOfDay <= _fStartingSunrise && enCurrTimeset != Timeset.NIGHT))
		{
			SetCurrentTimeset(Timeset.NIGHT);
		}
	}

	private void SetCurrentTimeset(Timeset currentTime)
	{
		enCurrTimeset = currentTime;
	}
}
