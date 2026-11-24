using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BusSimulator
{
    public class InterstitialAdController : MonoBehaviour
    {
        private bool isLoaded;

        public Text statusLabel;

        public void LoadInterstitial()
        {
            statusLabel.text = "Loading interstitial ad...";
            // Load the interstitial ad
            Advertisements.Instance.ShowInterstitial();

            // Check if the ad is available after loading
            if (Advertisements.Instance.IsInterstitialAvailable())
            {
                isLoaded = true;
                statusLabel.text = "Ad loaded. Click 'Show' to watch.";
            }
            else
            {
                isLoaded = false;
                statusLabel.text = "Ad not loaded. Click 'Load' to request an ad.";
            }
        }

        public void ShowInterstitial()
        {
            if (isLoaded)
            {
                // Show the interstitial ad
                Advertisements.Instance.ShowInterstitial();
                isLoaded = false;
                statusLabel.text = string.Empty;
            }
            else
            {
                statusLabel.text = "Ad not loaded. Click 'Load' to request an ad.";
            }
        }

        private void OnDestroy()
        {
            // Perform any necessary cleanup for the Gley plugin
            Debug.Log("InterstitialAdScene was destroyed!");
        }

        public void NextScene()
        {
            SceneManager.LoadScene("AdViewScene");
        }
    }
}
