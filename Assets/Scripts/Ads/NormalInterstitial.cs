
using GoogleMobileAds.Api;
using Gravitons.UI.Modal;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NormalInterstitial : MonoBehaviour
{
    public LevelManager levelManager;
    private InterstitialAd interstitial;
    public string adUnitId;
    public bool Testing = false;
    bool isAdShowing = false;
    public void Start()
    {
        if (Testing)
        {
            adUnitId = "ca-app-pub-3940256099942544/1033173712";
        }
        this.interstitial = new InterstitialAd(adUnitId);


        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleAdOpening;
        // Called when an ad request failed to show.
        this.interstitial.OnAdFailedToShow += HandleAdFailedToShow;
        // Called when the user should be  for interacting with the ad.
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleAdClosed;
        LoadAd();
    }
    void LoadAd()
    {
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleAdFailedToLoad event received with message: "
                             + args.LoadAdError);
    }

    public void HandleAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpening event received");
    }

    public void HandleAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleAdFailedToShow event received with message: "
                             + args.AdError.GetMessage());
        isAdShowing = false;
        checkIfHasDelegateToShow();
        LoadAd();

    }

    public void HandleAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
        levelManager.setSearchesAfterSuccessAdShown();
        isAdShowing = false;
        checkIfHasDelegateToShow();
        LoadAd();
    }



    LevelManager.delegateShowInfo _delegateShowInfo;
    GameObject _wordSearchView;
    void checkIfHasDelegateToShow()
    {
        if (_delegateShowInfo != null && _wordSearchView != null)
        {
            _delegateShowInfo(_wordSearchView);
        }
    }
    public void ShowAdWithDelegate(LevelManager.delegateShowInfo delegateShowInfo, GameObject wordSearchView)
    {
        _delegateShowInfo = delegateShowInfo;
        _wordSearchView = wordSearchView;
        isAdShowing = true;
        showAd();
        //await Task.Run(() => {
        //    while (isAdShowing) Task.Delay(50); //δοκιμασε με μικροτερο delay , και χωρις await να δουμε τι κανει sto build
        //});
    }
    public void ShowAd()
    {
        isAdShowing = true;
        showAd();
    }
    void showAd()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
        else
        {
            LoadAd();
        }
    }
    //public async Task ShowAd()
    //{
    //    if (this.interstitial.IsLoaded())
    //    {
    //        //Invoke(this.interstitial.Show());
    //        //UnityMainThreadDispatcher.Instance().Enqueue(ShowUserPanel());
    //        isAdShowing = true;
    //        UnityMainThreadDispatcher.Instance().Enqueue(actuallyShowAd());
    //        //await Task.Run(() =>
    //        //{
    //        //    while (isAdShowing) Task.Delay(50);
    //        //});
    //    }
    //    else
    //    {
    //        LoadAd();
    //    }
    //}
    //IEnumerator actuallyShowAd()
    //{
    //    this.interstitial.Show();
    //    yield return null;
    //}
}