using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class UnityAds : MonoBehaviour, IUnityAdsListener
{
    private string gameID = "4074129";
    private string bannerID = "banner";
    private string interstitialID = "interstitial";
    bool showBanner=false;

    void Start()
    {
        
    }

    void Update(){
        if(PlayerPrefs.GetString("VideoAd")=="Yes"){
            PlayerPrefs.SetString("VideoAd","No");
            ShowInterstitial();
        }
        if(!showBanner && Advertisement.IsReady(bannerID) && PlayerPrefs.GetString("BannerAd")=="Yes"){
            showBanner=true;
            PlayerPrefs.SetString("BannerAd","None");
            ShowBanner();
        }else if(showBanner && PlayerPrefs.GetString("BannerAd")=="No"){
            showBanner=false;
            PlayerPrefs.SetString("BannerAd","None");
            HideBanner();
        }
    }

    public void ShowInterstitial()
    {
        if (Advertisement.IsReady(interstitialID))
        {
            Advertisement.Show(interstitialID);
        }
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }

    public void ShowBanner(){
        if(Advertisement.IsReady(bannerID)){
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Show(bannerID);
        }
    }

    public void OnUnityAdsReady(string placementID)
    {

    }

    public void OnUnityAdsDidFinish(string placementID, ShowResult showResult)
    {
        
    }


    public void OnUnityAdsDidError(string message)
    {
        //Show or log the error here
    }

    public void OnUnityAdsDidStart(string placementID)
    {
        //Do this if ads starts
    }
}
