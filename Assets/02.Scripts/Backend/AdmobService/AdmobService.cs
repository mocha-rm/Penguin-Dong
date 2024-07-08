using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using GoogleMobileAds;
using GoogleMobileAds.Api;


namespace TingleAvoid
{
    using AD;

    public enum AdType
    {
        Banner,
        FullScreen,
        Reward,
    }

    public enum AdEvent
    {
        OnAdLoaded,
        OnRecordedImpression,
        OnAdClick,
        OnAdFullScreenOpend,
        OnAdFullScreenClosed,
        OnAdValue,
    }

    public class AdmobService
    {
        public bool IsInistalize
        {
            get
            {
                if (_isInitialized.HasValue == false) return false;

                return _isInitialized.Value;
            }
        }



        bool? _isInitialized;
        Ad_BannerViewProvider _bannerProvider;
        Ad_FullScreenViewProvider _fullScreenProvider;
        Ad_RewardViewProvider _rewardProvider;

        public void Init() //Initialize at GameManager or objects of a similar class
        {
            if (_isInitialized.HasValue) return;

            _isInitialized = false;

            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                _isInitialized = true;
                _bannerProvider = new Ad_BannerViewProvider();
                _fullScreenProvider = new Ad_FullScreenViewProvider();
                _rewardProvider = new Ad_RewardViewProvider();
            });
        }

        public void Clear()
        {
            _isInitialized = null;
            _bannerProvider.Clear();
            _fullScreenProvider.Clear();
            _rewardProvider.Clear();
        }

        public void DestroyADs()
        {
            _bannerProvider.DestroyAd();
            _fullScreenProvider.DestroyAd();
            _rewardProvider.DestroyAd();
        }

        public void RequestAd(AdActions adActions)
        {
            if (IsInistalize == false)
            {
                return;
            }

            switch (adActions._type)
            {
                case AdType.Banner:
                    _bannerProvider.LoadAd(adActions);
                    break;
                case AdType.FullScreen:
                    _fullScreenProvider.LoadAd(adActions);
                    break;
                case AdType.Reward:
                    _rewardProvider.LoadAd(adActions);
                    break;
            }
        }

        public void ShowAd(AdType type)
        {
            switch (type)
            {
                case AdType.Banner:
                    _bannerProvider.ShowAd();
                    break;
                case AdType.FullScreen:
                    _fullScreenProvider.ShowAd();
                    break;
                case AdType.Reward:
                    _rewardProvider.ShowAd();
                    break;
            }
        }

        #region AboutBanner
        public float GetBannerHeight()
        {
            return _bannerProvider.GetHeight();
        }

        public void SetBannerBotPosition(float navigationHeight)
        {
            _bannerProvider.SetPostionByNavigationHeight(navigationHeight);
        }
        #endregion
    }

}
