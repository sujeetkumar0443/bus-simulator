using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelectionController : MonoBehaviour
{
    public SoundManager sound;

    public GameObject loading;

    public Image loadingbar;

    public static bool FreeMoode;

    private void Start()
    {
    }

    public void FreeMOde()
    {
        sound.ButtonClickSound();
        FreeMoode = true;
    }

    public void CareerMode()
    {
        sound.ButtonClickSound();
        FreeMoode = false;
    }
    [System.Obsolete]
    public void Play()
    {
        sound.ButtonClickSound();
        loading.SetActive(true);
        if (FreeMoode)
        {
            StartCoroutine(loadlevel(2));
        }
        else
        {
            StartCoroutine(loadlevel(1));
        }
    }
    [System.Obsolete]
    public void Back()
    {
        sound.ButtonClickSound();
        Application.LoadLevel(Application.loadedLevel - 1);
    }
    [System.Obsolete]
    private IEnumerator loadlevel(int index)
    {
        yield return new WaitForSeconds(0.02f);
        if (loadingbar.fillAmount != 1f)
        {
            loadingbar.fillAmount += 0.01f;
            StartCoroutine(loadlevel(index));
        }
        else
        {
            StopAllCoroutines();
            Application.LoadLevel(Application.loadedLevel + index);
        }
    }

    public void OpenOtherUrl(string url)
    {
        sound.ButtonClickSound();
        Application.OpenURL(url);
    }
}
