using System;
using System.Collections;
using EVP;
using UnityEngine;

public class GameplayUIController : MonoBehaviour
{
    private VehicleStandardInput vehicleControls;

    public GameObject HandBrake;

    public GameObject gearDrive;

    public GameObject gearReverse;

    public GameObject gearDriveActive;

    public GameObject gearReverseActive;

    public GameObject GearBox;

    public GameObject HandBreak;

    public GameObject speedoNeedle;

    public GameObject controldialoge;

    public GameObject steercontrols;

    public GameObject steerWheelUI;

    private int gearNo;

    public SoundManager sound;

    public GameObject backlights;

    public GameObject carsignals;

    public GameObject pauseDialoge;

    public GameObject ObjectiveDialoge;

    public GameObject leftSignalActiveBtn;

    public GameObject rightSignalActiveBtn;

    public GameObject parkingSignalActiveBtn;

    public static Action<bool> door;

    public static Action<bool> BusDoorEvent;

    private bool isactive;

    public static GameplayUIController gameController;

    public GameObject vehicleUiControls;

    public GameObject fuelDialoge;

    public GameObject fuelDialogeWarningText;

    public LevelTimer timersScript;

    public GameObject fuelbar;

    private bool isGamePause;

    public GameObject SpeedUppassengers;

    public Light directionalLight;

    public GameObject rateUsDialoge;

    public GameObject busdoorbtn;

    private bool temp;

    private void Start()
    {
        if (PlayerPrefs.GetInt("transmission") == 1)
        {
            GearBox.SetActive(false);
            HandBrake.SetActive(true);
        }
        else
        {
            GearBox.SetActive(true);
            HandBrake.SetActive(false);
        }
        Advertisements.Instance.HideBanner();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseDialoge.activeSelf)
            {
                PauseButtonClicked(null);
            }
            else
            {
                ResumeButtonClicked(null);
            }
        }
    }

    private void OnEnable()
    {
        gameController = this;
    }

    private void OnDisable()
    {
        UnityEngine.Object.Destroy(gameController);
    }

    public void setDefaultControls(GameObject obj)
    {
        vehicleControls = obj.GetComponent<VehicleStandardInput>();
        backlights = obj.transform.Find("backlights").gameObject;
        carsignals = obj.transform.Find("carsignals").gameObject;
    }

    public void VehicleControlsActive(bool isActive)
    {
        vehicleUiControls.SetActive(isActive);
    }

    public void DriveGearPress(GameObject obj)
    {
        gearDrive.SetActive(false);
        gearReverse.SetActive(true);
    }

    public void DriveGearActivePress(GameObject obj)
    {
        gearDrive.SetActive(true);
        gearDriveActive.SetActive(false);
        gearReverseActive.SetActive(true);
        gearReverse.SetActive(false);
    }

    public void ReverseGearPress(GameObject obj)
    {
        gearReverse.SetActive(false);
        gearDrive.SetActive(true);
    }

    public void ReverseActivePress(GameObject obj)
    {
        gearReverse.SetActive(true);
        gearReverseActive.SetActive(false);
        gearDriveActive.SetActive(true);
        gearDrive.SetActive(false);
    }

    public void ButtonReleased(GameObject obj)
    {
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void RaceButtonPressed()
    {
        if (PlayerPrefs.GetInt("transmission") == 2)
        {
            if (gearDrive.activeSelf)
            {
                vehicleControls.forwardInput = 1f;
            }
            else if (gearReverse.activeSelf)
            {
                vehicleControls.forwardInput = 0f;
                vehicleControls.reverseInput = 1f;
            }
        }
        else
        {
            vehicleControls.forwardInput = 1f;
        }
    }

    public void RaceButtonReleased()
    {
        vehicleControls.forwardInput = 0f;
        vehicleControls.reverseInput = 0f;
    }

    public void ReverseButtonPressed()
    {
        if (PlayerPrefs.GetInt("transmission") == 2)
        {
            vehicleControls.handbrakeInput = 1f;
            if (backlights != null)
            {
                backlights.SetActive(true);
            }
        }
        else
        {
            vehicleControls.forwardInput = 0f;
            vehicleControls.reverseInput = 1f;
        }
    }

    public void ReverseButtonReleased()
    {
        if (PlayerPrefs.GetInt("transmission") == 2)
        {
            vehicleControls.handbrakeInput = 0f;
            if (backlights != null)
            {
                backlights.SetActive(false);
            }
        }
        else
        {
            vehicleControls.forwardInput = 0f;
            vehicleControls.reverseInput = 0f;
        }
    }

    public void BreakButtonPressed()
    {
        vehicleControls.handbrakeInput = 1f;
        if (backlights != null)
        {
            backlights.SetActive(true);
        }
    }

    public void BreakButtonReleased()
    {
        vehicleControls.handbrakeInput = 0f;
        if (backlights != null)
        {
            backlights.SetActive(false);
        }
    }

    public void NosButtonPressed()
    {
    }

    public void NosButtonReleased()
    {
    }

    public void LeftButtonPressed(GameObject obj)
    {
        obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        vehicleControls.steerInput = -1f;
    }

    public void LeftButtonReleased(GameObject obj)
    {
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        vehicleControls.steerInput = 0f;
    }

    public void RightButtonPressed(GameObject obj)
    {
        obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        vehicleControls.steerInput = 1f;
    }

    public void RightButtonReleased(GameObject obj)
    {
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        vehicleControls.steerInput = 0f;
    }

    public void ResetInputs()
    {
    }

    public void CameraSwitchPressed(GameObject obj)
    {
        obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }

    public void CameraSwitchReleased(GameObject obj)
    {
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void PauseButtonClicked(GameObject obj)
    {
        isGamePause = true;
        sound.ButtonClickSound();
        Time.timeScale = 0f;
        pauseDialoge.SetActive(true);
        AudioListener.volume = 0f;
        VehicleManager.vehicleManager.PauseVehicleSounds(false);
        AdManager.Instance.ShowInterstitial();
    }

    public void ResumeButtonClicked(GameObject obj)
    {
        isGamePause = false;
        sound.ButtonClickSound();
        Time.timeScale = 1f;
        pauseDialoge.SetActive(false);
        AudioListener.volume = 1f;
        VehicleManager.vehicleManager.PauseVehicleSounds(true);
    }
    [System.Obsolete]
    public void RestartButtonClicked(GameObject obj)
    {
        obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        sound.ButtonClickSound();
        Time.timeScale = 1f;
        Application.LoadLevel(Application.loadedLevel);
    }
    [System.Obsolete]
    public void LevelCompleteRestart(GameObject obj)
    {
        obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        sound.ButtonClickSound();
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("levelno", PlayerPrefs.GetInt("levelno") - 1);
        Application.LoadLevel(Application.loadedLevel);
    }
    [System.Obsolete]
    public void MainMenuClicked(GameObject obj)
    {
        obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        sound.ButtonClickSound();
        Time.timeScale = 1f;
        AudioListener.volume = 1f;
        Application.LoadLevel(0);
    }
    [System.Obsolete]
    public void NextLevelButtonClicked(GameObject obj)
    {
        obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        sound.ButtonClickSound();
        Time.timeScale = 1f;
        Application.LoadLevel(Application.loadedLevel);
    }

    public void objectiveok()
    {
        sound.ButtonClickSound();
        ObjectiveDialoge.SetActive(false);
        controldialoge.SetActive(true);
    }

    public void TiltControl()
    {
        GameState.starttime = true;
        GameState.tiltcontrol = true;
        GameState.steercontrol = false;
        controldialoge.SetActive(false);
        RCC.SetMobileController(RCC_Settings.MobileController.Gyro);
        if (!isGamePause)
        {
            timersScript.settime();
        }
    }

    public void SteeringControl()
    {
        GameState.starttime = true;
        GameState.tiltcontrol = false;
        GameState.steercontrol = true;
        controldialoge.SetActive(false);
        RCC.SetMobileController(RCC_Settings.MobileController.SteeringWheel);
        if (!isGamePause)
        {
            timersScript.settime();
        }
    }

    public void ButtonControl()
    {
        GameState.starttime = true;
        GameState.tiltcontrol = false;
        GameState.steercontrol = false;
        controldialoge.SetActive(false);
        RCC.SetMobileController(RCC_Settings.MobileController.TouchScreen);
        if (!isGamePause)
        {
            timersScript.settime();
        }
    }

    public void HornBtnClick(GameObject obj)
    {
        obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        sound.HornSound();
    }

    public void HornBtnOff(GameObject obj)
    {
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        sound.HornSoundoff();
    }

    public void radiobtn()
    {
        sound.ButtonClickSound();
        sound.RadioSounds();
    }

    public void leftSignal(GameObject btn)
    {
        ResetSignalLights();
        if (isactive)
        {
            carsignals.SetActive(true);
            StartCoroutine(Signalactivated(carsignals.transform.GetChild(1).gameObject, leftSignalActiveBtn));
        }
    }

    public void rightSignal(GameObject btn)
    {
        ResetSignalLights();
        if (isactive)
        {
            carsignals.SetActive(true);
            StartCoroutine(Signalactivated(carsignals.transform.GetChild(0).gameObject, rightSignalActiveBtn));
        }
    }

    public void ParkingSignal(GameObject btn)
    {
        ResetSignalLights();
        if (isactive)
        {
            carsignals.transform.GetChild(0).gameObject.SetActive(true);
            carsignals.transform.GetChild(1).gameObject.SetActive(true);
            carsignals.SetActive(false);
            StartCoroutine(Signalactivated(carsignals.gameObject, parkingSignalActiveBtn));
        }
    }

    private void ResetSignalLights()
    {
        StopAllCoroutines();
        isactive = !isactive;
        carsignals.transform.GetChild(0).gameObject.SetActive(false);
        carsignals.transform.GetChild(1).gameObject.SetActive(false);
        leftSignalActiveBtn.SetActive(false);
        rightSignalActiveBtn.SetActive(false);
        parkingSignalActiveBtn.SetActive(false);
    }

    private IEnumerator Signalactivated(GameObject temp, GameObject signalbtn)
    {
        temp.SetActive(true);
        signalbtn.SetActive(true);
        sound.signalBlinkSounds();
        yield return new WaitForSeconds(0.3f);
        temp.SetActive(false);
        signalbtn.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Signalactivated(temp, signalbtn));
    }

    public void BusDoor()
    {
        temp = !temp;
        door(temp);
        BusDoorEvent(true);
        busdoorbtn.SetActive(false);
        SpeedUppassengers.SetActive(true);
    }

    public void Feedback(GameObject obj)
    {
        obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        sound.ButtonClickSound();

    }

    public void FuelDialogeNo()
    {
        sound.ButtonClickSound();
        Time.timeScale = 1f;
        fuelDialogeWarningText.SetActive(false);
        fuelDialoge.SetActive(false);
        timersScript.StartTimeAgain();
    }

    public void FuelWatchVideo()
    {
        if (Advertisements.Instance.IsRewardVideoAvailable())
        {
            AdManager.Instance.ShowRewardedVideo();
            fuelDialogeWarningText.SetActive(false);
            fuelDialoge.SetActive(false);
            Time.timeScale = 1f;
            fuelbar.GetComponent<Animator>().SetBool("close", false);
            fuelbar.GetComponent<Animator>().SetBool("open", true);
            timersScript.ResetTime();
        }
        else
        {
            timersScript.StartTimeAgain();
            fuelDialogeWarningText.SetActive(true);
            Time.timeScale = 1f;
        }
    }

    private IEnumerator CheckUnityAd()
    {
        yield return new WaitForSeconds(0.5f);
        StopAllCoroutines();
        fuelbar.GetComponent<Animator>().SetBool("close", false);
        fuelbar.GetComponent<Animator>().SetBool("open", true);
        timersScript.ResetTime();
    }

    public void FastForward(float speed)
    {
        Time.timeScale = speed;
    }

    public void RateUs_IlikeIt()
    {
        sound.ButtonClickSound();
        rateUsDialoge.SetActive(false);
        ;
        PlayerPrefs.SetInt("rateuscounter", -1);
    }

    public void RateUs_NotNow()
    {
        sound.ButtonClickSound();
        rateUsDialoge.SetActive(false);
    }

    public void OpenOtherUrl(string url)
    {
        sound.ButtonClickSound();
        Application.OpenURL(url);
    }
}
