using UnityEngine;

public class PrivacyController : MonoBehaviour
{
    private const string AcceptKey = "Accept";


    [SerializeField] private GameObject privacyContainer;
  

    private void Start()
    {
        if (PlayerPrefs.HasKey(AcceptKey))
        {
            privacyContainer.SetActive(false);
        }
        else
        {
            // If the user hasn't accepted, show the privacy container
            privacyContainer.SetActive(true);
        }
    }

    public void Accept()
    {
        // Mark the user as accepted
        PlayerPrefs.SetInt(AcceptKey, 1);
        privacyContainer.SetActive(false);
    }

    public void Visit()
    {
        // Open a specific URL (replace "yourURL" with the actual URL)
        Application.OpenURL("https://www.google.com/");

        // For the first time, you might want to do additional setup here
    }

    
}
