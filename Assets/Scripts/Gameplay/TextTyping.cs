using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextTyping : MonoBehaviour
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
		StartCoroutine(startType(line));
	}

	private IEnumerator startType(char[] s)
	{
		yield return new WaitForSeconds(textSpeed);
		if (count < s.Length)
		{
			text.text += s[count];
			count++;
			StartCoroutine(startType(s));
		}
		else
		{
			StopAllCoroutines();
		}
	}
}
