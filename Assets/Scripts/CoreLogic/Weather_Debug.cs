using UnityEngine;

public class Weather_Debug : MonoBehaviour
{
	private Weather_Controller _clWeatherController;

	private bool _bWeatherDebugOn;

	private bool _bMoreDebugInfo;

	public GUISkin guiDebugSkin;

	private void Start()
	{
		_clWeatherController = (Weather_Controller)GetComponent(typeof(Weather_Controller));
		_bWeatherDebugOn = false;
	}

	private void Update()
	{
		BasicDebugControls();
		if (_bMoreDebugInfo)
		{
			AdvancedDebugControls();
		}
	}

	private void BasicDebugControls()
	{
		if (Input.GetKeyDown(KeyCode.O) && !_bWeatherDebugOn)
		{
			_bWeatherDebugOn = true;
		}
		else if (Input.GetKeyDown(KeyCode.O) && _bWeatherDebugOn)
		{
			_bWeatherDebugOn = false;
		}
		else if (Input.GetKeyDown(KeyCode.H) && !_bMoreDebugInfo)
		{
			_bMoreDebugInfo = true;
		}
		else if (Input.GetKeyDown(KeyCode.H) && _bMoreDebugInfo)
		{
			_bMoreDebugInfo = false;
		}
	}

	private void AdvancedDebugControls()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			_clWeatherController.UseWeatherTypeDebug(0);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			_clWeatherController.UseWeatherTypeDebug(1);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			_clWeatherController.UseWeatherTypeDebug(2);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			_clWeatherController.UseWeatherTypeDebug(3);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			_clWeatherController.UseWeatherTypeDebug(4);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			_clWeatherController.UseWeatherTypeDebug(5);
		}
	}

	private void OnGUI()
	{
		if (guiDebugSkin != null)
		{
			GUI.skin = guiDebugSkin;
		}
		else
		{
			Debug.Log("Missing Debug skin");
		}
		if (_bWeatherDebugOn)
		{
			GUI.color = Color.yellow;
			GUI.Label(new Rect(Screen.width / 2 - 120, 20f, 240f, 30f), "Debugging: WEATHER SYSTEM");
			GUI.Label(new Rect(Screen.width / 2 - 225, 40f, 450f, 30f), "Press H for more Debug information and controls");
			GUI.color = Color.red;
			GUI.Label(new Rect(20f, 60f, 300f, 30f), "Current weather:");
			GUI.Label(new Rect(320f, 60f, 100f, 30f), _clWeatherController.en_CurrWeather.ToString());
			GUI.Label(new Rect(20f, 90f, 300f, 30f), "Last weather:");
			GUI.Label(new Rect(320f, 90f, 100f, 30f), _clWeatherController.en_LastWeather.ToString());
			GUI.Label(new Rect(20f, 120f, 300f, 30f), "Current temprature:");
			GUI.Label(new Rect(320f, 120f, 100f, 30f), _clWeatherController.GetSet_fCurrTemp.ToString("F2"));
			GUI.Label(new Rect(20f, 180f, 300f, 30f), "Next weather change (days):");
			GUI.Label(new Rect(320f, 180f, 100f, 30f), _clWeatherController.Get_iAmountOfDaysToNewWeather.ToString());
			GUI.Label(new Rect(20f, 210f, 300f, 30f), "Days since last weather change:");
			GUI.Label(new Rect(320f, 210f, 100f, 30f), _clWeatherController.GetSet_iAmountOfDaysSinceLastWeather.ToString());
			GUI.Label(new Rect(20f, 240f, 300f, 30f), "Weather change:");
			GUI.Label(new Rect(320f, 240f, 100f, 30f), _clWeatherController.Get_bStartWeatherChange.ToString());
			GUI.Label(new Rect(20f, 270f, 300f, 30f), "Exiting Weather Timer:");
			GUI.Label(new Rect(320f, 270f, 100f, 30f), _clWeatherController.Get_fTimeChangeWeatherStart.ToString());
			if (_bMoreDebugInfo)
			{
				GUI.color = Color.blue;
				GUI.Label(new Rect(20f, 320f, 600f, 30f), "Press 1 to try getting a new RANDOM weather");
				GUI.Label(new Rect(20f, 350f, 600f, 30f), "Press 2 to get SUN");
				GUI.Label(new Rect(20f, 380f, 600f, 30f), "Press 3 to get CLOUDY");
				GUI.Label(new Rect(20f, 410f, 600f, 30f), "Press 4 to get RAIN");
				GUI.Label(new Rect(20f, 440f, 600f, 30f), "Press 5 to get THUNDERSTORM");
				GUI.Label(new Rect(20f, 470f, 600f, 30f), "Press 6 to get SNOW");
			}
		}
	}
}
