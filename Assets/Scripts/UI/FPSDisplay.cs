using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
	private float deltaTime;

	private float msec;

	private float fps;

	private string text;

	public Text fpsText;

	private void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		msec = deltaTime * 1000f;
		fps = 1f / deltaTime;
		fpsText.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
	}

	private void OnGUI()
	{
		int width = Screen.width;
		int height = Screen.height;
		GUIStyle gUIStyle = new GUIStyle();
		Rect position = new Rect(0f, 0f, width, height * 2 / 100);
		gUIStyle.alignment = TextAnchor.UpperLeft;
		gUIStyle.fontSize = height * 2 / 100;
		gUIStyle.normal.textColor = new Color(0f, 0f, 0.5f, 1f);
		GUI.Label(position, text, gUIStyle);
	}
}
