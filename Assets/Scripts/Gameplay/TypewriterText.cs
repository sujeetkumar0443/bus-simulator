using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TypewriterText : MonoBehaviour
{
	public Text text;

	 

	private int count;

	private char[] line;

	public float textSpeed;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void StartTyping(string s)
	{
		line = s.ToCharArray();
		StartCoroutine(StartType(line));
	}

	private IEnumerator StartType(char[] s)
	{
		yield return new WaitForSeconds(textSpeed);
		if (count < s.Length)
		{
			text.text += s[count];
			count++;
			StartCoroutine(StartType(s));
		}
		else
		{
			StopAllCoroutines();
		}
	}
}
