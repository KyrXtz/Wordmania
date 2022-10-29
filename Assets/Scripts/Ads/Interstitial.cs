
using GoogleMobileAds.Api;
using Gravitons.UI.Modal;
using System;
using UnityEngine;

public class Interstitial : MonoBehaviour
{
    public LevelManager levelManager;
    private RewardedInterstitialAd rewardedInterstitialAd;
    public string adUnitId;
    public bool Testing = false;
    public void Start()
    {
        if (Testing)
        {
            adUnitId = "ca-app-pub-3940256099942544/5354046379";
        }
        LoadAd();
    }
    void LoadAd()
    {
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        RewardedInterstitialAd.LoadAd(adUnitId, request, adLoadCallback);
    }
    

    private void adLoadCallback(RewardedInterstitialAd ad, AdFailedToLoadEventArgs arg2)
    {
        if (arg2 == null)
        {
            rewardedInterstitialAd = ad;
            
            rewardedInterstitialAd.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresent;
            rewardedInterstitialAd.OnAdDidPresentFullScreenContent += HandleAdDidPresent;
            rewardedInterstitialAd.OnAdDidDismissFullScreenContent += HandleAdDidDismiss;
            rewardedInterstitialAd.OnPaidEvent += HandlePaidEvent;
        }
    }

    private void HandleAdFailedToPresent(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print("Rewarded interstitial ad has failed to present.");
        LoadAd();

    }

    private void HandleAdDidPresent(object sender, EventArgs args)
    {
        MonoBehaviour.print("Rewarded interstitial ad has presented.");
    }

    private void HandleAdDidDismiss(object sender, EventArgs args)
    {
        MonoBehaviour.print("Rewarded interstitial ad has dismissed presentation.");
        levelManager.isReadyForInterstitial = 3;
        LoadAd();
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {
        MonoBehaviour.print(
            "Rewarded interstitial ad has received a paid event.");
    }

    public void ShowAd()
    {
        rewardedInterstitialAd?.Show(userEarnedRewardCallback);
    }
    private void userEarnedRewardCallback(Reward reward)
    {
        // TODO: Reward the user.
        MonoBehaviour.print(
            "User earned reward");
        ModalManager.Show("Thanks!", "You watched an ad so you earn 5 coins!", levelManager.iconsForModals[9], new[] {new ModalButton(){Text = "Ok!" }
                     });
        levelManager.updateCoins(5);
    }
}