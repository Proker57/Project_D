using GoogleMobileAds.Api;
using System;

namespace BOYAREngine.Ads
{
    public class GoogleAds
    {
        public BannerView BannerView;
        public InterstitialAd Interstitial;

        public void RequestBanner()
        {
#if UNITY_ANDROID
            const string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            const string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            const string adUnitId = "unexpected_platform";
#endif
            // Create a 320x50 banner at the top of the screen.
            BannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.TopLeft);
            // Create an empty ad request.
            var request = new AdRequest.Builder().Build();
            // Load the banner with the request.
            BannerView.LoadAd(request);
        }

        public void RequestInterstitial()
        {
#if UNITY_ANDROID
            const string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
            const string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
            const string adUnitId = "unexpected_platform";
#endif

            // Initialize an InterstitialAd.
            Interstitial = new InterstitialAd(adUnitId);

            // Called when an ad request has successfully loaded.
            //_interstitial.OnAdLoaded += HandleOnAdLoaded;
            // Called when an ad request failed to load.
            //_interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
            // Called when an ad is shown.
            //_interstitial.OnAdOpening += HandleOnAdOpened;
            // Called when the ad is closed.
            Interstitial.OnAdClosed += HandleOnAdClosed;
            // Called when the ad click caused the user to leave the application.
            //_interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

            // Create an empty ad request.
            var request = new AdRequest.Builder().Build();
            // Load the interstitial with the request.
            Interstitial.LoadAd(request);

//            if (Interstitial.IsLoaded())
//            {
//                Interstitial.Show();
//            }
        }

        private void HandleOnAdClosed(object sender, EventArgs args)
        {
            RequestBanner();
        }

    }
}

