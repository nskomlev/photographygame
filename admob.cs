using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class admob : MonoBehaviour {

    public static admob instance = null;     //Allows other scripts to call functions from SoundManager.             

    // Initialize an InterstitialAd.
    InterstitialAd interstitial;


    // Use this for initialization
    void Awake () {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);
    }
	
    public void loadBannerAd()
    {
        string adID = "ca-app-pub-9910378041227673/9554403085"; 
        string deviceId = "B21E137E9B0F4CA8";

        interstitial = new InterstitialAd(adID);

        AdRequest request = new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)       // Simulator.
            .AddTestDevice(deviceId)  // My test device.
            .Build();

        interstitial.LoadAd(request);

    }

    public void playGameButton()
    {
        loadBannerAd();
    }
    public void pauseGameButton()
    {
        print("SHOW ADV");
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
            //panels.instance.devInfoUpdate("banner load");
        }
        else
        {
           // panels.instance.devInfoUpdate("banner NOT load");
        }
    }

}
