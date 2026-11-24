using System.Collections;
using UnityEngine;

public class Weather_SoundFade : MonoBehaviour
{
	private enum Fade
	{
		IN = 0,
		OUT = 1
	}

	public void FadeAudioIn(float fTimeToFadeIn, float fEndVolume)
	{
		StartCoroutine(FadeAudio(fTimeToFadeIn, fEndVolume, Fade.IN));
	}

	public void FadeAudioOut(float fTimeToFadeIn, float fEndVolume)
	{
		StartCoroutine(FadeAudio(fTimeToFadeIn, fEndVolume, Fade.OUT));
	}

	private IEnumerator FadeAudio(float fTimeToFade, float fSoundVolume, Fade fadeType)
	{
		float start = ((fadeType != 0) ? fSoundVolume : 0f);
		float end = ((fadeType != 0) ? 0f : fSoundVolume);
		float i = 0f;
		float step = 1f / fTimeToFade;
		while (i < 1f)
		{
			i += Time.deltaTime * step;
			GetComponent<AudioSource>().volume = Mathf.Lerp(start, end, i);
			yield return new WaitForSeconds(step * Time.deltaTime);
		}
	}
}
