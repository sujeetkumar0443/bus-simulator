using UnityEngine;

public class ToD_Debug : MonoBehaviour
{
	private ToD_Base _clToDBase;

	private bool _bTodDebugOn;

	private bool _bMoreDebugInfo;

	public GUISkin guiDebugSkin;

	private void Start()
	{
		_clToDBase = (ToD_Base)GetComponent(typeof(ToD_Base));
		_bTodDebugOn = false;
		_bMoreDebugInfo = false;
	}

	private void Update()
	{
		BasicDebugControls();
	}

	private void BasicDebugControls()
	{
		if (Input.GetKeyDown(KeyCode.P) && !_bTodDebugOn)
		{
			_bTodDebugOn = true;
		}
		else if (Input.GetKeyDown(KeyCode.P) && _bTodDebugOn)
		{
			_bTodDebugOn = false;
		}
		else if (Input.GetKeyDown(KeyCode.H) && !_bMoreDebugInfo)
		{
			_bMoreDebugInfo = true;
		}
		else if (Input.GetKeyDown(KeyCode.H) && _bMoreDebugInfo)
		{
			_bMoreDebugInfo = false;
		}
		if (Input.GetKeyDown(KeyCode.Alpha0) && _clToDBase.GetSet_fTimeMultiplier <= 9.5f)
		{
			_clToDBase.GetSet_fTimeMultiplier += 0.5f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha9) && _clToDBase.GetSet_fTimeMultiplier >= 0.5f)
		{
			_clToDBase.GetSet_fTimeMultiplier -= 0.5f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha8) && _clToDBase.GetSet_fTimeMultiplier >= 0.5f)
		{
			_clToDBase.GetSet_fTimeMultiplier = 1f;
		}
	}

	private void OnGUI()
	{
		if (guiDebugSkin != null)
		{
			GUI.skin = guiDebugSkin;
		}
		if (_bTodDebugOn)
		{
			GUI.color = Color.yellow;
			GUI.Label(new Rect(Screen.width / 2 - 120, 20f, 240f, 30f), "Debugging: TIME OF DAY");
			GUI.Label(new Rect(Screen.width / 2 - 225, 40f, 450f, 30f), "Press H for more Debug information and controls");
			GUI.color = Color.red;
			GUI.Label(new Rect(20f, 60f, 200f, 30f), "Current time:");
			GUI.Label(new Rect(220f, 60f, 25f, 30f), _clToDBase.Get_fCurrentHour.ToString("F1"));
			GUI.Label(new Rect(250f, 60f, 200f, 30f), ":");
			GUI.Label(new Rect(260f, 60f, 200f, 30f), _clToDBase.Get_fCurrentMinute.ToString("F0"));
			GUI.Label(new Rect(20f, 90f, 200f, 30f), "Timeset:");
			GUI.Label(new Rect(220f, 90f, 200f, 30f), _clToDBase.enCurrTimeset.ToString());
			GUI.Label(new Rect(20f, 150f, 200f, 30f), "Current ToD Speed:");
			GUI.Label(new Rect(220f, 150f, 200f, 30f), _clToDBase.GetSet_fTimeMultiplier.ToString());
			GUI.Label(new Rect(20f, 180f, 200f, 30f), "Days played:");
			GUI.Label(new Rect(220f, 180f, 200f, 30f), _clToDBase.Get_iAmountOfDaysPlayed.ToString());
			if (_bMoreDebugInfo)
			{
				GUI.color = Color.blue;
				GUI.Label(new Rect(20f, 240f, 600f, 30f), "Press 0 to add 0.5f to the Time of Day speed");
				GUI.Label(new Rect(20f, 270f, 600f, 30f), "Press 9 to substract 0.5f off the Time of Day speed");
				GUI.Label(new Rect(20f, 300f, 600f, 30f), "Press 8 to reset Time of Day speed to 1.0f");
				GUI.Label(new Rect(20f, 330f, 600f, 30f), "Max speed is 10.0f, and Minimum speed is 0.0f");
			}
		}
	}
}
