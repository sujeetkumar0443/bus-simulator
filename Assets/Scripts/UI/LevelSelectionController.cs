using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
 

public class LevelSelectionController : MonoBehaviour
{
    public GameObject levels;
    public GameObject levelsLock;
    public SoundManager sound;
    public GameObject lockDialog;
    public GameObject loading;
    public Image loadingbar;
    public GameObject videoWarningText;

    private GameObject selectedLevel;

    private void Start()
    {
        SetLock();
        if (!PlayerPrefs.HasKey("levelno"))
        {
            PlayerPrefs.SetInt("levelno", 1);
        }
        // Show an interstitial ad
        if (Advertisements.Instance.IsInterstitialAvailable())
        {
            Advertisements.Instance.ShowInterstitial();
        }
    }

    private void SetLock()
    {
        if (!PlayerPrefs.HasKey("unlocklevelno"))
        {
            PlayerPrefs.SetInt("unlocklevelno1", 1);
        }
        for (int i = 1; i <= levels.transform.childCount; i++)
        {
            bool isLevelUnlocked = PlayerPrefs.GetInt("unlocklevelno" + i) == 1;
            levelsLock.transform.GetChild(i - 1).gameObject.SetActive(!isLevelUnlocked);
        }
    }

    public void LevelSelect(int levelnum)
    {
        sound.ButtonClickSound();
        PlayerPrefs.SetInt("levelno", levelnum);
    }

    public void LockClicked()
    {
        sound.ButtonClickSound();
        lockDialog.SetActive(true);
    }

    public void LockDialogeOkBtn(GameObject obj)
    {
        sound.ButtonClickSound();
        obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        lockDialog.SetActive(false);
    }

    public void StartLevel(GameObject obj)
    {
        sound.ButtonClickSound();
        loading.SetActive(true);
        obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        StartCoroutine(LoadLevelAsync());
    }

    private IEnumerator LoadLevelAsync()
    {
        // Use asynchronous loading to avoid blocking the main thread
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        while (!asyncLoad.isDone)
        {
            loadingbar.fillAmount = asyncLoad.progress;
            yield return null;
        }

        loadingbar.fillAmount = 1f; // Ensure the bar is full when done
    }

    public void WatchVideoToUnlock(int levelnum)
    {
        StartCoroutine(VideoShown(levelnum));
    }

    private IEnumerator VideoShown(int num)
    {
        // Simulate showing a video and unlocking the level
        yield return new WaitForSeconds(1f); // Simulate video delay
        PlayerPrefs.SetInt("unlocklevelno" + num, 1);
        SetLock(); // Update locks after unlocking
        DisableText(); // Hide warning text
    }

    private void DisableText()
    {
        videoWarningText.SetActive(false);
    }

    public void BackClicked(GameObject obj)
    {
        sound.ButtonClickSound();
        obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void ButtonReleased(GameObject obj)
    {
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void OpenOtherUrl(string url)
    {
        sound.ButtonClickSound();
        Application.OpenURL(url);
    }
}
