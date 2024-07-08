using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;
using UniRx;
using System.Drawing;


namespace TingleAvoid.AD
{
    public class Ad_BannerViewProvider
    {
        public IObservable<AdEvent> AsObservable() { return _adEvent; }
        public IObservable<AdError> ErrorAsObservable() { return _adErrorEvent; }

        // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
        private readonly string _adUnitId = AdConfiguration.Banner_Android_Id;
#elif UNITY_IPHONE
        private readonly string _adUnitId = AdConfiguration.Banner_Ios_Id;
#else
        private readonly string _adUnitId = "unused";
#endif


        private BannerView _bannerView;
        private AdActions _actions;

        private Subject<AdEvent> _adEvent = new Subject<AdEvent>();
        private Subject<AdError> _adErrorEvent = new Subject<AdError>();

        public void CreateBannerView()
        {
            Debug.Log("Creating banner view.");

            // If we already have a banner, destroy the old one.
            if (_bannerView != null)
            {
                DestroyAd();
            }


            AdSize adaptiveSize = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            //int xDP = 0;//Mathf.RoundToInt(adaptiveSize.Width / 2f);
            //int yDP = 0;// Mathf.RoundToInt(ConvertPixelsToDP(Screen.height) - adaptiveSize.Height - MobileStatusNavigationBar.NavigationBarHeight_Pixel);


            //Debug.Log($"DP y {yDP} : {ConvertPixelsToDP(Screen.height)}, {adaptiveSize.Height}, {MobileStatusNavigationBar.NavigationBarHeight_Pixel}=================");
            // Create a 320x50 banner at top of the screen.
            _bannerView = new BannerView(_adUnitId, adaptiveSize, AdPosition.Bottom);
            // Listen to events the banner may raise.
            ListenToAdEvents();


            Debug.Log("Banner view created.");
        }

        /// <summary>
        /// Creates the banner view and loads a banner ad.
        /// </summary>
        public void LoadAd(AdActions actions)
        {
            // Create an instance of a banner view first.
            CreateBannerView();
            _actions = actions;

            var adRequest = new AdRequest();

            // Send the request to load the ad.
            Debug.Log("Loading banner ad.");
            _bannerView.LoadAd(adRequest);
            _actions._onOpen?.Invoke();
            _actions._onOpen = null;
            // Create our request used to load the ad.
        }

        /// <summary>
        /// Shows the ad.
        /// </summary>
        public void ShowAd()
        {
            if (_bannerView != null)
            {
                _bannerView.Show();
            }

        }

        public float GetHeight()
        {
#if UNITY_EDITOR
            return 135f;
#endif
            return _bannerView.GetHeightInPixels();
        }

        public void SetPostionByNavigationHeight(float height)
        {
            int yDp = Mathf.RoundToInt(ConvertPixelsToDP(Screen.height) - ConvertPixelsToDP(GetHeight()) - ConvertPixelsToDP(height));
            _bannerView.SetPosition(0, yDp);
        }

        /// <summary>
        /// Hides the ad.
        /// </summary>
        public void HideAd()
        {
            if (_bannerView != null)
            {
                Debug.Log("Hiding banner view.");
                _bannerView.Hide();
            }
        }

        /// <summary>
        /// Destroys the ad.
        /// When you are finished with a BannerView, make sure to call
        /// the Destroy() method before dropping your reference to it.
        /// </summary>
        public void DestroyAd()
        {
            if (_bannerView != null)
            {
                Debug.Log("Destroying banner view.");
                _bannerView.Destroy();
                _bannerView = null;
            }

            if (_actions != null)
            {
                _actions._onClose?.Invoke();
                _actions = null;
            }
        }

        public void Clear()
        {
            DestroyAd();
            RemoveEvents();
        }


        void RemoveEvents()
        {
            _adEvent.Dispose();
            _adErrorEvent.Dispose();

            _adEvent = null;
            _adErrorEvent = null;
        }

        /// <summary>
        /// Logs the ResponseInfo.
        /// </summary>
        public void LogResponseInfo()
        {
            if (_bannerView != null)
            {
                var responseInfo = _bannerView.GetResponseInfo();
                if (responseInfo != null)
                {
                    UnityEngine.Debug.Log(responseInfo);
                }
            }
        }

        /// <summary>
        /// Listen to events the banner may raise.
        /// </summary>
        private void ListenToAdEvents()
        {
            // Raised when an ad is loaded into the banner view.
            _bannerView.OnBannerAdLoaded += () =>
            {
                _adEvent.OnNext(AdEvent.OnAdLoaded);
            };
            // Raised when an ad fails to load into the banner view.
            _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                _adErrorEvent.OnNext(error);
                _actions._onError?.Invoke(error);
                _actions._onError = null;

                Debug.LogError(error.ToString());
            };
            // Raised when the ad is estimated to have earned money.
            _bannerView.OnAdPaid += (AdValue adValue) =>
            {
                _adEvent.OnNext(AdEvent.OnAdValue);
            };
            // Raised when an impression is recorded for an ad.
            _bannerView.OnAdImpressionRecorded += () =>
            {
                _adEvent.OnNext(AdEvent.OnRecordedImpression);
            };
            // Raised when a click is recorded for an ad.
            _bannerView.OnAdClicked += () =>
            {
                _adEvent.OnNext(AdEvent.OnAdClick);
            };
            // Raised when an ad opened full screen content.
            _bannerView.OnAdFullScreenContentOpened += () =>
            {
                _adEvent.OnNext(AdEvent.OnAdFullScreenOpend);
            };
            // Raised when the ad closed full screen content.
            _bannerView.OnAdFullScreenContentClosed += () =>
            {
                _adEvent.OnNext(AdEvent.OnAdFullScreenClosed);
            };
        }

        private float ConvertPixelsToDP(float pixels)
        {
            return pixels / (Screen.dpi / 160);
        }
    }
}

