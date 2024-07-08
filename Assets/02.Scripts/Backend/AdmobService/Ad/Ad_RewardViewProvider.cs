using GoogleMobileAds.Api;
using System;
using UniRx;
using UnityEngine;

namespace TingleAvoid.AD
{
    public class Ad_RewardViewProvider
    {
        public IObservable<AdEvent> AsObservable() { return _adEvent; }
        public IObservable<AdError> ErrorAsObservable() { return _adErrorEvent; }

        // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
        protected string _adUnitId = AdConfiguration.Reward_Android_Id;
#elif UNITY_IPHONE
        protected string _adUnitId = AdConfiguration.Reward_Ios_Id;
#else
        protected string _adUnitId = "unused";
#endif

        private RewardedAd _rewardedAd;
        private AdActions _actions;

        private Subject<AdEvent> _adEvent = new Subject<AdEvent>();
        private Subject<AdError> _adErrorEvent = new Subject<AdError>();

        /// <summary>
        /// Loads the ad.
        /// </summary>
        public void LoadAd(AdActions actions)
        {
            // Clean up the old ad before loading a new one.
            if (_rewardedAd != null)
            {
                DestroyAd();
            }
            _actions = actions;

            // Create our request used to load the ad.
            _actions._onOpen?.Invoke();
            _actions._onOpen = null;

            var adRequest = new AdRequest();
            // Send the request to load the ad.
            RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                // If the operation failed with a reason.
                if (error != null)
                {
                    _adErrorEvent.OnNext(error);

                    _actions._onError?.Invoke(error);
                    _actions._onError = null;


                    _actions._onClose?.Invoke();
                    _actions._onClose = null;


                    Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                    return;
                }
                // If the operation failed for unknown reasons.
                // This is an unexpected error, please report this bug if it happens.
                if (ad == null)
                {
                    _actions._onClose?.Invoke();
                    _actions._onClose = null;

                    Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                    return;
                }

                // The operation completed successfully.
                Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
                _rewardedAd = ad;
                _adEvent.OnNext(AdEvent.OnAdLoaded);
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
            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                Debug.Log("Showing rewarded ad.");
                _rewardedAd.Show((Reward reward) =>
                {
                    _actions._onReward?.Invoke(reward);
                    _actions._onReward = null;
                    Debug.Log(String.Format("Rewarded ad granted a reward: {0} {1}",
                                            reward.Amount,
                                            reward.Type));
                });
            }
            else
            {
                Debug.LogError("Rewarded ad is not ready yet.");
            }

        }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        public void DestroyAd()
        {
            if (_rewardedAd != null)
            {
                Debug.Log("Destroying rewarded ad.");
                _rewardedAd.Destroy();
                _rewardedAd = null;
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
            if (_rewardedAd != null)
            {
                var responseInfo = _rewardedAd.GetResponseInfo();
                UnityEngine.Debug.Log(responseInfo);
            }
        }

        private void RegisterEventHandlers(RewardedAd ad)
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
                Utility.CustomLog.Log("Rewarded ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                _adEvent.OnNext(AdEvent.OnAdClick);

                Utility.CustomLog.Log("Rewarded ad was clicked.");
            };
            // Raised when the ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                _adEvent.OnNext(AdEvent.OnAdFullScreenOpend);
                Utility.CustomLog.Log("Rewarded ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                _adEvent.OnNext(AdEvent.OnAdFullScreenClosed);
                _actions._onClose?.Invoke();
                _actions._onClose = null;
                Utility.CustomLog.Log("Rewarded ad full screen content closed.");
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                _adErrorEvent.OnNext(error);

                _actions._onError?.Invoke(error);
                _actions._onError = null;

                _actions._onClose?.Invoke();
                _actions._onClose = null;
                Utility.CustomLog.LogError("Rewarded ad failed to open full screen content with error : "
                    + error);

            };
        }
    }
}
