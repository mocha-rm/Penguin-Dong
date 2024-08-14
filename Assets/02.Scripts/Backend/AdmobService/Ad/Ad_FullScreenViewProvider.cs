using GoogleMobileAds.Api;
using System;
using UniRx;
using UnityEngine;
using static RandomTingles.AD.Ad_BannerViewProvider;

namespace RandomTingles.AD
{
    public class Ad_FullScreenViewProvider
    {
        public IObservable<AdEvent> AsObservable() { return _adEvent; }
        public IObservable<AdError> ErrorAsObservable() { return _adErrorEvent; }

        /// <summary>
        /// UI element activated when an ad is ready to show.
        /// </summary>
        // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
        private readonly string _adUnitId = AdConfiguration.Interstitial_Android_Id;
#elif UNITY_IPHONE
        private readonly string _adUnitId = AdConfiguration.Interstitial_Ios_Id;
#else
        private readonly string _adUnitId = "unused";
#endif

        private InterstitialAd _interstitialAd;
        private AdActions _actions;

        private Subject<AdEvent> _adEvent = new Subject<AdEvent>();
        private Subject<AdError> _adErrorEvent = new Subject<AdError>();


        /// <summary>
        /// Loads the ad.
        /// </summary>
        public void LoadAd(AdActions actions)
        {
            // Clean up the old ad before loading a new one.
            if (_interstitialAd != null)
            {
                DestroyAd();
            }
            _actions = actions;


            Debug.Log("Loading interstitial ad.");
            // Create our request used to load the ad.
            var adRequest = new AdRequest();
            _actions._onOpen?.Invoke();
            _actions._onOpen = null;

            // Send the request to load the ad.
            InterstitialAd.Load(_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
            {
                // If the operation failed with a reason.
                if (error != null)
                {
                    _adErrorEvent.OnNext(error);
                    _actions._onError?.Invoke(error);
                    _actions._onError = null;

                    _actions._onClose?.Invoke();
                    _actions._onClose = null;
                    Debug.LogError("Interstitial ad failed to load an ad with error : " + error);
                    return;
                }
                // If the operation failed for unknown reasons.
                // This is an unexpected error, please report this bug if it happens.
                if (ad == null)
                {
                    _actions._onClose?.Invoke();
                    _actions._onClose = null;

                    Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                    return;
                }

                // The operation completed successfully.
                _adEvent.OnNext(AdEvent.OnAdLoaded);
                Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
                _interstitialAd = ad;
                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);

                ShowAd();
            });
        }

        /// <summary>
        /// Shows the ad.
        /// </summary>
        public void ShowAd()
        {
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {
                Debug.Log("Showing interstitial ad.");
                _interstitialAd.Show();
            }
            else
            {
                Debug.LogError("Interstitial ad is not ready yet.");
            }

        }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        public void DestroyAd()
        {
            if (_interstitialAd != null)
            {
                Debug.Log("Destroying interstitial ad.");
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            if (_actions != null)
            {
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
            _adEvent?.Dispose();
            _adErrorEvent?.Dispose();

            _adEvent = null;
            _adErrorEvent = null;
        }

        /// <summary>
        /// Logs the ResponseInfo.
        /// </summary>
        public void LogResponseInfo()
        {
            if (_interstitialAd != null)
            {
                var responseInfo = _interstitialAd.GetResponseInfo();
                UnityEngine.Debug.Log(responseInfo);
            }
        }

        private void RegisterEventHandlers(InterstitialAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                _adEvent.OnNext(AdEvent.OnAdValue);
                Utility.CustomLog.Log(String.Format("Rewarded ad paid {0} {1}.",
                        adValue.Value,
                        adValue.CurrencyCode));

            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                _adEvent.OnNext(AdEvent.OnRecordedImpression);
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                _adEvent.OnNext(AdEvent.OnAdClick);
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                _adEvent.OnNext(AdEvent.OnAdFullScreenOpend);
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                _actions._onClose?.Invoke();
                _actions._onClose = null;
                _adEvent.OnNext(AdEvent.OnAdFullScreenClosed);
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                _adErrorEvent.OnNext(error);

                _actions._onError?.Invoke(error);
                _actions._onError = null;

                Debug.LogError(error.ToString());

                _actions._onClose?.Invoke();
                _actions._onClose = null;

            };
        }
    }
}
