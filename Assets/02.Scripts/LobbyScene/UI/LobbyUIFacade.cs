using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UnityEngine.UI;
using TMPro;

using UniRx;
using Cysharp.Threading.Tasks;
using TingleAvoid.AD;
using TingleAvoid;


namespace LobbyScene.UI
{
    public class LobbyUIFacade : BaseFacade, IRegistMonobehavior
    {
        TextMeshProUGUI _myInfo;

        Button _startBtn;

        Button _soundBtn;
        Button _vibrationBtn;
        Button _rankingloadBtn;

        #region Login
        LoginService _loginService;
        DBService _dbService;

        Button _guestBtn;
        Button _googleBtn;
        TextMeshProUGUI _dataexplainText;
        #endregion

        AudioService _audioService;
        RankingService _rankService;
        RankingBoard _rankBoard;
        Transform _loadingPanel;

        //Admob
        AdmobService _admob;


        CompositeDisposable _disposables;



        public void RegistBehavior(IContainerBuilder builder)
        {
            _myInfo = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierarchy.MyInfo);

            _startBtn = gameObject.GetHierachyPath<Button>(Hierarchy.GameStartButton);
            _soundBtn = gameObject.GetHierachyPath<Button>(Hierarchy.SoundControlButton);
            _vibrationBtn = gameObject.GetHierachyPath<Button>(Hierarchy.VibrationControlButton);

            _rankingloadBtn = gameObject.GetHierachyPath<Button>(Hierarchy.RankingLoadButton);
            _rankBoard = gameObject.GetHierachyPath<RankingBoard>(Hierarchy.RankingBoard);

            _guestBtn = gameObject.GetHierachyPath<Button>(Hierarchy.GuestLoginButton);
            _googleBtn = gameObject.GetHierachyPath<Button>(Hierarchy.GoogleLoginButton);
            _dataexplainText = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierarchy.DataExplainTMP);

            _loadingPanel = gameObject.GetHierachyPath<Transform>(Hierarchy.LoadingPanel);
        }


        public override void Initialize()
        {
            _audioService = _container.Resolve<AudioService>();

            _loginService = _container.Resolve<LoginService>();

            _dbService = _container.Resolve<DBService>();

            _rankService = _container.Resolve<RankingService>();

            _admob = new AdmobService();
            _admob.Init();

           
            _disposables = new CompositeDisposable();

            var loader = _container.Resolve<SceneService>();

            var preferences = _container.Resolve<Preferences>();

            preferences.CheckSoundStatus(_soundBtn);
            preferences.CheckVibrationStatus(_vibrationBtn);

            _rankingloadBtn.interactable = false;

           
            _guestBtn.OnClickAsObservable().Subscribe(_ =>
            {
                _loadingPanel.gameObject.SetActive(true);
                _loginService.OnClickGuestLogin();
            }).AddTo(_disposables);


            _googleBtn.OnClickAsObservable().Subscribe(_ =>
            {
                //Add Later
            }).AddTo(_disposables);


            _startBtn.OnClickAsObservable().Subscribe(_ =>
            {
                _audioService.Play(AudioService.AudioResources.Button);
                loader.LoadScene(SceneName.GameScene).Forget();
            }).AddTo(_disposables);

            _startBtn.transform.parent.gameObject.SetActive(false);

            var pref = _container.Resolve<Preferences>();

            _soundBtn.OnClickAsObservable().Subscribe(_ =>
            {
                pref.SoundControl(_soundBtn);
                _audioService.Play(AudioService.AudioResources.Button);
            }).AddTo(_disposables);

            _vibrationBtn.OnClickAsObservable().Subscribe(_ =>
            {
                pref.VibrateControl(_vibrationBtn);
                _audioService.Play(AudioService.AudioResources.Button);
            }).AddTo(_disposables);

            _rankingloadBtn.OnClickAsObservable().Subscribe(_ =>
            {
                _audioService.Play(AudioService.AudioResources.Button);
                _rankBoard.gameObject.SetActive(true);
                _rankService.RequestLeaderboard(_loadingPanel.gameObject);

                IndicateRankTMP().Forget();

            }).AddTo(_disposables);


            
            Observable.EveryUpdate().Where(_ => _loginService.IsLoginSuccess).Where(_ => !_myInfo.gameObject.activeInHierarchy)
                .Subscribe(_ =>
                {
                    if (_dbService.IsUserLoaded)
                    {
                        _rankingloadBtn.interactable = true;
                        _loadingPanel.gameObject.SetActive(false);
                        _guestBtn.transform.parent.gameObject.SetActive(false);
                        _dataexplainText.gameObject.SetActive(false);
                        _startBtn.transform.parent.gameObject.SetActive(true);
                        _myInfo.gameObject.SetActive(true);

                        GetMyInfo();

                        var adAction = new AdActions()
                        {
                            _type = AdType.Banner,
                        };

                        _admob.RequestAd(adAction);
                    }
                    
                }).AddTo(_disposables);
        }

        public override void Dispose()
        {
            _startBtn.onClick.RemoveAllListeners();
            _soundBtn.onClick.RemoveAllListeners();
            _vibrationBtn.onClick.RemoveAllListeners();
            _rankingloadBtn.onClick.RemoveAllListeners();
            _guestBtn.onClick.RemoveAllListeners();
            _googleBtn.onClick.RemoveAllListeners();

            _disposables?.Dispose();
            _disposables = null;

            _admob.Clear();
        }

        private async UniTaskVoid IndicateRankTMP()
        {
            await UniTask.Delay(TimeSpan.FromMilliseconds(500f));

            if (!_loadingPanel.gameObject.activeInHierarchy)
            {
                _rankBoard.SetTMP(_rankService.GetRankInfo());
            }
        }

        private void GetMyInfo()
        {
            _rankService.MyRankChecking(_loginService.PLAYFABID, OnSuccess, error => Debug.LogError("Error:" + error));
        }
        private void OnSuccess(int rank)
        {
            int myRank = rank;
            _myInfo.text = $"Name : {_dbService.NickName}\nScore : {_dbService.BestScore}\nRank : {myRank}";//시작이 0이라서 1더해주기
        }


        public static class Hierarchy
        {
            public static readonly string MyInfo = "Background/SubText";

            public static readonly string GameStartButton = "Background/Start/GameStart";

            public static readonly string SoundControlButton = "Background/Preferences/Sound";
            public static readonly string VibrationControlButton = "Background/Preferences/Vibration";

            public static readonly string RankingLoadButton = "Background/Preferences/RankingLoad";
            public static readonly string RankingBoard = "RankingBoard";

            public static readonly string GuestLoginButton = "Background/Login/GuestLogin";
            public static readonly string GoogleLoginButton = "Background/Login/GoogleLogin";
            public static readonly string DataExplainTMP = "Background/Dataexplain";

            public static readonly string LoadingPanel = "Loading";
        }
    }
}

