using GoogleMobileAds.Api;
using System.Collections.Generic;
using UnityEngine;

public class AdInit : MonoBehaviour
{
    //public bool Testing = false;

    public void Start()
    {
        List<string> deviceIds = new List<string>();
        deviceIds.Add("e274f6c4-e836-424b-9fc6-1d7bdff4578b");
       // deviceIds.Add("8bebf0d9-c562-4707-b56d-1d6bd0034890"); //pol

        RequestConfiguration requestConfiguration = new RequestConfiguration
            .Builder()
            .SetTestDeviceIds(deviceIds)
            .build();
        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });
        //if (Testing)
        //{
        //    //this.gameObject.GetComponent<Banner>().adUnitId = "ca-app-pub-3940256099942544/6300978111";
        //    //this.gameObject.GetComponent<Interstitial>().adUnitId = "ca-app-pub-3940256099942544/5354046379";
        //    //this.gameObject.GetComponent<Rewarded>().adUnitId = "ca-app-pub-3940256099942544/5224354917";
        //    //Rewarded ca-app - pub - 3940256099942544 / 5224354917
        //    //Rewarded Interstitial   ca - app - pub - 3940256099942544 / 5354046379
        //}
    }
}